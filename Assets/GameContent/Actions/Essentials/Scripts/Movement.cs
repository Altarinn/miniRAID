using System.Collections;
using miniRAID.ActionHelpers;
using miniRAID.Spells;
using UnityEngine;

namespace miniRAID.Actions
{
    public class Movement : ActionDataSO
    {
        public override IEnumerator OnPerform(RuntimeAction ract, Mob mob, SpellTarget target)
        {
            yield return new JumpIn(mob.MoveToCorotine(target.targetPos[0], null, null));
        }
    }
}