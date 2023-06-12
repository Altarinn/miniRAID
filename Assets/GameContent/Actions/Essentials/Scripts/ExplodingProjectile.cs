using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;
using System.Linq;
using miniRAID.ActionHelpers;
using miniRAID.Spells;
using Sirenix.Utilities;

namespace miniRAID
{
    public class ExplodingProjectile : ActionDataSO
    {
        public GridShape explodeShape;
        public UnitFilters targetFilter;
        
        public ActionHelpers.Projectile projectile;
        public SimpleExplosionFx fxOnExplode;
        public SimpleExplosionFx fxOnHit;
        public SpellBuff buff;
        public SpellDamageHeal damageOrHeal;

        public override IEnumerator OnPerform(RuntimeAction ract, MobData mob, SpellTarget targets)
        {
            Vector3Int target = targets.targetPos[0];
            
            Debug.Log($"Current context: {Globals.cc.animation}");
            
            if(projectile != null)
                yield return new JumpIn(projectile.WaitForShootAt(mob, target));
            
            if(fxOnExplode != null)
                yield return new JumpIn(fxOnExplode.Do(target));
            
            // Capture all targets
            explodeShape.position = target;
            var targetMobs = explodeShape.ApplyTransform()
                .Where(pos => Globals.backend.InMap(pos))
                .Select(pos => Globals.backend.GetMap(pos.x, pos.y, pos.z).mob)
                .Where(targetMob => targetMob != null)
                .Where(targetMob => targetFilter.Check(mob, targetMob));

            foreach (var targetMob in targetMobs)
            {
                if(fxOnHit != null)
                    yield return new JumpIn(fxOnHit.Do(target));
                
                if(damageOrHeal != null)
                    yield return new JumpIn(damageOrHeal.Do(ract, mob, targetMob));

                if (buff != null)
                    yield return new JumpIn(buff.Do(ract, mob, targetMob));
            }
        }
    }
}
