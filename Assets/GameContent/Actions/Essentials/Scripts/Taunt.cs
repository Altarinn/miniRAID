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
    public class Taunt : ActionDataSO
    {
        public override IEnumerator OnPerform(RuntimeAction ract, MobData mob,
            Spells.SpellTarget target)
        {
            MobData targetMob = Essentials.MobAtGrid(target.targetPos[0]);
            
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
