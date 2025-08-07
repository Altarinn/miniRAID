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
    public class ExplodingProjectile : ActionDataSO<SingleMobTarget>
    {
        public IGridCollider explodeShape;
        public UnitFilters targetFilter;
        
        public ActionHelpers.Projectile projectile;
        public SimpleExplosionFx fxOnExplode;
        public SimpleExplosionFx fxOnHit;
        public SpellBuff buff;
        public SpellDamageHeal damageOrHeal;

        public override IEnumerator OnPerform(RuntimeAction<SingleMobTarget> ract, MobData mob, SingleMobTarget target)
        {
            Debug.Log($"Current context: {Globals.cc.animation}");
            
            if(projectile != null)
                yield return new JumpIn(projectile.WaitForShootAt(mob, target.Target.Position));
            
            if(fxOnExplode != null)
                yield return new JumpIn(fxOnExplode.Do(target.Target.Position));
            
            // Capture all targets
            explodeShape.Position = target.Target.Position;
            var targetMobs =
                CaptureTargetsInCollider.CaptureAllTargetsWithinRange(mob, targetFilter, explodeShape);

            yield return new JumpIn(
                MobListHelpers.WaitForAllMobs(
                    targetMobs, m => JumpInHelper.Chain(
                        fxOnHit?.Do(target.Target.Position),
                        damageOrHeal?.Do(ract, mob, m),
                        buff?.Do(ract, mob, m)
                    )
                )
            );
        }
    }
}
