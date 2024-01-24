using System.Collections;
using miniRAID.ActionHelpers;
using miniRAID.Agents;
using miniRAID.Spells;
using UnityEngine;

namespace miniRAID.Actions
{
    public class Defense : BasicProjectile
    {
        public override IEnumerator OnPerform(RuntimeAction ract, MobData mob, SpellTarget targets)
        {
            yield return new JumpIn(base.OnPerform(ract, mob, targets));
            
            mob.FindListener<PlayerAutoAttackAgentBase>()?.SkipNextTurn();

            yield return new JumpIn(mob.SetActive(false));
        }
    }
}