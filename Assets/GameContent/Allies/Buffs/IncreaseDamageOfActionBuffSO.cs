using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using miniRAID;
using miniRAID.Buff;

namespace GameContent.Buffs.Test
{
    public class IncreaseDamageOfActionBuffSO : BuffSO
    {
        public ActionDataSO targetAction;
        public float rate;
        
        public override MobListener Wrap(MobData parent)
        {
            return new IncreaseDamageOfActionBuffSORuntimeBuff(parent, this);
        }
    }

    public class IncreaseDamageOfActionBuffSORuntimeBuff : Buff
    {
        public IncreaseDamageOfActionBuffSORuntimeBuff(MobData source, IncreaseDamageOfActionBuffSO data) : base(source, data)
        {}

        public override void OnAttach(MobData mob)
        {
            base.OnAttach(mob);

            mob.OnBeforeDamageApplied += MobBeforeDamageHealApplied;
            onRemoveFromMob += m =>
            {
                m.OnBeforeDamageApplied -= MobBeforeDamageHealApplied;
            };
        }

        private IEnumerator MobBeforeDamageHealApplied(MobData mob, Consts.DamageHeal_FrontEndInput input, Consts.DamageHeal_ComputedRates rates)
        {
            if (input.sourceAction.data == ((IncreaseDamageOfActionBuffSO)data).targetAction)
            {
                rates.value = Mathf.RoundToInt((float)rates.value * ((IncreaseDamageOfActionBuffSO)data).rate);
            }

            yield break;
        }
    }
}