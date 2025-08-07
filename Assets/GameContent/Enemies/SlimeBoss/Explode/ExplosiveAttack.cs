using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;
using System.Linq;
using miniRAID.ActionHelpers;
using miniRAID.Spells;

namespace miniRAID
{
    public class ExplosiveAttack : ActionDataSO<SingleMobTarget>
    {
        public SimpleExplosionFx explosionFx;
        public UnitFilters targetFilter;
        public int explosionRange = 100;
        public SpellDamageHeal explosionDamage;
        public SpellBuff explosionBuff;
        
        public override IEnumerator OnPerform(RuntimeAction<SingleMobTarget> ract, MobData mob,
            SingleMobTarget target)
        {
            Vector3 origin = mob.Position;

            if (Globals.cc.animation && explosionFx != null)
                yield return new JumpIn(explosionFx.Do(origin));
            
            // Get all enemies
            var capturedMobs = Globals.backend.GetAllMobs()
                .Where(m => targetFilter.Check(mob, m)) // Enemies
                .Where(m => Consts.Distance(m.Position, origin) <= explosionRange); // Within explosion range

            yield return new JumpIn(
                MobListHelpers.WaitForAllMobs(capturedMobs, m => JumpInHelper.Chain(
                    explosionDamage?.Do(ract, mob, m),
                    explosionBuff?.Do(ract, mob, m))
                )
            );
        }
    }
}