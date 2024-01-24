using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;
using System.Linq;
using miniRAID.Spells;

namespace miniRAID
{
    public class OffhandAttack : ActionDataSO
    {
        public override IEnumerator OnPerform(RuntimeAction ract, MobData mob,
            Spells.SpellTarget target)
        {
            return base.OnPerform(ract, mob, target);
        }
    }
}
