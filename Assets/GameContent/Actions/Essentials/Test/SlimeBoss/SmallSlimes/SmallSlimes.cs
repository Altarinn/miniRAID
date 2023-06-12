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
    // TODO: Make me to MobData-based (summon) before actual contents (post prototyping) !!
    public class SmallSlimes : ActionDataSO
    {
        public BuffSO indicatorBuff;
        public ActionHelpers.Projectile smallSlimeProjectile;
        public SimpleExplosionFx onHitFx, smallSlimeSpawnFx;
        public SpellDamageHeal damage;
        public Summon<LockTargetAgent> summon;
        public CreateGridEffect poisonPool;
        
        public override IEnumerator OnPerform(RuntimeAction ract, MobData mob,
            Spells.SpellTarget _)
        {
            // Get all valid targets
            var targets = Globals.backend.GetAllMobs()
                .Where(m => Consts.ApplyMask(Consts.EnemyMask(mob.unitGroup), m.unitGroup)) // Choose from enemies
                .Where(m => m.FindListener(indicatorBuff) != null) // Choose the targets we have marked before
                .ToList();
            
            if(targets.Count <= 0){ yield break; }
            
            // Throw slimes!
            foreach (var target in targets)
            {
                // Shoot
                yield return new JumpIn(smallSlimeProjectile.WaitForShootAt(mob, target.Position));
                yield return new JumpIn(onHitFx.Do(target.Position));
                yield return new JumpIn(damage.Do(ract, mob, target));
                
                // Remove the indicator buff
                target.RemoveListener(indicatorBuff);

                // Find a proper position to spawn slime
                Vector3Int spawnPos = Globals.backend.FindNearestEmptyGrid(target.Position);
                if (Globals.backend.InMap(spawnPos))
                {
                    yield return new JumpIn(smallSlimeSpawnFx.Do(spawnPos));
                    
                    // Spawn
                    LockTargetAgent summonedMob = summon.Do(spawnPos, false).GetComponent<LockTargetAgent>();
                    summonedMob.target = mob;

                    // Generate poison pool
                    yield return new JumpIn(poisonPool.Do(ract, mob, target.Position));
                }
            }
        }
    }
}
