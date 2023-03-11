using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace miniRAID
{
    public class Scheduler : MonoBehaviour
    {
        static Scheduler mInstance;

        public static Scheduler Instance
        {
            get
            {
                return mInstance ? mInstance : (mInstance = (new GameObject("Scheduler")).AddComponent<Scheduler>());
            }
        }

        HashSet<Mob> AwaitForActions = new HashSet<Mob>();
        Consts.UnitGroup currentPhase;

        public int turn;

        // Start is called before the first frame update
        void Start()
        {
            turn = 0;
            //PlayerPhase();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void EnemyPhase()
        {
            // notify UI
            Globals.debugMessage.Instance.Message($"敌方回合");

            EnterPhase(Consts.UnitGroup.Enemy);
        }

        public void PlayerPhase()
        {
            turn += 1;

            // notify UI
            Globals.debugMessage.Instance.Message($"玩家回合");

            EnterPhase(Consts.UnitGroup.Player);
        }

        public void AllyPhase()
        {
            // TODO
        }

        private void EnterPhase(Consts.UnitGroup group)
        {
            AwaitForActions.Clear();
            currentPhase = group;

            // Add them to AwaitForActions first
            foreach (Mob mob in Databackend.GetSingleton().allMobs)
            {
                if (mob.data.unitGroup == group)
                {
                    AwaitForActions.Add(mob);
                }
            }

            // Then trigger all OnNextTurn events
            // AwaitForActions may be modified during following processes
            foreach (var mob in AwaitForActions.ToList())
            {
                mob._OnNextTurn();
            }

            // Finally trigger OnWakeUp events
            // AwaitForActions may be modified during following processes
            foreach (var mob in AwaitForActions.ToList())
            {
                mob.SetActive(true);
            }

            // Refresh UI state
            Globals.ui.Instance.EnterState();

            // Do a refresh in case of empty phase
            Refresh();
        }

        /// <summary>
        /// Check if we should change phase. If so, change the phase.
        /// </summary>
        public void Refresh()
        {
            // Wait until current animation finishes.
            if(Globals.ui.Instance.isInAnimation)
            {
                Globals.ui.Instance.OnAnimationFinish += Refresh;
                return;
            }

            foreach (var mob in AwaitForActions.ToArray())
            {
                if(mob.data.isDead)
                {
                    AwaitForActions.Remove(mob);
                    continue;
                }

                // Still something moveable so No
                if(mob.data.isActive == true)
                {
                    return;
                }
            }

            // Should change phase now
            EndPhase();
        }

        public void EndPhase()
        {
            switch (currentPhase)
            {
                case Consts.UnitGroup.Enemy:
                    PlayerPhase();
                    break;
                case Consts.UnitGroup.Player:
                    EnemyPhase();
                    break;
                default:
                    PlayerPhase();
                    break;
            }
        }
    }
}
