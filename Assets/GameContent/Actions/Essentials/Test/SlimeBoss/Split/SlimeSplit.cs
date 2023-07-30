using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;
using System.Linq;
using miniRAID.ActionHelpers;
using miniRAID.Agents;
using miniRAID.Spells;

namespace miniRAID
{
    public class SlimeSplit : ActionDataSO
    {
        public Vector3Int summonOffset;
        public float healthRatio;
        
        public SimpleExplosionFx onHitFx, slimeSpawnFx;
        public Summon<MobRenderer> summon;
        public ShowImportantMessage message;
        
        public override IEnumerator OnPerform(RuntimeAction ract, MobData mob,
            Spells.SpellTarget target)
        {
            yield return new JumpIn(message.Do());
            
            for (int s = -1; s <= 1; s += 2)
            {
                Vector3Int spawnPos = mob.Position + summonOffset * s;
                spawnPos.Clamp(Vector3Int.zero, new Vector3Int(
                    Globals.backend.mapSizeX - 1,
                    Globals.backend.mapHeight - 1,
                    Globals.backend.mapSizeZ - 1));
                
                // Spawn myself
                MobRenderer duplicatedSelf = summon.Do(spawnPos, true);
                var currentProgress = mob.FindListener<BasicPhasedBossAgent>().GetProgress();
                currentProgress.turnInPhase += 1;

                duplicatedSelf.data.OnInitialized += data =>
                {
                    data.health = Mathf.CeilToInt(data.maxHealth * healthRatio);
                    data.nickname = mob.nickname;
                    
                    data.FindListener<BasicPhasedBossAgent>().CopyFrom(currentProgress);
                    data.FindListener<AggroAgentBase>().SetRandomAggro(10.0f);
                    
                    data.SetInactiveImmediately();
                };
            }

            yield return new JumpIn(mob.Kill());

            yield break;
        }
    }
}
