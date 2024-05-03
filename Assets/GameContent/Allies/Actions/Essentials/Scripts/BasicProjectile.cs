using System.Collections;
using System.Collections.Generic;
using miniRAID.ActionHelpers;
using miniRAID.Spells;
using UnityEngine;

namespace miniRAID.Actions
{
    public class BasicProjectile : ActionDataSO<SingleMobTarget>
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

        public override IEnumerator OnPerform(RuntimeAction<SingleMobTarget> ract, MobData mob, SingleMobTarget target)
        {
            MobData dst = target.Target;
            
            Debug.Log($"Current context: {Globals.cc.animation}");
            
            if(projectile != null)
                yield return new JumpIn(projectile.WaitForShootAt(mob, dst.Position));
            
            if(fxOnHit != null)
                yield return new JumpIn(fxOnHit.Do(dst.Position));
            
            if(buff != null && buffAppliedFirst)
                yield return new JumpIn(buff.Do(ract, mob, dst));
                
            if(damageOrHeal != null)
                yield return new JumpIn(damageOrHeal.Do(ract, mob, dst));

            if (buff != null && !buffAppliedFirst)
                yield return new JumpIn(buff.Do(ract, mob, dst));
        }
    }
}