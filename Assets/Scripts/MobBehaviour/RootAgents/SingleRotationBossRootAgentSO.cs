using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace miniRAID.TurnSchedule.RootAgent
{
    [Serializable]
    public class SliceList
    {
        public List<MobTurnSliceBaseSO> list;
    }
    
    public class SingleRotationBossRootAgentSO : MobRootAgentBaseSO
    {
        [SerializeField]
        private List<SliceList> sliceRotations;
        
        [SerializeField]
        protected TurnSliceSO pivotSlice;
        
        public override void ModifyTurnSlicesInPlace(Timestamp now, MobRootAgentBase agent, TurnScheduleSequence schedule)
        {
            int idx = now.currentTurnID % sliceRotations.Count;
            var slices = sliceRotations[idx].list.Select(
                x => x.Wrap(
                    agent.parentMob,
                    new TurnSliceMetadata(agent.parentMob, agent.GetTurnSliceModificationPriority(now, schedule))));
            
            schedule.Where(n => n.Value.data == pivotSlice)
                .ToList()
                .ForEach(n =>
                {
                    foreach (var s in slices)
                    {
                        schedule.InsertTurnSliceAfter(n, s);
                    }
                });
        }
    }
}