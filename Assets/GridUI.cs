using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
using UnityEditor;
#endif

/* TODO:
 * UI logic refinement
 * Wait for all animations
 */

namespace miniRAID.UI
{
    public class GridUI : MonoBehaviour
    {
        private DefaultInputs inputs;
        public miniRAID.UIElements.CombatView combatView;

        public Camera cinemachineBrain;
        public Cinemachine.CinemachineVirtualCamera mainVCam;
        public Cinemachine.CinemachineVirtualCamera characterFocusVCam;
        public GridShapeCursor cursor;
        public float panSpeed = 5.0f;

        [Obsolete("Use cursor.position instead.")]
        public Vector2Int currentGridPos => cursor.position;

        public float gridWorldUnitSize = 1.0f;
        Stack<UIState> stateStack = new Stack<UIState>();
        public UIState currentState { get; protected set; }

        public Spells.Spell testSpellOnCounter;

        // Reserved for views communication
        public UIMenu_UIContainer uimenu_uicontainer;

        // Main mob stats panel
        public GameObject mainMobStatPanel;
        public TMPro.TextMeshProUGUI mainMobStatText;

        bool waitingAnimation;
        MobRenderer _statViewMobRenderer;

        // Animation related
        public bool isInAnimation
        {
            get
            {
                //return currentState is WaitAnimState;
                return waitingAnimation;
            }
        }
        public delegate void OnAnimationFinishCallback();
        public OnAnimationFinishCallback OnAnimationFinish;

        private void Awake()
        {
            inputs = new DefaultInputs();
            combatView = FindObjectOfType<miniRAID.UIElements.CombatView>();
        }

        private void OnEnable()
        {
            inputs.UI.Enable();
        }

        private void OnDisable()
        {
            inputs.UI.Disable();
        }

        // Start is called before the first frame update
        void Start()
        {
            Databackend.GetSingleton().testSpell = testSpellOnCounter;

            EnterState();
        }

        // Update is called once per frame
        void Update()
        {
            var inp = inputs.UI.PanCamera.ReadValue<Vector2>();
            mainVCam.transform.Translate(inp * panSpeed * Time.deltaTime);
        }

        /// <summary>
        /// Enters a new UI State.
        /// </summary>
        /// <param name="state">State that wants to enter</param>
        /// <param name="deeper">
        /// If true, then previous state will not be disabled, but "suspended" / "frozen" in stack and can be activated back via BackState();
        /// otherwise previous state will be removed.</param>
        public void EnterState(UIState state, bool deeper = false)
        {
            if(currentState != null)
            {
                currentState.OnStateExit();
            }

            // Start with fresh states
            if (deeper == false)
            {
                while (stateStack.Count > 0)
                {
                    UIState s = stateStack.Pop();
                    s.OnStateDestroyed();
                }
            }

            stateStack.Push(state);
            currentState = stateStack.Peek();
            currentState.OnStateEnter();
            Debug.Log($"Entered {state.stateStr}");
        }

        /// <summary>
        /// Enters (Backs to) the base state: free-view state.
        /// </summary>
        public void EnterState() => EnterState(new FreeView());

        public void BackState()
        {
            // Go back 1 step
            if (stateStack.Count > 0)
            {
                UIState s = stateStack.Pop();
                s.OnStateExit();
                s.OnStateDestroyed();

                currentState = null;
                if(stateStack.Count <= 0) { EnterState(); }

                if (stateStack.Count > 0)
                {
                    currentState = stateStack.Peek();
                    currentState.OnStateEnter();
                }
            }
        }

        public void WaitFor(IEnumerator action, IEnumerator onFinished)
        {
            if (waitingAnimation)
            {
                return;
            }
            
            IEnumerator Wrapper()
            {
                if (currentState is UnitMenu)
                {
                    combatView.menu.ShowMenu();
                    RefreshMainMobStats();
                    mainMobStatPanel.SetActive(true);
                }

                waitingAnimation = false;
                yield return new JumpIn(onFinished);
            }
            
            bool actionAccepted = Globals.combatMgr.Instance.UIPickedAction(action, Wrapper());
            if (actionAccepted)
            {
                // Only change UI state if the action is accepted (there's no actions on-going right now)
                // This could be triggered by keyboard shortcuts while the animation is playing, in some rare cases.
                waitingAnimation = true;
                combatView.menu.HideMenu();
                mainMobStatPanel.SetActive(false);
            }
        }

        public void WaitFor(IEnumerator action, System.Action onFinished)
        {
            IEnumerator Wrapper()
            {
                // Lazy evaluation
                onFinished();
                yield break;
            }

            WaitFor(action, Wrapper());
        }

        public void OnPoint(InputValue input)
        {
            Vector2 cursorPos = cinemachineBrain.ScreenToWorldPoint(input.Get<Vector2>());
            Vector2 _gridPos = (cursorPos / gridWorldUnitSize);

            Vector2Int tmp = cursor.position;
            cursor.position = new Vector2Int(Mathf.FloorToInt(_gridPos.x), Mathf.FloorToInt(_gridPos.y));
            if (cursor.position != tmp && currentState != null)
            {
                currentState.PointAtGrid(cursor.position);
            }
        }

        public bool CheckPassEvent()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return false;
            }
            return true;
        }

        public void OnSubmit(InputValue input)
        {
            if (isInAnimation && !(currentState is TargetRequester.TargetRequesterBase)) return;
            if (currentState == null) return;
            if (CheckPassEvent())
            {
                currentState.Submit(input);
            }
        }

        public void OnNavigate(InputValue input)
        {
            //if (currentState.freeNavigation)
            {
                var inp = input.Get<Vector2>();
                cursor.position += new Vector2Int(
                    inp.x == 0 ? 0 : (inp.x > 0 ? 1 : -1),
                    inp.y == 0 ? 0 : (inp.y > 0 ? 1 : -1));

                cursor.position = 
                    Vector2Int.Max(
                        Vector2Int.Min(
                            cursor.position, 
                            new Vector2Int(
                                Globals.backend.mapWidth, 
                                Globals.backend.mapHeight
                            )
                        ), 
                        Vector2Int.zero
                    );
            }
        }

        public void OnLeftClick(InputValue input)
        {
            OnSubmit(input);
        }

        public void OnCancel(InputValue input)
        {
            if (isInAnimation && !(currentState is TargetRequester.TargetRequesterBase)) return;
            if (currentState == null) return;
            currentState.Cancel(input);
        }

        public void RefreshMainMobStats()
        {
            var mob = _statViewMobRenderer;
            if (mob == null)
            {
                return;
            }

#if UNITY_EDITOR
            Selection.activeGameObject = mob.gameObject;
#endif

            string effects = "";
            foreach (var fx in mob.data.listeners)
            {
                if (fx.type == MobListenerSO.ListenerType.Buff)
                {
                    effects += "\n" + fx.name;
                }
            }

            mainMobStatText.text =
                //$"<mspace=0.58em>" +
                $"<style=\"Title\"><color=#fc0><b><size=12>{mob.gameObject.name}</size></b></color></style>\n" +
                $"\n" +
                $"<size=12><color=#0f0>HP {mob.data.health} / {mob.data.maxHealth}</color></size>\n" +
                $"<size=12><color=#ff0>AP {mob.data.actionPoints} / {mob.data.apMax}</color></size>\n\n" +
                $"{(mob.data.GCDstatus.Contains(GCDGroup.Common) ? "Action done" : "Action available")}" +
                $"\n\n" +
                $"<align=center><style=h3><size=12>===== STATS =====</size></style></align>\n\n" +
                $"VIT {(int)mob.data.baseStats.VIT,3} | STR {(int)mob.data.baseStats.STR,3}\n" +
                $"MAG {(int)mob.data.baseStats.MAG,3} | INT {(int)mob.data.baseStats.INT,3}\n" +
                $"DEX {(int)mob.data.baseStats.DEX,3} | TEC {(int)mob.data.baseStats.TEC,3}\n\n" +
                $"<align=center><style=h3><size=12>===== EQUIP =====</size></style></align>\n\n" +
                $"{mob.data.mainWeapon?.GetInformationString()}\n\n" +
                $"<align=center><style=h3><size=12>=====EFFECTS=====</size></style></align>\n" +
                $"{effects}" +
                //$"</mspace>" +
                $"\n";
        }

        public void ShowMainMobStats(MobRenderer mobRenderer)
        {
            _statViewMobRenderer = mobRenderer;
            RefreshMainMobStats();
            mainMobStatPanel.SetActive(true);
        }

        public void HideMainMobStats()
        {
            mainMobStatPanel.SetActive(false);
        }

        #region Shortcuts

        public void OnActionMove(InputValue input)
        {
            combatView.menu.ShortCut("1");
        }

        public void OnActionPass(InputValue input)
        {
            combatView.menu.ShortCut("R");
        }

        public void OnAction1(InputValue input)
        {
            combatView.menu.ShortCut("1");
        }

        public void OnAction2(InputValue input)
        {
            combatView.menu.ShortCut("2");
        }

        public void OnAction3(InputValue input)
        {
            combatView.menu.ShortCut("3");
        }

        public void OnAction4(InputValue input)
        {
            combatView.menu.ShortCut("4");
        }

        public void OnAction5(InputValue input)
        {
            combatView.menu.ShortCut("5");
        }

        #endregion
    }
}
