using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace miniRAID.Agents
{
    [CreateAssetMenu(menuName = "Agents/MonoBehaviourAgent")]
    public class MonoBehaviourAgentSO : AggroAgentBaseSO
    {
        public override MobListener Wrap(MobData parent)
        {
            return new MonoBehaviourAgent(parent, this);
        }
    }

    public class MonoBehaviourAgent : AggroAgentBase
    {
        MonoBehaviourAgentComponent agentComponent;

        public MonoBehaviourAgent(MobData parent, MonoBehaviourAgentSO data) : base(parent, data)
        {
            this.data = data;
        }

        public Dictionary<MobData, float> AggroList => aggroList;

        public IEnumerator RegularAttack(MobData mob) => Act(mob);

        public override void OnAttach(MobData mob)
        {
            base.OnAttach(mob);

            if (mob.mobRenderer == null)
            {
                Debug.LogError("MonoBehaviourAgent cannot be attached to a Mob(data) that has no valid MobRenderer!");
            }
            
            agentComponent = mob.mobRenderer.GetComponent<MonoBehaviourAgentComponent>();
            agentComponent.agent = this;

            if (agentComponent == null)
            {
                Debug.LogError($"Mob {mob.nickname}({mob.mobRenderer.name}): MonoBehaviourAgent requires a MonoBehaviourAgentComponent attached to the GameObject to work.");
            }
        }

        protected override IEnumerator OnAgentWakeUp(MobData mob)
        {
            if(agentComponent != null)
            {
                yield return new JumpIn(agentComponent.Act(mob, Globals.combatMgr.Instance.turn));
            }
        }

        public override string GetIncomingString(MobData mob)
        {
            return agentComponent.GetIncomingString(mob, Globals.combatMgr.Instance.turn);
        }
    }
}
