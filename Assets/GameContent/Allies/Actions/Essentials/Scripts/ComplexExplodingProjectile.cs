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

        public override IEnumerator OnPerform(RuntimeAction ract, MobData mob, SpellTarget targets)
        {
            yield return new JumpIn(base.OnPerform(ract, mob, targets));
            
            Vector3Int target = targets.targetPos[0];
            
            // Capture all targets
            explosionShape.position = target;
            var targetMobs = explosionShape.ApplyTransform()
                .Where(pos => Globals.backend.InMap(pos))
                .Select(pos => Globals.backend.GetMap(pos.x, pos.y, pos.z).mob)
                .Where(targetMob => targetMob != null)
                .Where(targetMob => explosionTargetFilter.Check(mob, targetMob));

            foreach (var targetMob in targetMobs)
            {
                if(explosionHitFx != null)
                    yield return new JumpIn(explosionHitFx.Do(target));
                
                if(explosionDamageOrHeal != null)
                    yield return new JumpIn(explosionDamageOrHeal.Do(ract, mob, targetMob));

                if (explosionBuff != null)
                    yield return new JumpIn(explosionBuff.Do(ract, mob, targetMob));
            }
        }
    }
}
