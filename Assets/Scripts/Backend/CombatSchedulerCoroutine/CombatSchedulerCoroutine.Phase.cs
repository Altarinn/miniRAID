using System;
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

        protected bool playerPhaseEnd = false, turnEnd = false;

        // TODO: Rename to Recovery
        public IEnumerator Phase(Consts.UnitGroup group)
        {
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
            
            foreach (MobData mob in Databackend.GetSingleton().allMobs)
            {
                mob.OnNewPhase();
            }
            
            yield return new JumpIn(WakeUpMobs(
                Globals.backend.allMobs.Where(x => x.unitGroup == group).ToHashSet()));

            yield break;
        }

        public IEnumerator AutoAttackStage(Consts.UnitGroup group = Consts.UnitGroup.Player)
        {
            Globals.ui.Instance.combatView.ShowCenterTitle("Auto Attack");
            yield return new WaitForSeconds(0.5f);
            Globals.ui.Instance.combatView.HideCenterTitle();
            
            // Enter phase
            awaitForActions = Globals.backend.allMobs.Where(x => x.unitGroup == group).ToHashSet();

            yield return new JumpIn(DoAutoAttack(awaitForActions));
        }
        
        public IEnumerator RecoveryStage(Consts.UnitGroup group = Consts.UnitGroup.Player)
        {
            Globals.ui.Instance.combatView.ShowCenterTitle("Recovery");
            yield return new WaitForSeconds(0.5f);
            Globals.ui.Instance.combatView.HideCenterTitle();
            
            // Enter phase
            awaitForActions = Globals.backend.allMobs.Where(x => x.unitGroup == group).ToHashSet();

            yield return new JumpIn(WakeUpMobs(awaitForActions));
            yield return new JumpIn(NotifyRecoveryStage(Globals.backend.allMobs));
        }

        public IEnumerator Turn(Consts.UnitGroup group, string message = null)
        {
            Globals.logger?.Log($"[csc] TURN START: {group.ToString()}");

            // Enter phase
            awaitForActions.Clear();
            currentPhase = group;

            // Add them to AwaitForActions first
            foreach (MobData mob in Databackend.GetSingleton().allMobs)
            {
                // mob.OnNewPhase();
                if (mob.unitGroup == group)
                {
                    awaitForActions.Add(mob);
                }
            }
            
            if (message != null)
            {
                Globals.ui.Instance.combatView.ShowCenterTitle(message);
                Globals.ui.Instance.combatView.schedulerPlaceholder.text = message;
                yield return new WaitForSeconds(0.5f);
                Globals.ui.Instance.combatView.HideCenterTitle();
            }
            
            yield return new WaitForSeconds(0.3f);

            // yield return new JumpIn(NotifyNewTurn(awaitForActions));

            // Player can control
            if(group == Consts.UnitGroup.Player)
            {
                yield return new JumpIn(UIWaitPlayerInput());
                yield return new JumpIn(NotifyNewTurn(Databackend.GetSingleton().allMobs));
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
            // while (awaitForActions.Any(m => m.isControllable))
            while(turnEnd == false)
            {
                yield return null;
            }

            // End phase. Do something?
        }

        [Obsolete]
        public bool CheckPhaseEnd(IEnumerable<MobData> mobs)
        {
            throw new System.NotImplementedException();
            // return !mobs.Any(m => m.isControllable);
            // return true;
        }

        public void MarkPlayerTurnEnd()
        {
            playerPhaseEnd = true;
            MarkTurnEnd();
        }

        public void MarkTurnEnd()
        {
            turnEnd = true;
        }
        
        public IEnumerator DoAutoAttack(IEnumerable<MobData> mobs)
        {
            // var mobs = Globals.backend.allMobs
            //     .Where(x => x.unitGroup == group)
            //     .OrderByDescending(x => x.DEX)
            //     .ToList();
            mobs = mobs
                .OrderByDescending(x => x.AGI)
                .ToList();
            
            foreach (MobData mob in mobs)
            {
                if (mob.canAutoAttack)
                {
                    yield return new JumpIn(mob.AutoAttack());
                }
            }
        }

        public IEnumerator WakeUpMobs(IEnumerable<MobData> mobs)
        {
            // Then trigger OnWakeUp events
            // AwaitForActions may be modified during following processes
            foreach (var mob in mobs.ToList())
            {
                yield return new JumpIn(mob.SetActive(true));
                mob.ResetSkipNextAutoAttack();
            }
            
            // Finally trigger Agent events
            // AwaitForActions may be modified during following processes
            foreach (var mob in mobs.ToList())
            {
                yield return new JumpIn(mob.OnAgentTurn());
            }
        }
        
        public IEnumerator NotifyNewTurn(IEnumerable<MobData> mobs)
        {
            // Trigger all OnNextTurn events
            // AwaitForActions may be modified during following processes
            foreach (var mob in mobs.ToList())
            {
                yield return new JumpIn(mob._OnNextTurn());
            }
        }
        
        public IEnumerator NotifyRecoveryStage(IEnumerable<MobData> mobs)
        {
            // Trigger all OnNextTurn events
            // AwaitForActions may be modified during following processes
            foreach (var mob in mobs.ToList())
            {
                yield return new JumpIn(mob._OnRecoveryStage());
            }
        }
    }
}
