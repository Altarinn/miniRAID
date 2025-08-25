using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;
using System.Linq;
using miniRAID.Spells;
using DG.Tweening;
using miniRAID.ActionHelpers;

using System.Linq;

namespace miniRAID
{
    public class SlimeJump_new : ActionDataSO<SingleCoordinateTarget>
    {
        public CreateGridEffect poisonPool;
        public ActionSOEntry skillJumpOnPool;
        public ShowImportantMessage importantMessage;

        private bool CheckGridHasPoisonPool(GridData grid)
        {
            throw new NotImplementedException();
            return false;
            
            // var effectList = grid.effects;
            // return effectList.Any(kvp => kvp.data == poisonPool.effect);
        }
        
        public override IEnumerator OnPerform(RuntimeAction<SingleCoordinateTarget> ract, MobData mob,
            SingleCoordinateTarget target)
        {
            // Perform Jump
            yield return mob.WaitForAnimation("JumpPrepare");

            var targetPos = Globals.backend.FindNearestEmptyGrid(target.Target, mob.Collider);
            throw new NotImplementedException();
            // yield return new JumpIn(mob.MoveToCoroutine(targetPos, null));
            
            // Check if landed on poison pool
            if (CheckGridHasPoisonPool(Globals.backend.GetMap(mob.Position)))
            {
                yield return new JumpIn(importantMessage.Do());
                yield return new JumpIn(mob.DoAction(skillJumpOnPool, new SingleMobTarget(mob, mob.GridPosition)));
            }
            
            // Generate poison pool
            // yield return new JumpIn(poisonPool.Do(ract, mob, targetPos));

            yield return mob.WaitForAnimation("JumpLanding");
        }
    }
}
