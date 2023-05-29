using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace miniRAID.Agents
{
    public abstract class MonoBehaviourAgentComponent : MonoBehaviour
    {
        public MonoBehaviourAgent agent;

        public abstract IEnumerator Act(MobData mob, int turn);

        public virtual string GetIncomingString(MobData mob, int turn) { return "UNKNOWN"; }
    }
}
