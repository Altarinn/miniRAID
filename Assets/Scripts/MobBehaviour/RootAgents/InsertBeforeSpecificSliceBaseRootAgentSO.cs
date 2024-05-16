using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace miniRAID.TurnSchedule.RootAgent
{
    public abstract class InsertBeforeSpecificSliceBaseRootAgentSO : MobRootAgentBaseSO
    {
        [SerializeField]
        private TurnSliceSO pivotSlice;

        protected abstract TurnSlice GetTurnSlice(Timestamp now, MobRootAgentBase agent, List<TurnSlice> schedule);
        
        public override void ModifyTurnSlicesInPlace(Timestamp now, MobRootAgentBase agent, List<TurnSlice> schedule)
        {
            Enumerable.Range(0, schedule.Count)
                .Where(i => schedule[i].data == pivotSlice)
                .ToList()
                .ForEach(i => schedule.Insert(i, GetTurnSlice(now, agent, schedule)));
        }
    }
}