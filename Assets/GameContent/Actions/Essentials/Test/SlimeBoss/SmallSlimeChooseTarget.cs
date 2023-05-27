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
        
        public override IEnumerator OnPerform(RuntimeAction ract, Mob mob,
            Spells.SpellTarget target)
        {
            // Get current aggro
            Mob currentAggro = Essentials.MobAtGrid(target.targetPos[0]);
            
            // Get all valid targets
            var enemies = Globals.backend.GetAllMobs()
                .Where(m => Consts.ApplyMask(Consts.EnemyMask(mob.data.unitGroup), m.data.unitGroup)) // Choose from enemies
                .Where(m => m != currentAggro) // Don't choose current aggro
                .Where(m => m.data.FindListener(buff.buff) == null) // Don't choose target repeatedly
                .ToList();
            
            if(enemies.Count <= 0){ yield break; }
            
            // Find random target
            int idx = Random.Range(0, enemies.Count);
            Mob chosenMob = enemies[idx];
            
            // Apply the buff as indicator
            yield return new JumpIn(buff.Do(ract, mob, chosenMob));
        }
    }
}
