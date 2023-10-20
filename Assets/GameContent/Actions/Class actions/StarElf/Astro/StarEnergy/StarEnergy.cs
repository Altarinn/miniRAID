using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using miniRAID;
using miniRAID.Buff;
using miniRAID.Spells;

namespace GameContent.Buffs.Test
{
    public class StarEnergy : PassiveSkillSO
    {
        public int manaRegen = 10;
        
        public override MobListener Wrap(MobData parent)
        {
            return new StarEnergyRuntimeBuff(parent, this);
        }
    }

    public class StarEnergyRuntimeBuff : Buff
    {
        private StarEnergy starEnergyData => (StarEnergy)buffData;
        private GeneralManaListener mobMana;
        
        public StarEnergyRuntimeBuff(MobData source, StarEnergy data) : base(source, data)
        {}

        public override void OnAttach(MobData mob)
        {
            base.OnAttach(mob);

            mobMana = mob.FindListener<GeneralManaListener>();
            mob.OnActionPostcast += MobOnActionPostcast;

            onRemoveFromMob += m =>
            {
                m.OnActionPostcast -= MobOnActionPostcast;
            };
        }

        public IEnumerator MobOnActionPostcast(MobData mob, RuntimeAction ract, SpellTarget target)
        {
            if (ract.flags.HasFlag(Consts.ActionFlags.RegularAction))
            {
                mobMana.AddMana(starEnergyData.manaRegen);
            }

            yield break;
        }
    }
}