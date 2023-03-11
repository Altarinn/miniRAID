using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;
using System.Linq;
using miniRAID.Spells;
using DG.Tweening;

namespace miniRAID
{
    public class SlimeJump_new : ActionDataSO
    {
        public override IEnumerator OnPerform(GeneralCombatData combatData, Mob mob, SpellTarget target)
        {
            // Perform Jump
            yield return mob.WaitForAnimation("JumpPrepare");

            var targetPos = Globals.backend.FindNearestEmptyGrid(target.targetPos[0], mob.gridBody);
            yield return new JumpIn(mob.MoveToCorotine(targetPos, null, null));

            yield return mob.WaitForAnimation("JumpLanding");
        }
    }
}
