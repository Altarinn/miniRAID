using System;
using System.Collections;
using System.Collections.Generic;
using miniRAID.TurnSchedule;
using UnityEngine;

namespace miniRAID.Agents
{
    [Obsolete]
    public abstract class MonoBehaviourAgentComponent : MonoBehaviour
    {
        public MonoBehaviourAgent agent;

        public abstract IEnumerator Turn(MobData mob, Timestamp turn);

        public virtual string GetIncomingString(MobData mob, int turn) { return "UNKNOWN"; }
    }
}
