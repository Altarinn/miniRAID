using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using miniRAID;
using miniRAID.ActionHelpers;
using miniRAID.Buff;

namespace GameContent.Buffs.Test
{
    public class AuraEffectSO : PassiveSkillSO
    {
        public UnitFilters filter;
        public int range;
        
        public override MobListener Wrap(MobData parent)
        {
            return new AuraEffectSORuntimeBuff(parent, this);
        }
    }

    public class AuraEffectSORuntimeBuff : Buff
    {
        private List<MobData> validTargets;
        private Dictionary<MobData, AuraEffectSORuntimeBuff> activeMobs;
        
        public AuraEffectSORuntimeBuff(MobData source, AuraEffectSO data) : base(source, data)
        {}

        public override void OnAttach(MobData mob)
        {
            base.OnAttach(mob);

            // Source of the aura; Do the check for everything
            if (mob == source)
            {
                activeMobs = new();
                
                // First get all available mobs
                validTargets = Globals.backend.GetAllMobs()
                    .Where(m => m != source)
                    .Where(m => ((AuraEffectSO)buffData).filter.Check(source, m))
                    .ToList();
                
                // Register events to all valid targets
                source.OnMobMoved += SourceOnMobMoved;
                foreach (var target in validTargets)
                {
                    target.OnMobMoved += TargetOnMobMoved;
                }

                onRemoveFromMob += m =>
                {
                    m.OnMobMoved -= SourceOnMobMoved;
                    foreach (var target in validTargets)
                    {
                        target.OnMobMoved -= TargetOnMobMoved;
                    }
                };
            }
        }

        private void SourceOnMobMoved(MobData mob)
        {
            foreach (var target in validTargets)
            {
                TargetOnMobMoved(target);
            }
        }

        private void TargetOnMobMoved(MobData mob)
        {
            // Ignore non-target mobs. Should not be triggered tho?
            if (!((AuraEffectSO)buffData).filter.Check(source, mob))
            {
                return;
            }
            
            // We want to remove the aura
            if (Consts.Distance(source.Position, mob.Position) > ((AuraEffectSO)buffData).range)
            {
                if (!activeMobs.ContainsKey(mob))
                {
                    return;
                }
                
                mob.RemoveListener(activeMobs[mob]);
                activeMobs.Remove(mob);
            }
            // We want to add the aura
            else
            {
                if (activeMobs.ContainsKey(mob))
                {
                    return;
                }
                
                var copied = mob.AddBuff((AuraEffectSO)buffData, source);
                activeMobs.Add(mob, (AuraEffectSORuntimeBuff)copied);
            }
        }
    }
}