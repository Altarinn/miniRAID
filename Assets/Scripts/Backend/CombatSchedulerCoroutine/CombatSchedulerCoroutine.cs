using System;
using System.Collections;
using System.Collections.Generic;
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

        private void Awake()
        {
            sc = GetComponent<SerialCoroutine>();
        }

        private void Start()
        {
            sc.StartSerialCoroutine(Combat());
        }

        public IEnumerator Combat()
        {
            // For debug
            //yield return new JumpIn(Test());

            // Preparing
            yield return new JumpIn(Preparation());

            // Before entering Turn 1
            yield return new JumpIn(StartCombat());

            // The real battle begins; Main loop
            while (!IsCombatFinished())
            {
                yield return new JumpIn(StartTurn());

                // Player phase
                yield return UIPlayerPhase();
                yield return new JumpIn(Phase(Consts.UnitGroup.Player));

                // Ally phase
                if (HasAlly())
                {
                    yield return UIAllyPhase();
                }

                // Enemy phase
                yield return UIEnemyPhase();
                yield return new JumpIn(Phase(Consts.UnitGroup.Enemy));
                //yield return new JumpIn(EnemyActions());

                yield return new JumpIn(EndTurn());
            }
        }

        private IEnumerator StartTurn()
        {
            turn++;
            Globals.combatStats.Turns = turn;
            yield break;
        }

        private IEnumerator EndTurn()
        {
            yield break;
        }

        private bool HasAlly()
        {
            return false;
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
