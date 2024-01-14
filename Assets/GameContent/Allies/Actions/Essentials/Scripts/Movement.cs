using System.Collections;
using miniRAID.ActionHelpers;
using miniRAID.Spells;
using UnityEngine;

namespace miniRAID.Actions
{
    public class Movement : ActionDataSO
    {
        public override IEnumerator OnPerform(RuntimeAction ract, MobData mob, SpellTarget target)
        {
            yield return new JumpIn(mob.MoveToCoroutine(target.targetPos[0], null));
        }
    }
}