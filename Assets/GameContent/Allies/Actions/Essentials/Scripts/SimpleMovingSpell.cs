using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;
using System.Linq;
using miniRAID.Spells;

namespace miniRAID
{
    public class SimpleMovingSpell : ActionDataSO<SingleCoordinateTarget>
    {
        public bool isInstant = false;
        
        public override IEnumerator OnPerform(RuntimeAction<SingleCoordinateTarget> ract, MobData mob,
            SingleCoordinateTarget target)
        {
            if (isInstant)
            {
                if (!Globals.backend.CanPositionPlaceMob(target.Target, mob.Collider))
                {
                    yield break;
                }

                yield return new JumpIn(mob.SetPosition(target.Target));
            }
            else
            {
                throw new NotImplementedException();
                // yield return new JumpIn(mob.MoveToCoroutine(target.Target, null, false));
            }
        }
    }
}
