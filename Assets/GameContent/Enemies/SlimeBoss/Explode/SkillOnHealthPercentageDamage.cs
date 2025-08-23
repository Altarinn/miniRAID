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
        [SerializeField] private float damageTotal = 0;
        private float healthRatio => ((SkillOnHealthPercentageDamage)data).healthRatio;
        private bool triggerOnDeath => ((SkillOnHealthPercentageDamage)data).triggerOnDeath;
        
        private ActionSOEntry skillData => ((SkillOnHealthPercentageDamage)data).skill;
        [SerializeField] private RuntimeAction<SingleMobTarget> runtimeSkill;

        public override string name
        {
            get
            {
                int remaining = Mathf.CeilToInt(parentMob.maxHealth * healthRatio - damageTotal);
                return $"{base.name} [{remaining}]";
            }
        }

        public SkillOnHealthPercentageDamageRuntimeBuff(MobData source, SkillOnHealthPercentageDamage data) : base(source, data)
        { }

        public override void OnAttach(MobData mob)
        {
            base.OnAttach(mob);

            damageTotal = 0.0f;
            runtimeSkill = (RuntimeAction<SingleMobTarget>)mob.AddAction(skillData);

            if (healthRatio > 0)
            {
                mob.OnDamageReceived.AddListener(OnReceiveDamageFinal);
                mob.OnHealReceived.AddListener(OnReceiveHealFinal);
            }
            
            mob.OnRealDeath.AddListener(OnRealDeath);
        }

        protected override void OnRemoveFromMob(MobData mob)
        {
            if (healthRatio > 0)
            {
                mob.OnDamageReceived.RemoveListener(OnReceiveDamageFinal);
                mob.OnHealReceived.RemoveListener(OnReceiveHealFinal);
            }
            
            mob.OnRealDeath.RemoveListener(OnRealDeath);
            base.OnRemoveFromMob(mob);
        }

        public IEnumerator OnReceiveDamageFinal(MobData mob, Consts.DamageHeal_Result info)
        {
            damageTotal += info.value;
            if (!mob.isDead)
            {
                if (damageTotal >= (float)mob.maxHealth * healthRatio)
                {
                    damageTotal -= (float)mob.maxHealth * healthRatio;
                    yield return new JumpIn(mob.DoAction(runtimeSkill, new SingleMobTarget(mob, mob.GridPosition)));
                }
            }
        }

        public IEnumerator OnRealDeath(MobData mob, Consts.DamageHeal_Result info)
        {
            if (triggerOnDeath)
            {
                yield return new JumpIn(mob.DoAction(runtimeSkill, new SingleMobTarget(mob, mob.GridPosition)));
            }
        }

        public IEnumerator OnReceiveHealFinal(MobData mob, Consts.DamageHeal_Result info)
        {
            damageTotal -= info.value;
            yield break;
        }
    }
}