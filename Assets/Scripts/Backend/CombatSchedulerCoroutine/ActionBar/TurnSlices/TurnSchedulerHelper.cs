using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace miniRAID.TurnSchedule
{
    public static class TurnSchedulerHelper
    {
        public static IEnumerator DoAutoAttack(IEnumerable<MobData> mobs)
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
        
        public static IEnumerator WakeUpMobs(IEnumerable<MobData> mobs)
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
        
        public static IEnumerator NotifyRecoveryStage(IEnumerable<MobData> mobs)
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