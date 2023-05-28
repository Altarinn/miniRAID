using System.Collections;
using miniRAID.ActionHelpers;
using miniRAID.Spells;
using Unity.VisualScripting;
using UnityEngine;

namespace miniRAID.Actions
{
    public class BasicProjectile : ActionDataSO
    {
        public ActionHelpers.Projectile projectile;
        public PlayFx fxOnHit;
        public SpellBuff buff;
        public SpellDamageHeal damageOrHeal;
        
        public override IEnumerator OnPerform(RuntimeAction ract, MobData mob, SpellTarget targets)
        {
            Vector2Int target = targets.targetPos[0];
            MobData dst = Essentials.MobAtGrid(target);
            
            Debug.Log($"Current context: {Globals.cc.animation}");
            
            if(projectile != null)
                yield return new JumpIn(projectile.WaitForShootAt(mob, target));
            
            if(fxOnHit != null)
                yield return new JumpIn(fxOnHit.Do(target));
            
            if(damageOrHeal != null)
                yield return new JumpIn(damageOrHeal.Do(ract, mob, dst));

            if (buff != null)
                yield return new JumpIn(buff.Do(ract, mob, dst));
        }
    }
}