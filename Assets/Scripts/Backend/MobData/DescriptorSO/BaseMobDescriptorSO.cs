using System;
using System.Collections.Generic;
using miniRAID.TurnSchedule;
using miniRAID.TurnSchedule.RootAgent;
using Sirenix.OdinInspector;
using UnityEngine;

namespace miniRAID
{
    [Serializable]
    public abstract class BaseMobDescriptorSO : CustomIconScriptableObject
    {
        public enum MovementType
        {
            Walk,
            Fly
        }
        
        [Header("Identification")]
        [PropertyOrder(-3)]
        public string nickname;
        [PropertyOrder(-3)]
        public RaceDescriptorSO race;
        [PropertyOrder(-3)]
        public ClassDescriptorSO job;

        [PropertyOrder(-3)] public Color color;
        
        [Header("Stats")]
        [PropertyOrder(-2)]
        public int level;
        
        [Header("Appearance")]
        public GridCollider gridBody;
        public MobRenderer rendererPrefab;

        [Header("Movement")] 
        public MovementSO movement;
        public bool movable = true;
        public int moveRange = 3;
        
        [Header("Equipments")]
        public Weapon.WeaponSO mainWeaponSO;
        public Weapon.WeaponSO subWeaponSO;
        
        [Header("Listeners")]
        public List<MobListenerSO> listenerSOs = new List<MobListenerSO>();
        
        [Header("Available actions")]
        public ActionSOEntry[] actionSOs;

        [Header("Behaviour")]
        [InlineEditor(InlineEditorObjectFieldModes.Boxed)]
        [ColoredBox("#fff4ac")]
        public MobRootAgentBaseSO rootAgent;

        public virtual void InitializeMobData(MobData mob)
            => InitializeMobData(mob, mob.Position, mob.Collider.Direction);
        
        public virtual void InitializeMobData(MobData mob, Vector3 position, Consts.Direction direction)
        {
            // Sanity check
            if (gridBody == null)
            {
                gridBody = new PointCollider();
            }

            var coll = (GridCollider)gridBody.CloneWithNewGuid();
            coll.Position = position;
            coll.Direction = direction;
            
            mob.Collider = coll;
            
            mob.GCDstatus = new HashSet<GCDGroup>();
            mob.actions = new List<RuntimeAction>();
            mob.availableActions = new HashSet<RuntimeAction>();
            
            // Compute some basic stats for later listeners
            mob.RecalculateStats();

            if (movement != null)
            {
                mob.movement = (Movement)mob.AddAction(new ActionSOEntry(){data = movement, level = 1});
            }

            if (actionSOs != null)
            {
                foreach (var aso in actionSOs)
                {
                    if (aso.data != null)
                    {
                        mob.AddAction(aso);
                    }
                }
            }

            // Duplicate my spells / weapons
            if (mainWeaponSO != null)
            {
                mob.mainWeapon = (Weapon.Weapon)mainWeaponSO.Wrap(mob);
                mob.AddListener(mob.mainWeapon);
            }

            // Register all listeners
            foreach (var l in listenerSOs)
            {
                //MobListenerSO newL = l.Clone();
                mob.AddListener(l);
            }
            
            // Register all listeners in race
            race?.racePassives.ForEach(l => mob.AddListener(l.data.LeveledWrap(mob, l.level)));
            race?.raceActions.ForEach(l => mob.AddAction(l));
            
            // Register all listeners in class
            job?.classPassives.ForEach(l => mob.AddListener(l.data.LeveledWrap(mob, l.level)));
            job?.classActions.ForEach(l => mob.AddAction(l));
            
            // Register root agent
            mob.AssignRootAgent(rootAgent);
        }

        public abstract void RecalculateMobBaseStats(MobData mob);
        public abstract void RecalculateMobBattleStats(MobData mob);

        public MobData Wrap(Vector3Int position, Consts.Direction direction, Consts.UnitGroup group)
        {
            MobData mob = new MobData();
            mob.baseDescriptor = this;
            mob.unitGroup = group;
            mob.Init(position, direction);

            return mob;
        }
    }
}