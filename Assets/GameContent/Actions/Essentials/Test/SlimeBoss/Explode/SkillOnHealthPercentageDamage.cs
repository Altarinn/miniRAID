using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using miniRAID;
using miniRAID.ActionHelpers;
using miniRAID.Buff;
using miniRAID.Spells;

namespace GameContent.Buffs.Test
{
    public class SkillOnHealthPercentageDamage : BuffSO
    {
        public float healthRatio = 0.1f;
        public ActionDataSO skill;

        public override MobListener Wrap(MobData parent)
        {
            return new SkillOnHealthPercentageDamageRuntimeBuff(parent, this);
        }
    }

    public class SkillOnHealthPercentageDamageRuntimeBuff : Buff
    {
        private float damageTotal = 0;
        private float healthRatio;
        
        private ActionDataSO skillData;
        private RuntimeAction runtimeSkill;

        public override string name
        {
            get
            {
                int remaining = Mathf.CeilToInt(parentMob.maxHealth * healthRatio - damageTotal);
                return $"{base.name} [{remaining}]";
            }
        }

        public SkillOnHealthPercentageDamageRuntimeBuff(MobData source, SkillOnHealthPercentageDamage data) : base(source, data)
        {
            healthRatio = data.healthRatio;
            skillData = data.skill;
        }

        public override void OnAttach(MobData mob)
        {
            base.OnAttach(mob);

            damageTotal = 0.0f;
            runtimeSkill = mob.AddAction(skillData);
            
            mob.OnDamageReceived += OnReceiveDamageFinal;
            onRemoveFromMob += m =>
            {
                m.OnDamageReceived -= OnReceiveDamageFinal;
            };
        }

        public IEnumerator OnReceiveDamageFinal(MobData mob, Consts.DamageHeal_Result info)
        {
            damageTotal += info.value;
            if (!mob.isDead)
            {
                if (damageTotal >= (float)mob.maxHealth * healthRatio)
                {
                    damageTotal -= (float)mob.maxHealth * healthRatio;
                    yield return new JumpIn(mob.DoAction(runtimeSkill, new SpellTarget(mob.Position)));
                }
            }
        }
    }
}