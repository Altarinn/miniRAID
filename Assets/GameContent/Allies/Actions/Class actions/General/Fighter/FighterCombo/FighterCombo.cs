using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using miniRAID;
using miniRAID.Buff;
using miniRAID.Spells;
using Sirenix.Serialization;

namespace GameContent.Buffs.Test
{
    public class FighterCombo : PassiveSkillSO
    {
        public float scalingFactor = 0.08f;
        
        public override MobListener Wrap(MobData parent)
        {
            return new FighterComboRuntimeBuff(parent, this);
        }
    }

    public class FighterComboRuntimeBuff : Buff
    {
        [OdinSerialize] private HashSet<ActionDataSO> performedActions = new();
        private FighterCombo fcData => (FighterCombo)data;
        
        public FighterComboRuntimeBuff(MobData source, FighterCombo data) : base(source, data)
        {}

        public override void OnAttach(MobData mob)
        {
            base.OnAttach(mob);

            mob.OnActionPrecast.AddListener(OnPreCast);
            mob.OnPassed.AddListener(Reset);
            mob.OnWakeup.AddListener(Reset);
            mob.OnStatCalculation.AddListener(OnStatCalculation);
        }

        public override void OnRemove(MobData mob)
        {
            mob.OnActionPrecast.RemoveListener(OnPreCast);
            mob.OnPassed.RemoveListener(Reset);
            mob.OnWakeup.RemoveListener(Reset);
            mob.OnStatCalculation.RemoveListener(OnStatCalculation);
            
            base.OnRemove(mob);
        }

        public void OnStatCalculation(MobData mob)
        {
            mob.attackPower.MulMul(1.00f + fcData.scalingFactor * stacks);
        }

        public IEnumerator Reset(MobData mob)
        {
            performedActions.Clear();
            stacks = 0;

            yield break;
        }

        public IEnumerator OnPreCast(MobData mob, RuntimeAction ract, SpellTarget target)
        {
            // Ignore mob's natural movement
            if (ract.data == mob.movement?.data)
            {
                yield break;
            }
            
            if (performedActions.Add(ract.data))
            {
                mob.AddListener(this.data);
            }

            yield break;
        }
    }
}