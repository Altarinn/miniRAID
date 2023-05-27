using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;
using System.Linq;
using miniRAID.ActionHelpers;
using miniRAID.Agents;
using miniRAID.Buff;
using miniRAID.Spells;

namespace miniRAID
{
    public class SmallSlimes : ActionDataSO
    {
        public BuffSO indicatorBuff;
        public ActionHelpers.Projectile smallSlimeProjectile;
        public PlayFx onHitFx, smallSlimeSpawnFx;
        public SpellDamageHeal damage;
        public Summon<LockTargetAgent> summon;
        public CreateGridEffect poisonPool;
        
        public override IEnumerator OnPerform(RuntimeAction ract, Mob mob,
            Spells.SpellTarget _)
        {
            // Get all valid targets
            var targets = Globals.backend.GetAllMobs()
                .Where(m => Consts.ApplyMask(Consts.EnemyMask(mob.data.unitGroup), m.data.unitGroup)) // Choose from enemies
                .Where(m => m.data.FindListener(indicatorBuff) != null) // Choose the targets we have marked before
                .ToList();
            
            if(targets.Count <= 0){ yield break; }
            
            // Throw slimes!
            foreach (var target in targets)
            {
                // Shoot
                yield return new JumpIn(smallSlimeProjectile.WaitForShootAt(mob, target.data.Position));
                yield return new JumpIn(onHitFx.Do(target.data.Position));
                yield return new JumpIn(damage.Do(ract, mob, target));
                
                // Remove the indicator buff
                target.data.RemoveListener(indicatorBuff);

                // Find a proper position to spawn slime
                Vector2Int spawnPos = Globals.backend.FindNearestEmptyGrid(target.data.Position);
                if (Globals.backend.InMap(spawnPos))
                {
                    yield return new JumpIn(smallSlimeSpawnFx.Do(spawnPos));
                    
                    // Spawn
                    LockTargetAgent summonedMob = summon.Do(spawnPos, false).GetComponent<LockTargetAgent>();
                    summonedMob.target = mob;

                    // Generate poison pool
                    yield return new JumpIn(poisonPool.Do(ract, mob, target.data.Position));
                }
            }
        }
    }
}
