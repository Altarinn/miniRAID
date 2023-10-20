using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using miniRAID;
using miniRAID.ActionHelpers;
using miniRAID.Buff;

namespace GameContent.Buffs.Test
{
    public class FallenStar : PassiveSkillSO
    {
        public float probability = 0.1f;
        
        public Projectile projectile;
        public SpellDamageHeal damage;
        
        public override MobListener Wrap(MobData parent)
        {
            return new FallenStarRuntimeBuff(parent, this);
        }
    }

    public class FallenStarRuntimeBuff : Buff
    {
        FallenStar fallenStarData => (FallenStar)buffData;
        
        public FallenStarRuntimeBuff(MobData source, FallenStar data) : base(source, data)
        {}

        public override void OnAttach(MobData mob)
        {
            base.OnAttach(mob);

            mob.OnDamageDealt += MobOnDamageDealt;

            onRemoveFromMob += m =>
            {
                m.OnDamageDealt -= MobOnDamageDealt;
            };
        }

        public IEnumerator MobOnDamageDealt(MobData mob, Consts.DamageHeal_Result result)
        {
            if (!Consts.IsDirect(result.flags))
            {
                yield break;
            }

            if (Globals.cc.rng.WithProbability(fallenStarData.probability, false))
            {
                if (fallenStarData.projectile != null)
                    yield return new JumpIn(fallenStarData.projectile.WaitForShootAt(source, result.target.Position));

                if (fallenStarData.damage != null)
                    yield return new JumpIn(fallenStarData.damage.Do(this, source, result.target));
            }
        }
    }
}