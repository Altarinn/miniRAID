using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace miniRAID.TurnSchedule
{
    public class AutoAttackTurnSliceSO : TurnSliceSO
    {
        HashSet<MobData> awaitForActions = new HashSet<MobData>();
        public Consts.UnitGroup group = Consts.UnitGroup.Player;
        
        public override IEnumerator Turn(CombatSchedulerCoroutine coroutine)
        {
            Globals.ui.Instance.combatView.ShowCenterTitle("Auto Attack - Strategy Phase");
            yield return new WaitForSeconds(0.5f);
            Globals.ui.Instance.combatView.HideCenterTitle();

            // Enter phase
            awaitForActions = Globals.backend.allMobs.Where(x => x.unitGroup == group).ToHashSet();
            foreach (var mob in awaitForActions)
            {
                yield return new JumpIn(mob.EnterStrategyPhase());
            }
            
            // Wait for UI confirmation
            yield return new JumpIn(coroutine.UIWaitPlayerInput());

            // Perform auto-attack
            yield return new JumpIn(TurnSchedulerHelper.DoAutoAttack(awaitForActions));
        }
    }
}