using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace miniRAID.TurnSchedule.RootAgent
{
    public class InsertSimpleSliceBeforeSpecificSliceRootAgentSO : InsertBeforeSpecificSliceBaseRootAgentSO
    {
        [SerializeField]
        private TurnSliceSO sliceToInsert;

        protected override TurnSlice GetTurnSlice(Timestamp now, MobRootAgentBase agent, List<TurnSlice> schedule)
        {
            return sliceToInsert.Wrap(new TurnSliceMetadata(
                agent.parentMob, agent.GetTurnSliceModificationPriority(now, schedule)));
        }
    }
}