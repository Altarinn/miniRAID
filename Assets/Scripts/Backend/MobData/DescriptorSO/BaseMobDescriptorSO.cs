using System;
using System.Collections.Generic;
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
        
        [Header("Stats")]
        [PropertyOrder(-2)]
        public int level;
        
        [Header("Appearance")]
        public GridShape gridBody;
        public MobRenderer rendererPrefab;

        [Header("Movement")]
        public MovementType movementType;
        public bool movable = true;
        public int moveRange = 3;
        
        [Header("Equipments")]
        public Weapon.WeaponSO mainWeaponSO;
        public Weapon.WeaponSO subWeaponSO;
        
        [Header("Listeners")]
        public List<MobListenerSO> listenerSOs = new List<MobListenerSO>();
        
        [Header("Available actions")]
        public ActionSOEntry[] actionSOs;

        public virtual void InitializeMobData(MobData mob)
        {
            // Sanity check
            if (gridBody == null)
            {
                gridBody = new GridShape();
                gridBody.AddGrid(Vector3Int.zero);
            }

            mob.gridBody = new GridShape();
            foreach (var g in gridBody.shape)
            {
                mob.gridBody.AddGrid(g);
            }
            
            mob.GCDstatus = new HashSet<GCDGroup>();
            mob.actions = new List<RuntimeAction>();
            mob.availableActions = new HashSet<RuntimeAction>();
            
            // Compute some basic stats for later listeners
            mob.RecalculateStats();

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
        }

        public abstract void RecalculateMobBaseStats(MobData mob);
        public abstract void RecalculateMobBattleStats(MobData mob);
    }
}