using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using miniRAID;
using miniRAID.ActionHelpers;
using miniRAID.Buff;
using miniRAID.Spells;
using UnityEngine.Animations;

namespace GameContent.Buffs.Test
{
    public class SkillOnHealthPercentageDamage : BuffSO
    {
        public float healthRatio = 0.1f;
        public bool triggerOnDeath = false;
        public ActionSOEntry skill;

        public override MobListener Wrap(MobData parent)
        {
            return new SkillOnHealthPercentageDamageRuntimeBuff(parent, this);
        }
    }

    public class SkillOnHealthPercentageDamageRuntimeBuff : Buff
    {
        private float damageTotal = 0;
        private float healthRatio;
        private bool triggerOnDeath;
        
        private ActionSOEntry skillData;
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
            triggerOnDeath = data.triggerOnDeath;
        }

        public override void OnAttach(MobData mob)
        {
            base.OnAttach(mob);

            damageTotal = 0.0f;
            runtimeSkill = mob.AddAction(skillData);

            if (healthRatio > 0)
            {
                mob.OnDamageReceived += OnReceiveDamageFinal;
                mob.OnHealReceived += OnReceiveHealFinal;
            }
            
            mob.OnRealDeath += OnRealDeath;
            
            onRemoveFromMob += m =>
            {
                if (healthRatio > 0)
                {
                    m.OnDamageReceived -= OnReceiveDamageFinal;
                    m.OnHealReceived -= OnReceiveHealFinal;
                }
                
                m.OnRealDeath -= OnRealDeath;
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

        public IEnumerator OnRealDeath(MobData mob, Consts.DamageHeal_Result info)
        {
            if (triggerOnDeath)
            {
                yield return new JumpIn(mob.DoAction(runtimeSkill, new SpellTarget(mob.Position)));
            }
        }

        public IEnumerator OnReceiveHealFinal(MobData mob, Consts.DamageHeal_Result info)
        {
            damageTotal -= info.value;
            yield break;
        }
    }
}