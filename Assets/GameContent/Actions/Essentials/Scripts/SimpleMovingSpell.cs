using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;
using System.Linq;
using miniRAID.Spells;

namespace miniRAID
{
    public class SimpleMovingSpell : ActionDataSO
    {
        public bool isInstant = false;
        
        public override IEnumerator OnPerform(RuntimeAction ract, MobData mob,
            Spells.SpellTarget target)
        {
            if (isInstant)
            {
                mob.Position = target.targetPos[0];
            }
            else
            {
                yield return new JumpIn(mob.MoveToCoroutine(target.targetPos[0], null, false));
            }
        }
    }
}
