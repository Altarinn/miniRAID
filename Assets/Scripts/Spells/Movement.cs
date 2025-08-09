using System;
using System.Collections;
using miniRAID.ActionHelpers;
using miniRAID.Spells;
using UnityEngine;

namespace miniRAID.Actions
{
    public class Movement : ActionDataSO<SingleCoordinateTarget>
    {
        public override IEnumerator OnPerform(RuntimeAction<SingleCoordinateTarget> ract, MobData mob, SingleCoordinateTarget target)
        {
            throw new NotImplementedException();
            // yield return new JumpIn(mob.MoveToCoroutine(target.Target, null));
        }
    }
}