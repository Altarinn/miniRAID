using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;
using System.Linq;
using miniRAID.Actions;
using miniRAID.Spells;
using miniRAID.Weapon;

namespace miniRAID
{
    public class BattlefieldPray : BasicProjectile
    {
        public override IEnumerator OnPerform(RuntimeAction ract, MobData mob,
            Spells.SpellTarget target)
        {
            (mob.mainWeapon as HeavyWeapon)?.RchargedAttack?.Charge(target, mob);
            yield return new JumpIn(base.OnPerform(ract, mob, new SpellTarget(mob.Position)));
            yield return new JumpIn(mob.SetActive(false));
        }
    }
}
