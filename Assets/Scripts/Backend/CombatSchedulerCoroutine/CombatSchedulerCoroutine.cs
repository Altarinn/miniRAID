using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using miniRAID.TurnSchedule;
using UnityEngine;
using XLua;

using Sirenix.OdinInspector;

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
        public int turn;
        public Databackend backend;

        SerialCoroutine sc;
        private TurnSlice currentTurnSlice;

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

            // The real battle begins; Main loop
            while (!IsCombatFinished())
            {
                UpdateSchedulerUI();
                
                currentTurnSlice = turnSchedule.Dequeue();
                yield return new JumpIn(currentTurnSlice.Turn());
                yield return new JumpIn(Chill());

                KeepTurnScheduleLength();

                // yield return new JumpIn(StartTurn());
                //
                // // Wait a moment (2 frames) to wait everything loading-up
                // yield return null;
                // yield return null;
                //
                // // Player phase
                // yield return UIPlayerPhase();
                //
                // yield return new JumpIn(Phase(Consts.UnitGroup.Player));
                //
                // for (int i = 1; i <= 4; i++)
                // {
                //     yield return new JumpIn(Turn(Consts.UnitGroup.Player, $"{i}/4"));
                //     yield return new JumpIn(Chill());
                //     if (ShouldSkipPlayerPhase())
                //     {
                //         break;
                //     }
                // }
                //
                // // Refresh & auto-attack
                // yield return new JumpIn(AutoAttackStage(Consts.UnitGroup.Player));
                // yield return new JumpIn(Chill());
                //
                // // Ally phase
                // if (HasAlly())
                // {
                //     yield return UIAllyPhase();
                //     yield return new JumpIn(Chill());
                // }
                //
                // // Enemy phase
                // yield return UIEnemyPhase();
                // yield return new JumpIn(Phase(Consts.UnitGroup.Enemy));
                // yield return new JumpIn(Turn(Consts.UnitGroup.Enemy));
                // yield return new JumpIn(Chill());
                // //yield return new JumpIn(EnemyActions());
                //
                // // TODO: Extra turns / Heavy weapon
                // yield return new JumpIn(RecoveryStage(Consts.UnitGroup.Player));
                // yield return new JumpIn(Chill());
                //
                // yield return new JumpIn(EndTurn());
                // yield return new JumpIn(Chill());
            }
        }

        // TODO: Move me to another place specific for UI
        public void UpdateSchedulerUI()
        {
            int length = 8;

            string message = String.Join("\n",
                turnSchedule
                    .Where(x => x.showInUI)
                    .Take(length)
                    .Select(x =>
                        $"<color=#{ColorUtility.ToHtmlStringRGB(x.mainColor)}> {x.label} </color>")
                    .ToArray());
            
            Globals.ui.Instance.combatView.schedulerPlaceholder.text = message;
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
            turn = 0;
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
