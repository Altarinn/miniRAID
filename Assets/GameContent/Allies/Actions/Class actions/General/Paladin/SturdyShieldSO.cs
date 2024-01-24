using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using miniRAID;
using miniRAID.Buff;
using Sirenix.Serialization;

namespace GameContent.Buffs.Test
{
    public class SturdyShieldSO : BuffSO
    {
        public LeveledStats<float> shieldRatio;

        [Tooltip("Multiplies with shieldRatio, i.e., shieldRatio = 0.1, thresholdRatio = 2.5 => Damage threshold = 25% of MaxHP")]
        public LeveledStats<float> thresholdRatio;
        
        public override MobListener Wrap(MobData parent)
        {
            return new SturdyShieldRuntimeBuff(parent, this);
        }
    }

    public class SturdyShieldRuntimeBuff : Buff
    {
        public SturdyShieldSO ssData => (SturdyShieldSO)data;
        
        public SturdyShieldRuntimeBuff(MobData source, SturdyShieldSO data) : base(source, data)
        {}

        public override void OnAttach(MobData mob)
        {
            base.OnAttach(mob);

            mob.OnBeforeDamageApplied += MobOnBeforeDamageApplied;
            
            onRemoveFromMob += m =>
            {
                mob.OnBeforeDamageApplied -= MobOnBeforeDamageApplied;
            };
        }

        public IEnumerator MobOnBeforeDamageApplied(MobData mob, Consts.DamageHeal_FrontEndInput info,
            Consts.DamageHeal_ComputedRates rates)
        {
            if (rates.value >= Mathf.CeilToInt(mob.maxHealth * ssData.shieldRatio.Eval(level) * ssData.thresholdRatio.Eval(level)))
            {
                rates.value -= Mathf.CeilToInt(mob.maxHealth * ssData.shieldRatio.Eval(level));
                RemoveStacks(1);
            }
            
            yield break;
        }
    }
}