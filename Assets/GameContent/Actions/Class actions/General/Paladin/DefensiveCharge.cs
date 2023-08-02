using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using miniRAID;
using miniRAID.Buff;
using miniRAID.Weapon;

namespace GameContent.Buffs.Test
{
    public class DefensiveCharge : PassiveSkillSO
    {
        public float damageReduceRate;
        
        public override MobListener Wrap(MobData parent)
        {
            return new DefensiveChargeRuntimeBuff(parent, this);
        }
    }

    public class DefensiveChargeRuntimeBuff : Buff
    {
        DefensiveCharge dcData => data as DefensiveCharge;
        
        public DefensiveChargeRuntimeBuff(MobData source, DefensiveCharge data) : base(source, data)
        {}

        public override void OnAttach(MobData mob)
        {
            base.OnAttach(mob);

            mob.OnBeforeDamageApplied += MobOnBeforeDamageApplied;
            
            onRemoveFromMob += m =>
            {
                m.OnBeforeDamageApplied -= MobOnBeforeDamageApplied;
            };
        }
        
        private void MobOnBeforeDamageApplied(MobData mob, Consts.DamageHeal_FrontEndInput info, Consts.DamageHeal_ComputedRates rates)
        {
            HeavyWeapon weapon = mob.mainWeapon as HeavyWeapon;
            if (weapon == null)
            {
                return;
            }

            if (weapon.RchargedAttack.isCharging)
            {
                rates.value = Mathf.FloorToInt(rates.value * (1 - dcData.damageReduceRate));
            }
        }
    }
}