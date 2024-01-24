using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using miniRAID;
using miniRAID.Buff;

namespace GameContent.Buffs.Test
{
    public class DamageReduction : BuffSO
    {
        public LeveledStats<float> percentage;
        
        public override MobListener Wrap(MobData parent)
        {
            return new DamageReductionRuntimeBuff(parent, this);
        }
    }

    public class DamageReductionRuntimeBuff : Buff
    {
        public DamageReductionRuntimeBuff(MobData source, DamageReduction data) : base(source, data)
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
        
        private IEnumerator MobOnBeforeDamageApplied(MobData mob, Consts.DamageHeal_FrontEndInput info, Consts.DamageHeal_ComputedRates rates)
        {
            // var mana = mob.FindListener<GeneralManaListener>();
            // if (mana == null) return;

            rates.value -= Mathf.CeilToInt(rates.value * ((data as DamageReduction).percentage.Eval(level) * stacks));
            yield break;
        }
    }
}