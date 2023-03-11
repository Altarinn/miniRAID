using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace miniRAID.Agents
{
    public class LockTargetAgent : MonoBehaviourAgentComponent
    {
        public Mob target;

        public override IEnumerator Act(Mob mob, int turn)
        {
            if(target != null)
            {
                agent.useAggro = false;
                agent.currentTarget = target;
                yield return new JumpIn(agent.Act(mob));
            }
        }
    }
}
