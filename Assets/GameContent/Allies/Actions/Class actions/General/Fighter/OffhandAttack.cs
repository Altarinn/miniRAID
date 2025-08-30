using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;
using System.Linq;
using miniRAID.Spells;

namespace miniRAID
{
    public class OffhandAttack : ActionDataSO<SingleMobTarget>
    {
        public override IEnumerator OnPerform(RuntimeAction<SingleMobTarget> ract, MobData mob,
            SingleMobTarget target)
        {
            var offhandRAtk = mob.subWeapon?.GetRegularAttackSpell();
            
            if (offhandRAtk is {Valid: true})
            {
                yield return new JumpIn(mob.DoAction(offhandRAtk, target, null));
            }
        }

        public override void RecalculateStats(RuntimeAction ract, MobData mob)
        {
            base.RecalculateStats(ract, mob);
            
            var offhandRAtk = mob.subWeapon?.GetRegularAttackSpell();
            ract.Valid = offhandRAtk is { Valid: true };
        }
    }
}
