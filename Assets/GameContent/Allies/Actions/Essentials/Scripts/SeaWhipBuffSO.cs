using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using miniRAID;
using miniRAID.ActionHelpers;
using miniRAID.Buff;

namespace GameContent.Buffs.Test
{
    public class SeaWhipBuffSO : BuffSO
    {
        public Projectile projectile;
        public SpellDamageHeal damage;
        
        public override MobListener Wrap(MobData parent)
        {
            return new SeaWhipBuffSORuntimeBuff(parent, this);
        }
    }

    public class SeaWhipBuffSORuntimeBuff : Buff
    {
        private SeaWhipBuffSO seawhipData => (SeaWhipBuffSO)data;
        
        public SeaWhipBuffSORuntimeBuff(MobData source, SeaWhipBuffSO data) : base(source, data)
        {}

        public override void OnAttach(MobData mob)
        {
            base.OnAttach(mob);

            mob.OnDamageDealt += OnDamageDealt;
            onRemoveFromMob += m =>
            {
                m.OnDamageDealt -= OnDamageDealt;
            };
        }

        public IEnumerator OnDamageDealt(MobData mob, Consts.DamageHeal_Result info)
        {
            if (Consts.IsDirect(info.flags))
            {
                if (seawhipData.projectile != null)
                    yield return new JumpIn(seawhipData.projectile.WaitForShootAt(source, info.target.Position));

                if (seawhipData.damage != null)
                    yield return new JumpIn(seawhipData.damage.Do(this, source, info.target));
            }
        }
    }
}