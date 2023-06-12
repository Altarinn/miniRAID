using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;
using System.Linq;
using miniRAID.ActionHelpers;
using miniRAID.Spells;

namespace miniRAID
{
    public class ExplosiveAttack : ActionDataSO
    {
        public SimpleExplosionFx explosionFx;
        public UnitFilters targetFilter;
        public int explosionRange = 100;
        public SpellDamageHeal explosionDamage;
        
        public override IEnumerator OnPerform(RuntimeAction ract, MobData mob,
            Spells.SpellTarget target)
        {
            Vector3Int origin = mob.Position;

            if (Globals.cc.animation && explosionFx != null)
                yield return new JumpIn(explosionFx.Do(origin));
            
            // Get all enemies
            var capturedMobs = Globals.backend.GetAllMobs()
                .Where(m => targetFilter.Check(mob, m)) // Enemies
                .Where(m => Consts.Distance(m.Position, origin) <= explosionRange); // Within explosion range

            foreach (MobData targetMob in capturedMobs)
            {
                yield return new JumpIn(explosionDamage.Do(
                    ract, mob, targetMob));
            }
        }
    }
}