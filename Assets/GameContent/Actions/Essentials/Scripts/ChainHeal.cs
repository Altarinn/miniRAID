using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;
using System.Linq;
using miniRAID.ActionHelpers;
using miniRAID.Spells;

namespace miniRAID.Actions
{
    public class ChainHeal : ActionDataSO
    {
        public int jumps = 3;
        public int jumpRange = 4;
        public float jumpRatio = 0.7f;
        
        public SpellDamageHeal heal;
        public UnitFilters filter;
        public SimpleRayFx ray;

        public override IEnumerator OnPerform(RuntimeAction ract, MobData mob,
            Spells.SpellTarget targets)
        {
            if (heal.power.type != FloatModifierType.Multiply)
            {
                Debug.LogError("Chain heal must have a multiply power modifier !!\n" +
                               "The behavior of this action is undefined !");
            }
            
            SpellDamageHeal newHeal = new SpellDamageHeal(heal);
            SimpleRay[] rays = new SimpleRay[jumps];
            
            Vector3Int target = targets.targetPos[0];
            Vector3 startPos = Globals.backend.GridToWorldPosCentered(mob.Position);
            MobData dst = Essentials.MobAtGrid(target);

            for (int i = 0; i < jumps; i++)
            {
                // Ray effect
                if (Globals.cc.animation && ray != null)
                    rays[i] = ray.InstantiateRay(
                        startPos, 
                        Globals.backend.GridToWorldPosCentered(dst.Position));
                
                // Heal
                if (heal != null)
                    yield return new JumpIn(newHeal.Do(ract, mob, dst));
                
                // Find next target
                startPos = Globals.backend.GridToWorldPosCentered(dst.Position);
                dst = Globals.backend.allMobs
                    .Where(t => filter.Check(mob, t))
                    .Where(t => Consts.Distance(dst.Position, t.Position) <= jumpRange)
                    .Where(t => t != dst)
                    .OrderBy(Consts.GetPrioritizedHealthRatio)
                    .FirstOrDefault();

                if (dst == null) break;
                
                newHeal.power.value *= jumpRatio;
            }
            
            // Ray effect
            if (Globals.cc.animation && ray != null)
            {
                yield return new WaitForSeconds(0.8f);
                foreach (SimpleRay simpleRay in rays)
                {
                    if (simpleRay != null)
                        simpleRay.Destroy();
                }
            }
        }
    }
}
