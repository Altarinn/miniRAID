using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using miniRAID.ActionHelpers;
using miniRAID.Spells;

namespace miniRAID
{
    public class SmallSlimeChooseTarget : ActionDataSO<SingleMobTarget>
    {
        public SpellBuff buff;
        public ActionHelpers.Projectile indicator;

        public override IEnumerator OnPerform(RuntimeAction<SingleMobTarget> ract, MobData mob,
            SingleMobTarget target)
        {
            // Get current aggro
            MobData currentAggro = target.Target;
            
            // Get all valid targets
            var enemies = Globals.backend.GetAllMobs()
                .Where(m => Consts.ApplyMask(Consts.EnemyMask(mob.unitGroup), m.unitGroup)) // Choose from enemies
                .Where(m => m != currentAggro) // Don't choose current aggro
                .Where(m => m.FindListener(buff.buff) == null) // Don't choose target repeatedly
                .ToList();
            
            if(enemies.Count <= 0){ yield break; }
            
            // Find random target
            int idx = Random.Range(0, enemies.Count);
            MobData chosenMob = enemies[idx];

            yield return new JumpIn(indicator.WaitForShootAt(mob, chosenMob.Position));
            
            // Apply the buff as indicator
            yield return new JumpIn(buff.Do(ract, mob, chosenMob));
        }
    }
}
