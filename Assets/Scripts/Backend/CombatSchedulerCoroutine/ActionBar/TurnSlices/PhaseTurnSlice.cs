using System.Collections;
using System.Linq;
using UnityEngine;

namespace miniRAID.TurnSchedule
{
    public class PhaseTurnSlice : TurnSlice
    {
        public Consts.UnitGroup group = Consts.UnitGroup.Player;
        public bool UIOnly = false;
        
        public override IEnumerator Turn()
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

            if (!UIOnly)
            {
                foreach (MobData mob in Databackend.GetSingleton().allMobs)
                {
                    mob.OnNewPhase();
                }
            
                yield return new JumpIn(TurnSchedulerHelper.WakeUpMobs(
                    Globals.backend.allMobs.Where(x => x.unitGroup == group).ToHashSet()));
            }

            yield break;
        }
    }
}