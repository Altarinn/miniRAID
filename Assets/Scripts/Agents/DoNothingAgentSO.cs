using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace miniRAID.Agents
{
    [CreateAssetMenu(menuName = "Agents/NullAgent")]
    public class DoNothingAgentSO : MobAgentBaseSO
    {
        public DoNothingAgentSO() { type = ListenerType.Agent; }

        public override MobListener Wrap(MobData parent)
        {
            return new DoNothingAgent(parent, this);
        }
    }

    public class DoNothingAgent : MobAgentBase
    {
        public DoNothingAgent(MobData parent, MobAgentBaseSO data) : base(parent, data)
        {
        }

        protected override IEnumerator OnAgentWakeUp(MobData mob)
        {
            yield return new JumpIn(mob.SetActive(false));
        }
    }
}