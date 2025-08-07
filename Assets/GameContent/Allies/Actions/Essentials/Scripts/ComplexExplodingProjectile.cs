using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;
using System.Linq;
using miniRAID.ActionHelpers;
using miniRAID.Spells;
using Sirenix.Utilities;

namespace miniRAID.Actions
{
    public class ComplexExplodingProjectile : BasicProjectile
    {
        public IGridCollider explosionShape;
        public UnitFilters explosionTargetFilter;
        
        public SimpleExplosionFx explosionHitFx;
        public SpellBuff explosionBuff;
        public SpellDamageHeal explosionDamageOrHeal;

        public override IEnumerator OnPerform(RuntimeAction<SingleMobTarget> ract, MobData mob, SingleMobTarget target)
        {
            yield return new JumpIn(base.OnPerform(ract, mob, target));
            
            // Capture all targets
            explosionShape.Position = target.Target.Position;
            var targetMobs =
                CaptureTargetsInCollider.CaptureAllTargetsWithinRange(mob, explosionTargetFilter, explosionShape);

            yield return new JumpIn(
                MobListHelpers.WaitForAllMobs(
                    targetMobs, m => JumpInHelper.Chain(
                        explosionHitFx?.Do(m.Position),
                        explosionDamageOrHeal?.Do(ract, mob, m),
                        explosionBuff?.Do(ract, mob, m)
                    )
                )
            );
        }
    }
}
