using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace miniRAID.Agents
{
    public class LockTargetAgent : MonoBehaviourAgentComponent
    {
        public MobRenderer initialTarget;
        public MobData target;

        private void Awake()
        {
            if (initialTarget != null)
            {
                target = initialTarget.data;
            }
        }

        public override IEnumerator Act(MobData mob, int turn)
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
