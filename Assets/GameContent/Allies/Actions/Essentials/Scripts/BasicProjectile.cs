using System.Collections;
using System.Collections.Generic;
using miniRAID.ActionHelpers;
using miniRAID.Spells;
using UnityEngine;

namespace miniRAID.Actions
{
    public class BasicProjectile : ActionDataSO
    {
        public ActionHelpers.Projectile projectile;
        public SimpleExplosionFx fxOnHit;
        public bool buffAppliedFirst = false;
        public SpellBuff buff;
        public SpellDamageHeal damageOrHeal;

        public override Dictionary<string, object> LazyPrepareTooltipVariables(RuntimeAction ract)
        {
            var result = base.LazyPrepareTooltipVariables(ract);
            result.Add("HitPower", damageOrHeal?.GetPower(ract));
            result.Add("BuffPower", buff?.GetPower(ract));

            return result;
        }

        public override IEnumerator OnPerform(RuntimeAction ract, MobData mob, SpellTarget targets)
        {
            Vector3Int target = targets.targetPos[0];
            MobData dst = Essentials.MobAtGrid(target);
            
            Debug.Log($"Current context: {Globals.cc.animation}");
            
            if(projectile != null)
                yield return new JumpIn(projectile.WaitForShootAt(mob, target));
            
            if(fxOnHit != null)
                yield return new JumpIn(fxOnHit.Do(target));
            
            if(buff != null && buffAppliedFirst)
                yield return new JumpIn(buff.Do(ract, mob, dst));
                
            if(damageOrHeal != null)
                yield return new JumpIn(damageOrHeal.Do(ract, mob, dst));

            if (buff != null && !buffAppliedFirst)
                yield return new JumpIn(buff.Do(ract, mob, dst));
        }
    }
}