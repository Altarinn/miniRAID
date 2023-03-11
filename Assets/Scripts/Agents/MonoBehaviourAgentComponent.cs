using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace miniRAID.Agents
{
    public abstract class MonoBehaviourAgentComponent : MonoBehaviour
    {
        public MonoBehaviourAgent agent;

        public abstract IEnumerator Act(Mob mob, int turn);

        public virtual string GetIncomingString(Mob mob, int turn) { return "UNKNOWN"; }
    }
}
