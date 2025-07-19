using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using miniRAID.TurnSchedule;
using UnityEngine;

using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace miniRAID
{
    [RequireComponent(typeof(SerialCoroutine))]
    public partial class CombatSchedulerCoroutine : MonoBehaviour
    {
        static CombatSchedulerCoroutine mInstance;

        public static CombatSchedulerCoroutine Instance
        {
            get
            {
                return mInstance ? mInstance : (mInstance = (new GameObject("CombatSchedulerCoroutine")).AddComponent<CombatSchedulerCoroutine>());
            }
        }


        [Title("Combat")] 
        public Timestamp now, appendedTurns;

        SerialCoroutine sc;
        [OdinSerialize] private TurnSlice currentTurnSlice;

        [NonSerialized] public Action OnBeforeNextTurnSlice;

        private void Awake()
        {
            sc = GetComponent<SerialCoroutine>();
        }

        private void Start()
        {
            // This starts the whole combat SerialCoroutine.
            // Changes on default context should not be made here as this will last for entire combat.
            sc.StartSerialCoroutine(Combat(), new SerialCoroutineContext()
            {
                animation = true,
                rng = new RNG((uint)(DateTime.Now.GetHashCode() - int.MinValue))
            });
        }

        public void OnNextSnapshot(Action act)
        {
            if (OnBeforeNextTurnSlice != null)
            {
                Debug.LogError("CombatSchedulerCoroutine is already full of OnBeforeNextTurnSlice! Action Ignored!");
                return;
            }
            OnBeforeNextTurnSlice = act;
        }
        
        public IEnumerator Combat()
        {
            // TODO: Move me to the game's beginning scene
            yield return new JumpIn(Globals.localizer.Initialization());
            
            // For debug
            //yield return new JumpIn(Test());

            // Preparing
            yield return new JumpIn(Preparation());

            // Before entering Turn 1
            yield return new JumpIn(StartCombat());
            
            // TODO: FIXME: Remove me from here!
            FindObjectOfType<BGMLoopWithIntro>().Play();

            // Battle main loop
            while (!IsCombatFinished())
            {
                // Save-Load happens only here, perhaps
                if (turnSchedule.First.Value != null &&
                    turnSchedule.First.Value.data.GetType() == typeof(CommonPlayerTurnSliceSO))
                {
                    SaveDataSerializer.saveSlotBackup = SaveDataSerializer.SerializeEverything();
                }
                if (OnBeforeNextTurnSlice != null)
                {
                    OnBeforeNextTurnSlice.Invoke();
                    OnBeforeNextTurnSlice = null;
                }
                
                UpdateSchedulerUI();
                
                currentTurnSlice = turnSchedule.Dequeue();
                yield return new JumpIn(currentTurnSlice.Turn());
                // yield return new JumpIn(Chill());
                
                currentTurnSlice.OnRemove(this);

                KeepTurnScheduleLength();
            }
        }

        // TODO: Move me to another place specific for UI
        public void UpdateSchedulerUI()
        {
            int length = 12;

            string message = String.Join("\n",
                turnSchedule
                    .Where(x => x.ShowInUI)
                    // .Skip(1)
                    .Take(length)
                    .Select(x =>
                        $"<color=#{ColorUtility.ToHtmlStringRGB(x.MainColor)}> {x.Label} </color>")
                    .ToArray());
            
            Globals.ui.Instance.combatView.schedulerPlaceholder.text = message;

            var t = turnSchedule
                .First(x => x.ShowInUI);

            Globals.ui.Instance.combatView.currentTurnPlaceholder.text = $"<color=#{ColorUtility.ToHtmlStringRGB(t.MainColor)}> {t.Label} </color>";
        }

        private IEnumerator Chill()
        {
            yield return new WaitForSeconds(0.65f);
        }

        private bool HasAlly()
        {
            return false;
        }

        public bool ShouldSkipPlayerPhase()
        {
            // TODO: Controllable allies?
            var mobs = Globals.backend.allMobs.Where(x => x.unitGroup == Consts.UnitGroup.Player);
            return !mobs.Any(m => m.isControllable);
        }

        private bool IsCombatFinished()
        {
            return false;
        }

        private IEnumerator Preparation()
        {
            // TODO
            yield break;
        }

        private IEnumerator StartCombat()
        {
            InitializeTurnSchedule();
            
            // TODO
            now.currentTurnID = 0;
            now.currentTurnSliceID = 0;
            
            yield break;
        }

        // Update is called once per frame
        void Update()
        {
        }

        #region test

        private IEnumerator Test()
        {
            // Test
            Debug.Log("Test1");
            for (int i = 0; i < 1000; i++) { var a = Test2(); yield return new JumpIn(a); }
            Debug.Log("Test2");
            yield return new WaitForSeconds(2.0f);
            Debug.Log("Test3");
            yield return Test3();
            Debug.Log("Test4");
        }

        private IEnumerator Test2()
        {
            if (false)
            {
                yield return null;
            }
        }

        private IEnumerator Test3()
        {
            // Test
            Debug.Log("Test3.1");
            yield return new WaitForSeconds(2.0f);
            Debug.Log("Test3.2");
        }

        #endregion
    }
}
