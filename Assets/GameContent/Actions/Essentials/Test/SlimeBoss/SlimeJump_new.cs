using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;
using System.Linq;
using miniRAID.Spells;
using DG.Tweening;
using miniRAID.ActionHelpers;

namespace miniRAID
{
    public class SlimeJump_new : ActionDataSO
    {
        public CreateGridEffect poisonPool;
        
        public override IEnumerator OnPerform(RuntimeAction ract, MobData mob,
            Spells.SpellTarget target)
        {
            // Perform Jump
            yield return mob.WaitForAnimation("JumpPrepare");

            var targetPos = Globals.backend.FindNearestEmptyGrid(target.targetPos[0], mob.gridBody);
            yield return new JumpIn(mob.MoveToCoroutine(targetPos, null));
            
            // Generate poison pool
            yield return new JumpIn(poisonPool.Do(ract, mob, targetPos));

            yield return mob.WaitForAnimation("JumpLanding");
        }
    }
}
