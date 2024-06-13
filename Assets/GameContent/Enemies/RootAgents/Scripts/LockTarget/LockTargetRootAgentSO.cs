using miniRAID.TurnSchedule;
using miniRAID.TurnSchedule.RootAgent;

namespace miniRAID.Agents.RootAgents.Scripts
{
    public class LockTargetRootAgentSO : InsertMobSliceAfterSpecificSliceRootAgentSO
    {
        public override MobListener Wrap(MobData parent)
        {
            return new LockTargetRootAgent(parent, this);
        }

        protected override TurnSlice GetTurnSlice(Timestamp now, MobRootAgentBase _agent, TurnScheduleSequence schedule)
        {
            LockTargetRootAgent agent = (LockTargetRootAgent)_agent;
            return new LockTargetTS(agent.parentMob, agent.targetMob, null, new TurnSliceMetadata(
                agent.parentMob, agent.GetTurnSliceModificationPriority(now, schedule)));
        }
    }
    
    public class LockTargetRootAgent : MobRootAgentBase
    {
        public MobData targetMob;

        public LockTargetRootAgent(MobData parent, MobRootAgentBaseSO data) : base(parent, data)
        { }
    }

    public class LockTargetTS : MobTurnSlice
    {
        public LockTargetTS(MobData mob, MobData target, AbstractTurnSliceSO data, TurnSliceMetadata metadata) : base(mob, data, metadata)
        {
        }
    }
}