using System;
using System.Collections;
using System.Collections.Generic;
using miniRAID.TurnSchedule;
using UnityEngine;

namespace miniRAID.Agents
{
    [CreateAssetMenu(menuName = "Agents/MonoBehaviourAgent")]
    public class MonoBehaviourAgentSO : TargetedAgentBaseSO
    {
        public override MobTurnSlice Wrap(MobData parent, TurnSliceMetadata metadata)
        {
            return new MonoBehaviourAgent(parent, this, metadata);
        }
    }

    [Obsolete]
    public class MonoBehaviourAgent : TargetedAgentBase
    {
        MonoBehaviourAgentComponent agentComponent;

        public MonoBehaviourAgent(MobData mob, MonoBehaviourAgentSO data, TurnSliceMetadata metadata) 
            : base(mob, data, metadata)
        {
            this.data = data;
            
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

        // public Dictionary<MobData, float> AggroList => mobAggro.AggroList;

        public IEnumerator RegularAttack() => Turn();

        public override IEnumerator Turn()
        {
            if(agentComponent != null)
            {
                yield return new JumpIn(agentComponent.Turn(mob, Globals.combatMgr.Instance.now));
            }
        }

        // public override string GetIncomingString(MobData mob)
        // {
        //     return agentComponent.GetIncomingString(mob, Globals.combatMgr.Instance.turn);
        // }
    }
}
