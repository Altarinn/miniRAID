using UnityEngine;
using System.Collections;
using System;

using Sirenix.OdinInspector;

using miniRAID.Spells;
using System.Collections.Generic;
using System.Linq;

namespace miniRAID
{
    /// <summary>
    /// MobData is more like a struct that just hold data and have some utility functions.
    /// Unlike "PhaserPlayground", events are all handled in Mob.
    /// This is designed to be used when save / restore current game state ... ?
    /// </summary>
    [Serializable]
    public class MobData
    {
        public enum MovementType
        {
            Walk,
            Fly
        }

        Vector2Int _position;
        public Vector2Int Position
        {
            get { return _position; }
            set 
            {
                if(value != _position && parentMob != null)
                {
                    Globals.backend.MoveMob(
                        _position,
                        value,
                        parentMob);
                }
                _position = value; 
            }
        }
        public bool isActive, isDead;

        [Header("Basic stats")]
        public MovementType movementType;
        public Consts.UnitGroup unitGroup;

        public bool moveable = true;
        public int moveStartCost = 0;

        public int level;

        [Header("Identification")]
        public string nickname;
        public string race;
        public string job;

        [Header("Battle stats")]
        [SerializeField] private int _actionPointsMul100 = 400;
        public float actionPoints { get { return _actionPointsMul100 / 100.0f; } }
        //public dNumber baseActionPoints = (dNumber)4, extraActionPoints = (dNumber)0;
        public float apRecovery = 3;
        public int apMax = 5;

        public dNumber attackPower, spellPower, healPower, buffPower;
        public dNumber defense, spDefense;

        public dNumber hitAcc, dodge;
        public dNumber crit, antiCrit;
        public dNumber extraRange;

        public dNumber aggroMul = (dNumber)1.0f;
        //public float healPriority = 1.0f;

        public HashSet<GCDGroup> GCDstatus;

        public Consts.BaseStatsInt baseStatsFundamental;
        public Consts.BaseStats baseStats;
        public Consts.BattleStats battleStats;

        [Space]
        [ProgressBar(0, "maxHealth")]
        public int health = 100;
        public int maxHealth = 100, damageShield;

        [Header("Equipments")]
        public Weapon.WeaponSO mainWeaponSO;
        public Weapon.WeaponSO subWeaponSO;
        public Weapon.Weapon mainWeapon;
        public Weapon.Weapon subWeapon;

        [Header("Listeners")]
        public List<MobListenerSO> listenerSOs = new List<MobListenerSO>();
        public List<MobListener> listeners = new List<MobListener>();

        [Header("Actions")]
        [Tooltip("Raw action objects.")]
        public ActionDataSO[] actionSOs;
        [HideInInspector]
        public List<RuntimeAction> actions = new List<RuntimeAction>();
        [Tooltip("Runtime action objects; the available actions to the mob now.")]
        public HashSet<RuntimeAction> availableActions = new HashSet<RuntimeAction>();

        //[HideInInspector]
        [NonSerialized]
        public Mob parentMob;

        public void Init(Mob mob)
        {
            GCDstatus = new HashSet<GCDGroup>();
            actions = new List<RuntimeAction>();
            availableActions = new HashSet<RuntimeAction>();

            foreach (var aso in actionSOs)
            {
                AddAction(aso);
            }

            // Duplicate my spells / weapons
            if (mainWeaponSO != null)
            {
                mainWeapon = (Weapon.Weapon)mainWeaponSO.Wrap(this);
                AddListener(mainWeapon);
            }

            // Register all listeners
            foreach (var l in listenerSOs)
            {
                //MobListenerSO newL = l.Clone();
                AddListener(l);
            }
        }

        public RuntimeAction AddAction(ActionDataSO actSO)
        {
            var ract = new RuntimeAction(this, actSO);
            ract.SetData(actSO);
            actions.Add(ract);
            AddListener(ract);

            return ract;
        }

        public void RefreshActions()
        {
            availableActions.Clear();

            for (int i = 0; i < actions.Count; i++)
            {
                actions[i].RecalculateStats(parentMob);
                availableActions.Add(actions[i]);
            }
        }

        public void _SetActionPoints(float v)
        {
            _actionPointsMul100 = Mathf.RoundToInt(v * 100);
        }

        public void OnWakeUp(Mob _)
        {
            //_actionPoints = baseActionPoints + extraActionPoints;
            //extraActionPoints = (dNumber)0;

            _actionPointsMul100 += Mathf.RoundToInt(apRecovery * 100);
            if(_actionPointsMul100 > apMax * 100) { _actionPointsMul100 = apMax * 100; }
            GCDstatus.Clear();
        }

        public void SetGCD(GCDGroup group)
        {
            if(group != GCDGroup.None)
            {
                this.GCDstatus.Add(group);
            }
        }

        public bool IsInGCD(GCDGroup group)
        {
            return this.GCDstatus.Contains(group);
        }

        public void AddListener(MobListenerSO listenerSO, bool addToList = true)
        {
            MobListener listener = listenerSO.Wrap(this);
            AddListener(listener, addToList);
        }

        public void AddListener(MobListener listener, bool addToList = true)
        {
            if(listener.TryAdd(parentMob))
            {
                listener.OnAttach(parentMob);
                listener.OnEnterScene(parentMob);

                if (addToList)
                {
                    listeners.Add(listener);
                }
            }
        }

        public void RemoveListener(MobListenerSO listenerSO)
        {
            listeners
                .FindAll(l => l.data == listenerSO)
                .ForEach(l =>
                {
                    l.OnExitScene(parentMob);
                    l.OnRemove(parentMob);
                });
            listeners.RemoveAll(l => l.data == listenerSO);
        }

        public void RemoveListener(MobListener listener)
        {
            listeners
                .FindAll(l => l == listener)
                .ForEach(l =>
                {
                    l.OnExitScene(parentMob);
                    l.OnRemove(parentMob);
                });
            listeners.RemoveAll(l => l == listener);
        }
    }
}
