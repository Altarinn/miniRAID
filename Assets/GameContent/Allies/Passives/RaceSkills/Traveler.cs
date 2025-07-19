using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using miniRAID;
using miniRAID.ActionHelpers;
using miniRAID.Buff;

namespace GameContent.Buffs.Test
{
    public class Traveler : PassiveSkillSO
    {
        public SpellDamageHeal healPerMovement;
        
        public override MobListener Wrap(MobData parent)
        {
            return new TravelerRuntimeBuff(parent, this);
        }
    }

    public class TravelerRuntimeBuff : Buff
    {
        Traveler travelerData => data as Traveler;
        
        public TravelerRuntimeBuff(MobData source, Traveler data) : base(source, data)
        {}

        public override void OnAttach(MobData mob)
        {
            base.OnAttach(mob);
            
            mob.OnMobMoved.AddListener(MobOnMobMoved);
            
            /* Custom events:
            mob.OnXXXX += XXXX;
            onRemoveFromMob += m =>
            {
                m.OnXXXX -= XXXX;
            };
            */
        }

        protected override void OnRemoveFromMob(MobData mob)
        {
            mob.OnMobMoved.RemoveListener(MobOnMobMoved);
            base.OnRemoveFromMob(mob);
        }

        private IEnumerator MobOnMobMoved(MobData mob, Vector3Int from)
        {
            yield return new JumpIn(travelerData.healPerMovement.Do(this, mob, mob));
        }
    }
}