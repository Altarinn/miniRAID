using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using miniRAID.TurnSchedule;
using Sirenix.OdinInspector;

namespace miniRAID.MobBehaviour.TurnSlices
{
    // Might be a SO later
    
    public class PreparableActionPreparationTurnSliceSO : MobTurnSliceBaseSO
    {
        [InlineEditor(InlineEditorObjectFieldModes.Boxed)]
        public List<PreparableActionSO> actionList;
        public TurnSliceSO playerTurn; // Will be used for checking and insertion
        
        public override IEnumerator Turn(TurnSlice slice, CombatSchedulerCoroutine coroutine)
        {
            // TODO: Formalize me
            MobData mob = ((MobTurnSlice)slice).mob;
            
            float rng = Globals.cc.rng.NextFloat();
            var weights = actionList.Select(x => x.GetWeight(mob)).ToList();
            float totalWeight = weights.Sum();
            
            rng = rng * totalWeight;

            PreparableActionSO pickedAction = actionList[0];
            for (int i = 0; i < actionList.Count; i++)
            {
                rng -= weights[i];
                if (rng <= 0)
                {
                    pickedAction = actionList[i];
                    break;
                }
            }

            pickedAction.ModifySchedule(mob, coroutine);
            
            yield break;
        }
    }
}