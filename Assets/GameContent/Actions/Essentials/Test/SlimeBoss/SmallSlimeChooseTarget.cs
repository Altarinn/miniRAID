using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;
using System.Linq;
using miniRAID.ActionHelpers;
using miniRAID.Spells;

namespace miniRAID
{
    public class SmallSlimeChooseTarget : ActionDataSO
    {
        public SpellBuff buff;
        
        public override IEnumerator OnPerform(RuntimeAction ract, MobData mob,
            Spells.SpellTarget target)
        {
            // Get current aggro
            MobData currentAggro = Essentials.MobAtGrid(target.targetPos[0]);
            
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
            
            // Apply the buff as indicator
            yield return new JumpIn(buff.Do(ract, mob, chosenMob));
        }
    }
}
