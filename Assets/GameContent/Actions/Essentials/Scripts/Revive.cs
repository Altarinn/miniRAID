using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;
using System.Linq;
using miniRAID.ActionHelpers;
using miniRAID.Spells;

namespace miniRAID
{
    public class Revive : ActionDataSO
    {
        public SpellDamageHeal healing;
        
        public override IEnumerator OnPerform(RuntimeAction ract, MobData mob,
            Spells.SpellTarget target)
        {
            var targetMob = Globals.backend.GetMap(target.targetPos[0])?.mob;
            if (targetMob == null)
            {
                yield break;
            }

            var info = healing.GetInfo(ract, null, mob);
            yield return new JumpIn(targetMob.Revive(info));
        }
    }
}
