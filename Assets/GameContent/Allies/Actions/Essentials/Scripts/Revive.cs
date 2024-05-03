using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;
using System.Linq;
using miniRAID.ActionHelpers;
using miniRAID.Spells;

namespace miniRAID
{
    public class Revive : ActionDataSO<SingleMobTarget>
    {
        public SpellDamageHeal damageOrHeal;
        
        public override IEnumerator OnPerform(RuntimeAction<SingleMobTarget> ract, MobData mob,
            SingleMobTarget target)
        {
            var info = damageOrHeal.GetInfo(ract, null, mob);
            yield return new JumpIn(target.Target.Revive(info));
        }
    }
}
