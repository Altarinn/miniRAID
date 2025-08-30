using UnityEngine;
using System.Collections;
using System;

using Sirenix.OdinInspector;

using miniRAID.Spells;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using miniRAID.Backend;
using miniRAID.TurnSchedule;
using miniRAID.TurnSchedule.RootAgent;
using Sirenix.Serialization;
using UnityEngine.Serialization;

namespace miniRAID
{
    public enum GCDGroup
    {
        None,
        Common,
        RegularAttack,
        Special
    }
    
    /// <summary>
    /// MobData is more like a struct that just hold data and have some utility functions.
    /// Unlike "PhaserPlayground", events are all handled in Mob.
    /// This is designed to be used when save / restore current game state ... ?
    /// </summary>
    [Serializable]
    public partial class MobData : BackendState, IColliderState, IRenderableState
    {
        [InlineEditor(InlineEditorObjectFieldModes.Boxed)]
        public BaseMobDescriptorSO baseDescriptor;
        public bool enemyDebug = false;
        public bool IsInWorld => World != null;
        public Databackend World { get; private set; }

        public enum MovementType
        {
            Walk,
            Fly
        }

        // This (float) is accurate as long as non-solid blocks have 2^N steps (e.g., 8.)
        public GridCollider Collider { get => _collider; set => _collider = value; }
        [OdinSerialize] private GridCollider _collider;

        public Vector3 Position
        {
            get { return Collider.Position; }
            private set 
            {
                if(value != Collider.Position)
                {
                    Globals.backend.MoveMob(
                        Collider.Position,
                        value,
                        this);
                    UpdateRenderer();
                }
            }
        }

        public Vector3 SpellCastPivot
        {
            get { return Collider.Position + Vector3.one * 0.5f; }
        }

        public void OnEnterCollider(BackendState other)
        {
            // TODO: Do nothing?
            return;
        }

        public void OnExitCollider(BackendState other)
        {
            // TODO: Do nothing?
            return;
        }

        public Vector3Int GridPosition => Databackend.BackendToGridPos(Position);

        public void AddedToWorld(Databackend world)
        {
            World = world;
        }
        
        public void RemovedFromWorld(Databackend world)
        {
            if (World == world)
            {
                World = null;
            }
        }

        public IEnumerator SetPosition(Vector3Int position)
        {
            var prevPos = Position;
            Position = Globals.backend.GridToBackendFloorPos(position);
            yield return new JumpIn(OnMobMoved?.InvokeCoroutine(this, prevPos));
        }
        
        [OdinSerialize] public bool isActive { get; private set; }
        [OdinSerialize] public bool isDead { get; private set; }
        [OdinSerialize] public bool skipAutoAttack { get; private set; }
        
        public bool isControllable => isActive && (!isDead);
        public bool canAutoAttack => (!skipAutoAttack) && (!isDead);

        public bool initialized = false;

        [Header("Basic stats")]
        public MovementType movementType;

        public Consts.UnitGroup unitGroup;

        public int level => baseDescriptor.level;

        [Header("Identification")]
        public string nickname;
        public string race => baseDescriptor.race?.raceName;
        public string job => baseDescriptor.job?.className;

        [Header("Battle stats")]
        [SerializeField] private int _actionPointsMul100 = 400;

        [SerializeField] private int _freeActionPointsMul100 = 100;
        public float actionPoints { get { return (_actionPointsMul100 + _freeActionPointsMul100) / 100.0f; } }
        public float freeActionPoints { get { return _freeActionPointsMul100 / 100.0f; } }
        //public dNumber baseActionPoints = (dNumber)4, extraActionPoints = (dNumber)0;
        [NonSerialized] [ShowInInspector] public float apRecovery = 3;
        [NonSerialized] [ShowInInspector] [FormerlySerializedAs("apMax")] public int apNonFreeMax = 5;

        public float movedGrids = 0;
        
        [NonSerialized] [ShowInInspector] public dNumber moveRange;
        [NonSerialized] [ShowInInspector] public dNumber attackPower;
        [NonSerialized] [ShowInInspector] public dNumber spellPower;
        [NonSerialized] [ShowInInspector] public dNumber healPower;
        [NonSerialized] [ShowInInspector] public dNumber buffPower;
        [NonSerialized] [ShowInInspector] public dNumber defense;
        [NonSerialized] [ShowInInspector] public dNumber spDefense;

        [NonSerialized] [ShowInInspector] public dNumber hitAcc;
        [NonSerialized] [ShowInInspector] public dNumber dodge;
        [NonSerialized] [ShowInInspector] public dNumber crit;
        [NonSerialized] [ShowInInspector] public dNumber antiCrit;
        
        [NonSerialized] [ShowInInspector] public dNumber aggroMul = (dNumber)1.0f;
        [NonSerialized] [ShowInInspector] public float healPriority = 1.0f;
        
        // TODO: Me?
        public dNumber extraRange;

        public HashSet<GCDGroup> GCDstatus;

        [NonSerialized] [ShowInInspector] public Consts.BaseStats baseStats;
        [NonSerialized] [ShowInInspector] public Consts.BattleStats battleStats;
        
        public int VIT => (int)baseStats.VIT.Value;
        public int STR => (int)baseStats.STR.Value;
        public int MAG => (int)baseStats.MAG.Value;
        public int INT => (int)baseStats.INT.Value;
        public int AGI => (int)baseStats.AGI.Value;
        public int TEC => (int)baseStats.TEC.Value;

        public int MoveRange => (int)moveRange.Value;
        public float MoveRangeLeft => MoveRange - movedGrids;
        
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
        public Weapon.Weapon mainWeapon;
        public Weapon.Weapon subWeapon;

        [Header("Listeners")]
        public List<MobListener> listeners = new List<MobListener>();

        [Header("Actions")]
        [HideInInspector]
        public List<RuntimeAction> actions = new List<RuntimeAction>();
        [Tooltip("Runtime action objects; the available actions to the mob now.")]
        public HashSet<RuntimeAction> availableActions = new HashSet<RuntimeAction>();

        [Header("Behaviour")]
        [SerializeField]
        private MobRootAgentBase rootAgent;

        [HideInInspector]
        public MobRenderer mobRenderer => (MobRenderer)renderer;

        public void Init(Vector3 position, Consts.Direction direction)
        {
            baseDescriptor.InitializeMobData(this, position, direction);

            // Initial stats calculation
            RecalculateStats();

            initialized = true;

            Databackend.GetSingleton().SetMob(this);
            Register();

            OnInitialized?.InvokeInstant(this);
        }

        public bool UseActionPoint(float v)
        {
            // TODO: Emit events here
            
            int vMul100 = Mathf.FloorToInt(v * 100);
            int freeUsage = Mathf.Min(vMul100, _freeActionPointsMul100);
            vMul100 -= freeUsage;
            _freeActionPointsMul100 -= freeUsage;

            if (vMul100 <= 0)
            {
                return true;
            }
            
            if (_actionPointsMul100 >= vMul100)
            {
                _actionPointsMul100 -= vMul100;
                return true;
            }

            return false;
        }

        public void OnWakeUp()
        {
            GCDstatus.Clear();
            actedThisTurn = false;
            movedGrids = 0;
            
            if (isDead)
            {
                return;
            }
            
            //_actionPoints = baseActionPoints + extraActionPoints;
            //extraActionPoints = (dNumber)0;

            mobRenderer?.OnWakeUp();
        }

        public void Recover()
        {
            _freeActionPointsMul100 = Consts.freeAP * 100;
            
            _actionPointsMul100 += Mathf.RoundToInt(apRecovery * 100);
            if(_actionPointsMul100 > apNonFreeMax * 100) { _actionPointsMul100 = apNonFreeMax * 100; }
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

        public MobListener AddListener(MobListenerSO listenerSO, bool addToList = true)
        {
            if (isDead)
            {
                return null;
            }
            
            MobListener listener = listenerSO.Wrap(this);
            return AddListener(listener, addToList);
        }

        public MobListener AddListener(MobListener listener, bool addToList = true)
        {
            if (isDead)
            {
                return null;
            }

            MobListener l = null;
            
            if(listener.TryAdd(this))
            {
                listener.OnAttach(this);
                listener.OnEnterScene(this);

                if (addToList)
                {
                    listeners.Add(listener);
                }

                l = listener;
            }

            if (initialized)
            {
                RecalculateStats();
            }

            return l;
        }
        
        public Buff.Buff AddBuff(Buff.BuffSO buff, int level, MobData source)
        {
            if (isDead)
            {
                return null;
            }
            
            return (Buff.Buff)AddListener(buff.LeveledWrap(source, level));
        }

        public Buff.Buff AddBuff(Buff.Buff buff)
        {
            if (isDead)
            {
                return null;
            }
            
            // TODO: Check if buff duplicated
            return (Buff.Buff)AddListener(buff);
        }
        
        public void RemoveBuffOnce(Buff.BuffSO buffData, int stacks = 1)
        {
            listeners
                .FindAll(l => l.data == buffData)
                .ForEach(l => (l as Buff.Buff)?.RemoveStacks(stacks));
        }

        public void RemoveBuffOnce(Buff.Buff buff, int stacks = 1)
        {
            listeners
                .FindAll(l => buff.IsDuplicated(l as Buff.Buff))
                .ForEach(l => (l as Buff.Buff)?.RemoveStacks(stacks));
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
        
        public T FindListener<T>() where T : MobListener
        {
            return (T)(FindListener(x => x is T));
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
                    // Debug.LogWarning("Please implement GetResource for AP.");
                    _actionPointsMul100 += Mathf.RoundToInt(cost.value * 100);
                    if(_actionPointsMul100 > apNonFreeMax * 100) { _actionPointsMul100 = apNonFreeMax * 100; }
                    break;
                case Cost.Type.Mana:
                    Debug.LogWarning("Please implement GetResource for Mana.");
                    break;
            }
            
            return;
        }

        public IEnumerator EnterStrategyPhase()
        {
            yield return new JumpIn(SetActive(false));
            // yield break;
        }
        
        public IEnumerator SetActive(bool value)
        {
            if(isActive == value) { yield break; }
            
            Globals.logger?.Log($"[SetActive] {nickname}: {value}");

            if (value)
            {
                isActive = true;

                OnWakeUp();
                RecalculateStats(); // Do we need it here? <- Yes (e.g., ChargedAction will modify AP regen here)
                yield return new JumpIn(OnWakeup?.InvokeCoroutine(this));
            }
            else
            {
                yield return new JumpIn(OnPassed?.InvokeCoroutine(this));
                SetInactiveImmediately();
            }
            
            if (Globals.cc.animation && mobRenderer != null)
                yield return new JumpIn(mobRenderer.SetActiveAnim(value));
        }
        
        // public IEnumerator OnAgentTurn()
        // {
        //     if(!initialized || !isActive) { yield break; }
        //     
        //     RecalculateStats();
        //     yield return new JumpIn(OnAgentWakeUp?.Invoke(this));
        // }

        public void AssignRootAgent(MobRootAgentBaseSO agentSO)
        {
            if (agentSO == null)
            {
                return;
            }
            
            if (this.rootAgent != null)
            {
                RemoveListener(this.rootAgent);
                this.rootAgent = null;
            }
            
            this.rootAgent = (MobRootAgentBase)(AddListener(agentSO));
            this.ModifyTurnSlicesInPlace(
                 Globals.combatMgr.Instance.now,
                 Globals.combatMgr.Instance.turnSchedule);
        }

        // Modify current TurnSchedule by inserting its own turnSlices
        public virtual void ModifyTurnSlicesInPlace(Timestamp now, TurnScheduleSequence schedule)
        {
            if (this.rootAgent != null && schedule != null)
            {
                this.rootAgent.ModifyTurnSlicesInPlace(now, schedule);
            }
        }

        public virtual int GetTurnSliceModificationPriority(Timestamp now, TurnScheduleSequence schedule)
            => this.rootAgent?.GetTurnSliceModificationPriority(now, schedule) ?? 0;
        
        public void SetInactiveImmediately()
        {
            if(isActive)
            {
                isActive = false;
            }
        }
        
        public IEnumerator TryAutoEndTurn()
        {
            // Auto end disabled
            yield break;
            if(actionPoints <= 0)
            {
                yield return new JumpIn(SetActive(false));
            }
        }

        public IEnumerator _OnNextTurn()
        {
            yield return new JumpIn(OnNextTurn?.InvokeCoroutine(this));
        }
        
        public IEnumerator _OnRecoveryStage()
        {
            Recover();
            yield return new JumpIn(OnRecoveryStage?.InvokeCoroutine(this));
            RecalculateStats();
        }

        public void ConstructRenderer()
        {
            renderer = GameObject.Instantiate(
                    baseDescriptor.rendererPrefab.gameObject, Globals.backend.BackendToRenderPosCenteredGrounded(Position),
                    Quaternion.identity)
                .GetComponent<MobRenderer>();
        }

        public void UpdateRenderer()
        {
            if (mobRenderer != null)
            {
                mobRenderer.data = this;
                mobRenderer.Refresh();
            }
        }
    }
}
