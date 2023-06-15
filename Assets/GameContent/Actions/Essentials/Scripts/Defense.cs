using System.Collections;
using miniRAID.ActionHelpers;
using miniRAID.Spells;
using UnityEngine;

namespace miniRAID.Actions
{
    public class Defense : BasicProjectile
    {
        public override IEnumerator OnPerform(RuntimeAction ract, MobData mob, SpellTarget targets)
        {
            yield return new JumpIn(base.OnPerform(ract, mob, targets));

            yield return new JumpIn(mob.SetActive(false));
        }
    }
}