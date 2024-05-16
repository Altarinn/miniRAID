using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace miniRAID.TurnSchedule.RootAgent
{
    [CreateAssetMenu(menuName = "RootAgents/Insert MobSlice Before Specific Slice RootAgent")]
    public class InsertMobSliceBeforeSpecificSliceRootAgentSO : InsertBeforeSpecificSliceBaseRootAgentSO
    {
        [SerializeField]
        [InlineEditor(InlineEditorObjectFieldModes.Boxed)]
        private MobTurnSliceBaseSO sliceToInsert;

        protected override TurnSlice GetTurnSlice(Timestamp now, MobRootAgentBase agent, List<TurnSlice> schedule)
        {
            return sliceToInsert.Wrap(agent.parentMob,
                new TurnSliceMetadata(agent.parentMob, agent.GetTurnSliceModificationPriority(now, schedule)));
        } 
    }
}