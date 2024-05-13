using System.Collections;
using miniRAID.Agents;
using UnityEngine;

namespace miniRAID.TurnSchedule
{
    public class MobProxyAgentTurnSlice : TurnSlice
    {
        protected MobData mob;
        protected ActionSOEntry action;
        protected MobProxyAgentSO agent;
        
        public MobProxyAgentTurnSlice(TurnSliceSO data) : base(data)
        {
        }

        public void Setup(MobData mob, ActionSOEntry actionSO, MobProxyAgentSO agentSO)
        {
            this.mob = mob;
            this.action = action;
            this.agent = agentSO;
        }
        
        public override IEnumerator Turn()
        {
            if (mob == null)
            {
                Debug.LogError("Please call MobProxyAgentTurnSlice.SetMob() before MobProxyAgentTurnSlice.Turn()!!");
                yield break;
            }

            MobProxyAgent agent = this.agent.Wrap(mob) as MobProxyAgent;
            agent.SetProxyTurnSliceInfo(mob, action);
            
            // TODO: Disable mob's current agent's OnAgentWakeUp
            // TODO: FIXME: Multiple agents?
            // mob.FindListener<MobAgentBase>()

            mob.AddListener(agent);
            
            // TODO: Wake up mob
            
            // TODO: End of turn - remove agent from mob and re-attach original agent
        }
    }
}