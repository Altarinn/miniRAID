using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace miniRAID
{
	public partial class CombatSchedulerCoroutine : MonoBehaviour
    {
        HashSet<MobData> awaitForActions = new HashSet<MobData>();
        public Consts.UnitGroup currentPhase { get; private set; }

        public IEnumerator Phase(Consts.UnitGroup group)
        {
            Globals.logger?.Log($"[csc] PHASE START: {group.ToString()}");

            if (group == Consts.UnitGroup.Player)
            {
                Globals.ui.Instance.combatView.ShowCenterTitle("Player Phase");
            }
            else if (group == Consts.UnitGroup.Ally)
            {
                Globals.ui.Instance.combatView.ShowCenterTitle("Ally Phase");
            }
            else if (group == Consts.UnitGroup.Enemy)
            {
                Globals.ui.Instance.combatView.ShowCenterTitle("Enemy Phase");
            }
            else
            {
                Globals.ui.Instance.combatView.ShowCenterTitle("Phase");
            }
            
            // Wait some time
            yield return new WaitForSeconds(1f);
            
            Globals.ui.Instance.combatView.HideCenterTitle();
            
            // Enter phase
            awaitForActions.Clear();
            currentPhase = group;

            // Add them to AwaitForActions first
            foreach (MobData mob in Databackend.GetSingleton().allMobs)
            {
                mob.OnNewPhase();
                if (mob.unitGroup == group)
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
            while (awaitForActions.Any(m => m.isControllable))
            {
                yield return null;
            }

            // End phase. Do something?
        }

        public bool CheckPhaseEnd(IEnumerable<MobData> mobs)
        {
            return !mobs.Any(m => m.isControllable);
        }

        public IEnumerator WakeUpMobs(IEnumerable<MobData> mobs)
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
            
            // Finally trigger Agent events
            // AwaitForActions may be modified during following processes
            foreach (var mob in mobs.ToList())
            {
                yield return new JumpIn(mob.OnAgentTurn());
            }
        }
    }
}
