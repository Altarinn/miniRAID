using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using miniRAID;
using miniRAID.Buff;

namespace GameContent.Buffs.Test
{
    public class ManaShield : BuffSO
    {
        public override MobListener Wrap(MobData parent)
        {
            return new ManaShieldRuntimeBuff(parent, this);
        }
    }

    public class ManaShieldRuntimeBuff : Buff
    {
        public ManaShieldRuntimeBuff(MobData source, ManaShield data) : base(source, data)
        {}

        public override void OnAttach(MobData mob)
        {
            base.OnAttach(mob);
            mob.OnBeforeDamageApplied += MobOnBeforeDamageApplied;
            
            onRemoveFromMob += m =>
            {
                m.OnBeforeDamageApplied -= MobOnBeforeDamageApplied;
            };
            
            /* Custom events:
            mob.OnXXXX += XXXX;
            onRemoveFromMob += m =>
            {
                m.OnXXXX -= XXXX;
            };
            */
        }
        
        private void MobOnBeforeDamageApplied(MobData mob, Consts.DamageHeal_FrontEndInput info, Consts.DamageHeal_ComputedRates rates)
        {
            var mana = mob.FindListener<GeneralManaListener>();
            if (mana == null) return;
            
            rates.value -= mana.UseMana(rates.value);
        }
    }
}