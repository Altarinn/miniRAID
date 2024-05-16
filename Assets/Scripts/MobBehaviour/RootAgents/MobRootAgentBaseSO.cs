using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace miniRAID.TurnSchedule.RootAgent
{
    public abstract class MobRootAgentBaseSO : MobListenerSO
    {
        public MobRootAgentBaseSO() { type = ListenerType.Agent; }
        
        [Tooltip("Different from mob.AGI; as this is not expected to be changed dynamically across combat.")]
        public int TurnSchedulePriority;

        public abstract void ModifyTurnSlicesInPlace(Timestamp now, MobRootAgentBase agent, List<TurnSlice> schedule);

        public override MobListener Wrap(MobData parent)
        {
            return new MobRootAgentBase(parent, this);
        }
    }

    public class MobRootAgentBase : MobListener
    {
        public MobRootAgentBaseSO agentData => (MobRootAgentBaseSO)data;

        public MobRootAgentBase(MobData parent, MobRootAgentBaseSO data) : base(parent, data)
        {
            this.data = data;
        }

        public virtual int GetTurnSliceModificationPriority(Timestamp now, List<TurnSlice> schedule)
            => agentData.TurnSchedulePriority;

        public virtual void ModifyTurnSlicesInPlace(Timestamp now, List<TurnSlice> schedule)
            => agentData.ModifyTurnSlicesInPlace(now, this, schedule);

        [Obsolete]
        public virtual string GetIncomingString(MobData mob) { return "BAD_STR MobRootAgentBaseSO.cs:42"; }
    }
}
