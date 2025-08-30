using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace miniRAID.TurnSchedule.RootAgent
{
    [CreateAssetMenu(menuName = "RootAgents/Claim Mob Slice Root Agent")]
    public class ClaimMobSliceRootAgentSO : MobRootAgentBaseSO
    {
        [SerializeField] private List<MobTurnSliceBaseSO> targetSlices;
        
        public override void ModifyTurnSlicesInPlace(Timestamp now, MobRootAgentBase agent, TurnScheduleSequence schedule)
        {
            schedule
                .Where(x => targetSlices.Contains(x.Value.data))
                .ToList()
                .ForEach(x => schedule.Claim(x, (x.Value.data as MobTurnSliceBaseSO)?.Wrap(agent.parentMob, new TurnSliceMetadata(agent.parentMob))));
        }
    }
}