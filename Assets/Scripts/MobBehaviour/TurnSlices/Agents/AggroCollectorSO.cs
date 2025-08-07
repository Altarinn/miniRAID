using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using miniRAID.Actions;
using miniRAID.Backend;
using miniRAID.Spells;
using miniRAID.TurnSchedule;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Serialization;

namespace miniRAID.Agents
{
    public class AggroCollectorSO : MobListenerSO
    {
        [Header("User Interface")] public Material decalIndicatorMaterial;
        
        public override MobListener Wrap(MobData parent)
        {
            return new AggroCollector(parent, this);
        }
    }

    public abstract class TargetIndicator : MobListener
    {
        protected TargetIndicator(MobData parent, MobListenerSO data) : base(parent, data)
        {
        }

        public virtual MobData CurrentTarget => null;
    }

    public class AggroCollector : TargetIndicator, IRenderableState
    {
        public AggroCollector(MobData parent, MobListenerSO data) : base(parent, data)
        {
        }

        public Dictionary<MobData, float> AggroList => aggroList; // TODO: Readonly?
        [OdinSerialize] protected Dictionary<MobData, float> aggroList;
        public float maxAggro => aggroList.Max(kv => kv.Value);
        public AggroCollectorSO aggroCollectorData => (AggroCollectorSO)data;

        [OdinSerialize] protected MobData currentTarget;

        public override MobData CurrentTarget
        {
            get
            {
                UpdateAggro();
                return currentTarget;
            }
        }
        
        public bool useAggro = true;

        private DecalIndicator targetIndicator => (DecalIndicator)renderer;

        public override void OnAttach(MobData mob)
        {
            base.OnAttach(mob);
            
            mob.OnDamageReceived.AddListener(Mob_OnReceiveDamageFinal);
            mob.OnRecoveryStage.AddListener(Mob_OnRecoveryStage);
            aggroList = new Dictionary<MobData, float>();
        }

        public override void OnRemove(MobData mob)
        {
            base.OnRemove(mob);

            mob.OnDamageReceived.RemoveListener(Mob_OnReceiveDamageFinal);
            mob.OnRecoveryStage.RemoveListener(Mob_OnRecoveryStage);

            foreach (MobData targetMob in aggroList.Keys)
            {
                targetMob.OnHealReceived.RemoveListener(TargetMob_OnReceiveHealFinal);
            }
        }

        public IEnumerator Mob_OnRecoveryStage(MobData mob)
        {
            foreach (var entry in aggroList.Keys.ToList())
            {
                // Decrease aggro by some constant per turn
                aggroList[entry] *= Consts.AggroDecay;
            }
            UpdateAggro();
            
            yield break;
        }
        
        public void SetRandomAggro(float magnitude)
        {
            aggroList.Clear();
            foreach (var mob in Globals.backend.GetAllMobs())
            {
                if (Consts.ApplyMask(Consts.EnemyMask(parentMob.unitGroup), mob.unitGroup))
                {
                    aggroList.Add(mob, Globals.cc.rng.NextFloat() * magnitude);
                }
            }
        }

        private IEnumerator Mob_OnReceiveDamageFinal(MobData mob, Consts.DamageHeal_Result info)
        {
            if (!useAggro || info.source.unitGroup == mob.unitGroup) { yield break; }
            AddToAggro(info.source, info.value * info.source.aggroMul);
        }

        private IEnumerator TargetMob_OnReceiveHealFinal(MobData mob, Consts.DamageHeal_Result info)
        {
            // TODO: Why?
            if (!useAggro || parentMob == null || info.source == null) { yield break; }
            
            if (info.source.unitGroup == parentMob.unitGroup) { yield break; }
            AddToAggro(info.source, info.value * info.source.aggroMul * Consts.HealAggroMul);
        }

        protected void AddToAggro(MobData mob, float aggro)
        {
            if (!useAggro) { return; }
            
            // Add to aggrolist if not exist
            if (!aggroList.ContainsKey(mob))
            {
                aggroList.Add(mob, 0);
                mob.OnHealReceived.AddListener(TargetMob_OnReceiveHealFinal);
                // TODO: Buffs
            }

            aggroList[mob] += aggro;
            
            // Check if aggro exceeded the threshold
            if (mob != currentTarget)
            {
                if (!(mob == null || parentMob == null))
                {
                    if (aggroList[mob] >= maxAggro)
                    {
                        Globals.popupMgr.Instance.Popup(">TARGET<", Globals.backend.BackendToRenderPosCentered(mob.Position), Consts.BuffColor);
                        Globals.popupMgr.Instance.Popup("ATTACKING YOU", Globals.backend.BackendToRenderPosCentered(parentMob.Position), Consts.BuffColor);
                    }
                    else if (aggroList[mob] >= maxAggro * Settings.highAggroThreshold)
                    {
                        Globals.popupMgr.Instance.Popup("!", Globals.backend.BackendToRenderPosCentered(mob.Position), Consts.BuffColor);
                        Globals.popupMgr.Instance.Popup("HIGH AGGRO", Globals.backend.BackendToRenderPosCentered(parentMob.Position), Consts.BuffColor);
                    }
                }
            }
            
            UpdateAggro();
        }

        protected void RemoveFromAggro(MobData mob)
        {
            if (aggroList.ContainsKey(mob))
            {
                aggroList.Remove(mob);
                mob.OnHealReceived.RemoveListener(TargetMob_OnReceiveHealFinal);
                // TODO: Buffs
            }
            UpdateAggro();
        }

        public void UpdateAggro()
        {
            if (useAggro)
            {
                // Find our target first.
                // Target may die due to previous action, so we may need to find a new one.
                MobData target = null;
                float maxAggro = 0;

                List<MobData> shouldRemove = new List<MobData>();

                foreach (var entry in aggroList)
                {
                    // Remove from aggroList if aggro too small
                    // TODO: Check if dead or out of range
                    if (entry.Key == null || entry.Key.isDead)
                    {
                        shouldRemove.Add(entry.Key);
                        continue;
                    }
                    
                    if (aggroList[entry.Key] <= 1.0)
                    {
                        shouldRemove.Add(entry.Key);
                        continue;
                    }

                    if (target == null) { target = entry.Key; }
                    if (entry.Value > maxAggro)
                    {
                        target = entry.Key;
                        maxAggro = entry.Value;
                    }
                }

                foreach (var item in shouldRemove)
                {
                    RemoveFromAggro(item);
                }
                
                currentTarget = target;
                UpdateRenderer();
            }
        }

        public struct AggroInfo
        {
            [DisplayAsString]
            public string nickname;

            [DisplayAsString]
            public float aggro;
        }
        
        public List<AggroInfo> GetAggroListUtil()
        {
            return aggroList.Select(kv => new KeyValuePair<string, float>(kv.Key.nickname, kv.Value))
                .OrderByDescending(kv => kv.Value)
                .Select(kv => new AggroInfo() { nickname = kv.Key, aggro = kv.Value })
                .ToList();
        }

        public void SetAsMaxAggro(MobData target, float margin)
        {
            aggroList[target] = 0;
            AddToAggro(target, maxAggro + margin);
        }

        public void ConstructRenderer()
        {
            if (aggroCollectorData.decalIndicatorMaterial)
            {
                renderer = DecalIndicator.Instantiate(
                    aggroCollectorData.decalIndicatorMaterial,
                    Globals.backend.BackendToRenderPosCenteredGrounded(parentMob.Position));
            }
        }

        public void UpdateRenderer()
        {
            if (targetIndicator != null)
            {
                targetIndicator.Follow(currentTarget);
            }
        }
    }
}