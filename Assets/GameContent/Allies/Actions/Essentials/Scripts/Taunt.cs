using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;
using System.Linq;
using miniRAID.ActionHelpers;
using miniRAID.Agents;
using miniRAID.Spells;

namespace miniRAID
{
    public class Taunt : ActionDataSO<SingleMobTarget>
    {
        public override IEnumerator OnPerform(RuntimeAction<SingleMobTarget> ract, MobData mob,
            SingleMobTarget target)
        {
            MobData targetMob = target.Target;
            
            // Check if target is aggro based
            if (targetMob.FindListener<AggroAgentBase>() != null)
            {
                var aggroAgent = targetMob.FindListener<AggroAgentBase>();
                aggroAgent.SetAsMaxAggro(mob, 1.2f);
            }

            yield break;
        }
    }
}
