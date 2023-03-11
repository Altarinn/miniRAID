using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace miniRAID
{
	public partial class CombatSchedulerCoroutine : MonoBehaviour
    {
        HashSet<Mob> awaitForActions = new HashSet<Mob>();
        Consts.UnitGroup currentPhase;

        public IEnumerator Phase(Consts.UnitGroup group)
        {
            // Enter phase
            awaitForActions.Clear();
            currentPhase = group;

            // Add them to AwaitForActions first
            foreach (Mob mob in Databackend.GetSingleton().allMobs)
            {
                if (mob.data.unitGroup == group)
                {
                    awaitForActions.Add(mob);
                }
            }

            yield return new JumpIn(WakeUpMobs(awaitForActions));

            // Player can control
            if(group == Consts.UnitGroup.Player)
            {
                yield return new JumpIn(UIWaitPlayerInput());
            }
            else if(group == Consts.UnitGroup.Ally || group == Consts.UnitGroup.Enemy)
            {
                // TODO: Priorities?
                //foreach (Mob mob in awaitForActions)
                //{
                //    yield return new JumpIn(mob)
                //}
            }
            else
            {
                
            }

            // TODO: Do something to end the phase!
            while (awaitForActions.Any(m => m.isActive))
            {
                yield return null;
            }

            // End phase. Do something?
        }

        public bool CheckPhaseEnd(IEnumerable<Mob> mobs)
        {
            return !mobs.Any(m => m.isActive);
        }

        public IEnumerator WakeUpMobs(IEnumerable<Mob> mobs)
        {
            // Trigger all OnNextTurn events
            // AwaitForActions may be modified during following processes
            foreach (var mob in mobs.ToList())
            {
                yield return new JumpIn(mob._OnNextTurn());
            }

            // Then trigger OnWakeUp events
            // AwaitForActions may be modified during following processes
            foreach (var mob in mobs.ToList())
            {
                yield return new JumpIn(mob.SetActive(true));
            }
        }
    }
}
