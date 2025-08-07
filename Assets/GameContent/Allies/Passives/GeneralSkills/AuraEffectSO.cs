using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using miniRAID;
using miniRAID.ActionHelpers;
using miniRAID.Buff;
using Sirenix.Serialization;

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

    // TODO: Perhaps can change me to collider based?
    public class AuraEffectSORuntimeBuff : Buff
    {
        [OdinSerialize]
        private HashSet<MobData> validTargets;
        
        [OdinSerialize]
        private Dictionary<MobData, AuraEffectSORuntimeBuff> activeMobs;
        
        public AuraEffectSORuntimeBuff(MobData source, AuraEffectSO data) : base(source, data)
        {}

        public override void OnAttach(MobData mob)
        {
            base.OnAttach(mob);

            // Source of the aura; Do the check for everything
            if (mob == source)
            {
                validTargets = new();
                activeMobs = new();
                
                // First get all available mobs
                var targetList = Globals.backend.GetAllMobs()
                    .Where(m => m != source)
                    .Where(m => ((AuraEffectSO)buffData).filter.Check(source, m))
                    .ToList();

                // Register listeners to all valid targets
                source.OnMobMoved.AddListener(SourceOnMobMoved);
                targetList.ForEach(OnMobAdded);

                // Register listener to any new mobs etc.
                Globals.backend.onMobAdded.AddListener(OnMobAdded);
                Globals.backend.onMobRemoved.AddListener(OnMobRemoved);
            }
        }

        protected override void OnRemoveFromMob(MobData mob)
        {
            if (mob == source)
            {
                Globals.backend.onMobAdded.RemoveListener(OnMobAdded);
                Globals.backend.onMobRemoved.RemoveListener(OnMobRemoved);
                
                validTargets.ToList().ForEach(OnMobRemoved);
                mob.OnMobMoved.RemoveListener(SourceOnMobMoved);
            }
        }

        protected void OnMobAdded(MobData mob)
        {
            if (mob != source && ((AuraEffectSO)buffData).filter.Check(source, mob))
            {
                validTargets.Add(mob);
                mob.OnMobMoved.AddListener(TargetOnMobMovedCoroutine);
                TargetOnMobMoved(mob, mob.GridPosition);
            }
        }
        
        protected void OnMobRemoved(MobData mob)
        {
            if (validTargets.Contains(mob))
            {
                DeactivateAuraOnMob(mob);
                mob.OnMobMoved.RemoveListener(TargetOnMobMovedCoroutine);
                validTargets.Remove(mob);
            }
        }

        protected IEnumerator SourceOnMobMoved(MobData mob, Vector3Int from)
        {
            foreach (var target in validTargets)
            {
                TargetOnMobMoved(target, target.GridPosition);
            }

            yield break;
        }

        protected IEnumerator TargetOnMobMovedCoroutine(MobData mob, Vector3Int from)
        {
            TargetOnMobMoved(mob, from);
            yield break;
        }

        protected void TargetOnMobMoved(MobData mob, Vector3Int from)
        {
            // Ignore non-target mobs. Should not be triggered tho?
            if (!((AuraEffectSO)buffData).filter.Check(source, mob))
            {
                return;
            }
            
            // We want to remove the aura
            if (Consts.Distance(source.Position, mob.Position) > ((AuraEffectSO)buffData).range)
            {
                DeactivateAuraOnMob(mob);
            }
            // We want to add the aura
            else
            {
                ActivateAuraOnMob(mob);
            }
        }

        private void ActivateAuraOnMob(MobData mob)
        {
            if (activeMobs.ContainsKey(mob))
            {
                return;
            }

            var copied = mob.AddBuff((AuraEffectSO)buffData, this.level, source);
            activeMobs.Add(mob, (AuraEffectSORuntimeBuff)copied);
        }

        private void DeactivateAuraOnMob(MobData mob)
        {
            if (!activeMobs.ContainsKey(mob))
            {
                return;
            }

            mob.RemoveListener(activeMobs[mob]);
            activeMobs.Remove(mob);
        }
    }
}