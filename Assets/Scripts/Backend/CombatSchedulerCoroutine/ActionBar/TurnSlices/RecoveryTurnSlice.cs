using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace miniRAID.TurnSchedule
{
    public class RecoveryTurnSlice : TurnSlice
    {
        HashSet<MobData> awaitForActions = new HashSet<MobData>();
        public Consts.UnitGroup group = Consts.UnitGroup.Player;
        
        public override IEnumerator Turn()
        {
            Globals.ui.Instance.combatView.ShowCenterTitle("Recovery");
            yield return new WaitForSeconds(0.5f);
            Globals.ui.Instance.combatView.HideCenterTitle();
            
            // Enter phase
            awaitForActions = Globals.backend.allMobs.Where(x => x.unitGroup == group).ToHashSet();

            yield return new JumpIn(TurnSchedulerHelper.WakeUpMobs(awaitForActions));
            yield return new JumpIn(TurnSchedulerHelper.NotifyRecoveryStage(Globals.backend.allMobs));
        }
    }
}