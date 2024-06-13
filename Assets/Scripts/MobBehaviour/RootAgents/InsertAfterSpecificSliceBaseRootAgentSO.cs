using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace miniRAID.TurnSchedule.RootAgent
{
    public abstract class InsertAfterSpecificSliceBaseRootAgentSO : MobRootAgentBaseSO
    {
        [SerializeField]
        protected TurnSliceSO pivotSlice;

        protected abstract TurnSlice GetTurnSlice(Timestamp now, MobRootAgentBase agent, TurnScheduleSequence schedule);
        
        public override void ModifyTurnSlicesInPlace(Timestamp now, MobRootAgentBase agent, TurnScheduleSequence schedule)
        {
            schedule.Where(n => n.Value.data == pivotSlice)
                .ToList()
                .ForEach(n => schedule.InsertTurnSliceAfter(n, GetTurnSlice(now, agent, schedule)));
        }
    }
}