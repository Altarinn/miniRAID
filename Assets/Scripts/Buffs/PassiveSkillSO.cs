using System;
using System.Collections;
using UnityEngine;

using Sirenix.OdinInspector;
using System.Collections.Generic;

namespace miniRAID.Buff
{
    [CreateAssetMenu(menuName = "Buffs/PassiveSkill")]
    public partial class PassiveSkillSO : BuffSO
    {
        public PassiveSkillSO()
        {
            base.type = ListenerType.Passive;

            timed = false;
            stackable = false;
        }

        public override MobListener Wrap(MobData parent)
        {
            return new Buff(parent, this);
        }
    }
}