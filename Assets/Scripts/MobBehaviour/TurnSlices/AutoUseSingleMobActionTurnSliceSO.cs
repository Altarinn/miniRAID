using System.Collections;
using miniRAID.Agents;
using miniRAID.Spells;
using miniRAID.TurnSchedule;
using miniRAID.UI.TargetRequester;
using miniRAID.ActionHelpers;
using UnityEngine;

namespace miniRAID.MobBehaviour.TurnSlices
{
    public class AutoUseSingleMobActionTurnSliceSO : MobTurnSliceBaseSO
    {
        public ActionSOEntry actionToUse;
        
        public override IEnumerator Turn(TurnSlice slice, CombatSchedulerCoroutine coroutine)
        {
            var mob = ((MobTurnSlice)slice).mob;

            var targetIndicator = mob.FindListener<TargetIndicator>();
            if (targetIndicator == null)
            {
                Debug.LogError("No AggroCollector. Skipped.");
                yield break;
            }

            var ract = mob.GetAction(actionToUse.data) as RuntimeAction<SingleMobTarget>;
            if (ract == null)
            {
                ract = mob.AddAction(actionToUse) as RuntimeAction<SingleMobTarget>;
                
                if (ract == null)
                {
                    Debug.LogError($"Cannot add {actionToUse.data.name} to mob {mob.nickname} (not SingleMobTarget?). Action skipped.");
                    yield break;
                }
            }
            
            if (ract.level != actionToUse.level)
            {
                Debug.LogError($"Action {actionToUse.data.name}: level in schedule does not match the level of the action in the mob.");
            }
            
            if(targetIndicator.CurrentTarget != null)
            {
                SingleMobTarget sTarget;
            
                // TODO: FIXME: IsAssignableFrom order reversed?
                if (ract.actionData?.RequestValidator?.GetType().IsAssignableFrom(typeof(ConfirmRequestValidator)) ?? false)
                {
                    sTarget = new SingleMobTarget(mob, mob.GridPosition);
                }
                else
                {
                    sTarget = ValidTargetFinder.FindValidSingleMobTarget(mob, targetIndicator.CurrentTarget, ract.actionData);
                }
            
                if (sTarget != null)
                {
                    yield return new JumpIn(mob.DoActionWithDefaultCosts(
                        ract,
                        sTarget
                    ));
                }
            }
        }
    }
}