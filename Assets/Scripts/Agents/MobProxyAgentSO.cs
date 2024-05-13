using UnityEngine;

namespace miniRAID.Agents
{
    public abstract class MobProxyAgentSO : MobAgentBaseSO
    {
        public override MobListener Wrap(MobData parent)
        {
            return base.Wrap(parent);
        }
    }

    public abstract class MobProxyAgent : MobAgentBase
    {
        protected ActionSOEntry actionThisTurn;
        protected MobData mobThisTurn; // Should always be the same as parent
        
        protected MobProxyAgent(MobData parent, MobAgentBaseSO data) : base(parent, data) { }

        public virtual void SetProxyTurnSliceInfo(MobData mob, ActionSOEntry action)
        {
            actionThisTurn = action;
            
            if (mob != parentMob)
            {
                Debug.LogError("MobProxyAgent: SetProxyTurnSliceInfo's mob is different from parentMob!");
            }
            mobThisTurn = mob;
        }
    }
}