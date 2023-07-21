using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace miniRAID.Agents
{
    public abstract class MobAgentBaseSO : MobListenerSO
    {
        public MobAgentBaseSO() { type = ListenerType.Agent; }

        public override MobListener Wrap(MobData parent)
        {
            return base.Wrap(parent);
        }
    }

    public abstract class MobAgentBase : MobListener
    {
        public MobAgentBaseSO agentData => (MobAgentBaseSO)data;

        public MobAgentBase(MobData parent, MobAgentBaseSO data) : base(parent, data)
        {
            this.data = data;
        }

        public override void OnEnterScene(MobData mob)
        {
            base.OnEnterScene(mob);
        }

        public override void OnAttach(MobData mob)
        {
            base.OnAttach(mob);

            mob.OnAgentWakeUp += OnAgentWakeUp;
        }

        public override void OnRemove(MobData mob)
        {
            base.OnRemove(mob);

            mob.OnAgentWakeUp -= OnAgentWakeUp;
        }

        protected abstract IEnumerator OnAgentWakeUp(MobData mob);

        public virtual string GetIncomingString(MobData mob) { return "UNKNOWN"; }
    }
}
