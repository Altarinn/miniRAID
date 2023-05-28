using UnityEngine;
using System.Collections;
using System;

using Sirenix.OdinInspector;

using miniRAID.Spells;
using System.Collections.Generic;
using System.Linq;
using miniRAID.Backend;
using Unity.VisualScripting;
using XLua;

namespace miniRAID
{
    /// <summary>
    /// MobData is more like a struct that just hold data and have some utility functions.
    /// Unlike "PhaserPlayground", events are all handled in Mob.
    /// This is designed to be used when save / restore current game state ... ?
    /// </summary>
    [Serializable]
    [Inspectable]
    [ParameterDefaultName("mob")]
    [LuaCallCSharp]
    public partial class MobData : BackendState
    {
        public bool enemyDebug = false;
        
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
                if(value != _position && mobRenderer != null)
                {
                    Globals.backend.MoveMob(
                        _position,
                        value,
                        this);
                }
                _position = value; 
            }
        }
        public bool isActive { get; private set; }
        public bool isDead { get; private set; }
        public bool isControllable => isActive && (!isDead);

        [Header("Basic stats")]
        public MovementType movementType;

        public GridShape gridBody;
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
        
        public int VIT => (int)baseStats.VIT.Value;
        public int STR => (int)baseStats.STR.Value;
        public int MAG => (int)baseStats.MAG.Value;
        public int INT => (int)baseStats.INT.Value;
        public int DEX => (int)baseStats.DEX.Value;
        public int TEC => (int)baseStats.TEC.Value;
        
        public int AttackPower => (int)attackPower.Value;
        public int SpellPower => (int)spellPower.Value;
        public int HealPower => (int)healPower.Value;
        public int BuffPower => (int)buffPower.Value;

        public int Def => (int)defense.Value;
        public int SpDef => (int)spDefense.Value;

        public float AggroMul => (float)aggroMul.Value;

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
        public MobRenderer mobRenderer;

        public void Init()
        {
            if (gridBody == null)
            {
                gridBody = new GridShape();
                gridBody.AddGrid(Vector2Int.zero);
            }
            
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
            
            // TODO: FIXME: Why do we need mobRenderer here?
            Databackend.GetSingleton().SetMob(Position, gridBody, this);
            
            // Initial stats calculation
            RecalculateStats();
        }

        private void _SetActionPoints(float v)
        {
            _actionPointsMul100 = Mathf.RoundToInt(v * 100);
        }

        public bool UseActionPoint(float v)
        {
            if (actionPoints >= v)
            {
                _SetActionPoints(actionPoints - v);
                return true;
            }

            return false;
        }

        public void OnWakeUp()
        {
            //_actionPoints = baseActionPoints + extraActionPoints;
            //extraActionPoints = (dNumber)0;

            _actionPointsMul100 += Mathf.RoundToInt(apRecovery * 100);
            if(_actionPointsMul100 > apMax * 100) { _actionPointsMul100 = apMax * 100; }
            GCDstatus.Clear();
            
            mobRenderer?.OnWakeUp();
        }

        public void OnNewPhase()
        {
            mobRenderer?.OnNewPhase();
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
            if (isDead)
            {
                return;
            }
            
            MobListener listener = listenerSO.Wrap(this);
            AddListener(listener, addToList);
        }

        public void AddListener(MobListener listener, bool addToList = true)
        {
            if (isDead)
            {
                return;
            }
            
            if(listener.TryAdd(this))
            {
                listener.OnAttach(this);
                listener.OnEnterScene(this);

                if (addToList)
                {
                    listeners.Add(listener);
                }
            }
        }
        
        public bool AddBuff(Buff.BuffSO buff, MobData source)
        {
            if (isDead)
            {
                return false;
            }
            
            AddListener(buff.Wrap(source));
            return true;
        }

        public bool AddBuff(Buff.Buff buff)
        {
            if (isDead)
            {
                return false;
            }
            
            // TODO: Check if buff duplicated
            AddListener(buff);
            return true;
        }

        public void RemoveListener(MobListenerSO listenerSO)
        {
            listeners
                .FindAll(l => l.data == listenerSO)
                .ForEach(l =>
                {
                    l.OnExitScene(this);
                    l.OnRemove(this);
                });
            listeners.RemoveAll(l => l.data == listenerSO);
        }

        public void RemoveListener(MobListener listener)
        {
            listeners
                .FindAll(l => l == listener)
                .ForEach(l =>
                {
                    l.OnExitScene(this);
                    l.OnRemove(this);
                });
            listeners.RemoveAll(l => l == listener);
        }

        public MobListener FindListener(MobListenerSO data)
        {
            return FindListener(x => x.data == data);
        }

        public MobListener FindListener(Predicate<MobListener> condition)
        {
            // TODO: Optimize me
            return listeners.Find(condition);
        }
        
        public void GetResource(Cost cost)
        {
            switch (cost.type)
            {
                case Cost.Type.AP:
                    Debug.LogWarning("Please implement GetResource for AP.");
                    break;
                case Cost.Type.Mana:
                    Debug.LogWarning("Please implement GetResource for Mana.");
                    break;
            }
            
            return;
        }
        
        public IEnumerator SetActive(bool value)
        {
            if(isActive == value) { yield break; }

            if (value)
            {
                isActive = true;

                OnWakeUp();
                RecalculateStats(); // Do we need it here?
                yield return new JumpIn(OnWakeup?.Invoke(this));

                RecalculateStats();
                yield return new JumpIn(OnAgentWakeUp?.Invoke(this));
            }
            else
            {
                isActive = false;
            }
            
            if (Globals.cc.animation && mobRenderer != null)
                yield return new JumpIn(mobRenderer.SetActiveAnim(value));
        }
        
        public IEnumerator TryAutoEndTurn()
        {
            if(actionPoints <= 0)
            {
                yield return new JumpIn(SetActive(false));
            }
        }

        public IEnumerator _OnNextTurn()
        {
            yield return new JumpIn(OnNextTurn?.Invoke(this));
        }
    }
}
