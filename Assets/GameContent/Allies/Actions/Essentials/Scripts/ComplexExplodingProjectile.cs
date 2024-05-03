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
        public GridShape explosionShape;
        public UnitFilters explosionTargetFilter;
        
        public SimpleExplosionFx explosionHitFx;
        public SpellBuff explosionBuff;
        public SpellDamageHeal explosionDamageOrHeal;

        public override IEnumerator OnPerform(RuntimeAction<SingleMobTarget> ract, MobData mob, SingleMobTarget target)
        {
            yield return new JumpIn(base.OnPerform(ract, mob, target));
            
            // Capture all targets
            explosionShape.position = target.Target.Position;
            var targetMobs = explosionShape.ApplyTransform()
                .Where(pos => Globals.backend.InMap(pos))
                .Select(pos => Globals.backend.GetMap(pos.x, pos.y, pos.z).mob)
                .Where(targetMob => targetMob != null)
                .Where(targetMob => explosionTargetFilter.Check(mob, targetMob));

            foreach (var targetMob in targetMobs)
            {
                if(explosionHitFx != null)
                    yield return new JumpIn(explosionHitFx.Do(target.Target.Position));
                
                if(explosionDamageOrHeal != null)
                    yield return new JumpIn(explosionDamageOrHeal.Do(ract, mob, targetMob));

                if (explosionBuff != null)
                    yield return new JumpIn(explosionBuff.Do(ract, mob, targetMob));
            }
        }
    }
}
