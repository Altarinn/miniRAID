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

        public Dictionary<Mob, float> AggroList => aggroList;

        public IEnumerator RegularAttack(Mob mob) => Act(mob);

        public override void OnAttach(Mob mob)
        {
            base.OnAttach(mob);
            agentComponent = mob.GetComponent<MonoBehaviourAgentComponent>();
            agentComponent.agent = this;

            if (agentComponent == null)
            {
                Debug.LogError($"Mob {mob.data.nickname}({mob.name}): MonoBehaviourAgent requires a MonoBehaviourAgentComponent attached to the GameObject to work.");
            }
        }

        protected override IEnumerator OnAgentWakeUp(Mob mob)
        {
            if(agentComponent != null)
            {
                yield return new JumpIn(agentComponent.Act(mob, Globals.combatMgr.Instance.turn));
            }
        }

        public override string GetIncomingString(Mob mob)
        {
            return agentComponent.GetIncomingString(mob, Globals.combatMgr.Instance.turn);
        }
    }
}
