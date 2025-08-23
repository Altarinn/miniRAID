using System.Collections;
using System.Collections.Generic;
using System.Linq;
using miniRAID.ActionHelpers;
using miniRAID.Spells;

namespace miniRAID.Agents.SlimeBoss.MeleeAttackSet
{
    public class SplashAttack : ActionDataSO<SingleMobTarget>
    {
        public UnitFilters filters;
        public SpellDamageHeal damage;
        
        public GridCollider range;

        public override GridCollider MainShape => range;

        public override IEnumerator OnPerform(RuntimeAction<SingleMobTarget> ract, MobData mob, SingleMobTarget target)
        {
            List<MobData> captured;
            range.Position = target.Target.Position;
            captured = CaptureTargetsInCollider.CaptureAllTargetsWithinRange(
                    mob, filters, range, null)
                .ToList();

            yield return new JumpIn(
                MobListHelpers.WaitForAllMobs(captured, m => damage.Do(ract, mob, m))
            );
        }
    }
}