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
    public class StarlightPulseBuff : BuffSO
    {
        public ActionDataSO starlightPulseAction;
        public SimpleExplosionFx fx;
        public SpellDamageHeal damage;

        public SimpleRayFx rayFx;
        
        public override MobListener Wrap(MobData parent)
        {
            return new StarlightPulseBuffRuntimeBuff(parent, this);
        }
    }

    public class StarlightPulseBuffRuntimeBuff : Buff
    {
        StarlightPulseBuff starlightPulseData => ((StarlightPulseBuff)buffData);
        
        public StarlightPulseBuffRuntimeBuff(MobData source, StarlightPulseBuff data) : base(source, data)
        {}

        public override void OnAttach(MobData mob)
        {
            base.OnAttach(mob);
            
            mob.OnDamageReceived += MobOnDamageReceived;

            onRemoveFromMob += m =>
            {
                m.OnDamageReceived -= MobOnDamageReceived;
            };
        }
        
        public IEnumerator MobOnDamageReceived(MobData mob, Consts.DamageHeal_Result result)
        {
            if ((!result.isAvoid) && (!result.isBlock) &&
                result.IsAction && 
                result.sourceAction.data == starlightPulseData.starlightPulseAction)
            {
                for (int i = 0; i < stacks; i++)
                {
                    if (starlightPulseData.fx != null)
                        yield return new JumpIn(starlightPulseData.fx.Do(result.target.Position));

                    if (starlightPulseData.damage != null)
                        yield return new JumpIn(starlightPulseData.damage.Do(this, result.source, result.target));
                }
            }

            yield break;
        }
    }
}