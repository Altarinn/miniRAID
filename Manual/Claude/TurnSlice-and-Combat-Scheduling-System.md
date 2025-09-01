# TurnSlice and Combat Scheduling System Architecture

## Table of Contents
1. [System Overview](#system-overview)
2. [TurnSlice Architecture](#turnslice-architecture)
3. [Combat Scheduling System](#combat-scheduling-system)
4. [TurnSlice Specialized Types](#turnslice-specialized-types)
5. [TurnSliceBuff System](#turnslicebuff-system)
6. [Advanced Features](#advanced-features)
7. [Implementation Patterns & Best Practices](#implementation-patterns--best-practices)

---

## System Overview

The TurnSlice and Combat Scheduling System is miniRAID's core turn-based combat orchestration framework. It provides a flexible, event-driven architecture that manages the execution order, timing, and interaction of all combat actions through a queue-based scheduling system.

### Core Concepts

**TurnSlice**: The fundamental unit of combat execution - represents a single action, phase, or event in the combat timeline. Each turn slice encapsulates:
- **What** to execute (action logic)
- **When** to execute (scheduling metadata)
- **Who** initiated it (source mob/system)
- **How** to display it (UI integration)

**Combat Scheduler**: The central orchestrator that manages the turn schedule queue, processes turn slices sequentially, handles UI synchronization, and coordinates the overall combat flow.

**Turn Schedule**: A priority-managed queue (`TurnScheduleSequence`) that maintains the order of all pending turn slices, supports dynamic insertion/removal, and respects priority and category-based ordering.

### Three-Layer Architecture Integration

The turn system follows miniRAID's three-layer architecture:

1. **ScriptableObject Layer**: `AbstractTurnSliceSO` and derivatives define turn slice templates with Inspector-configurable properties
2. **Runtime Layer**: `TurnSlice` instances represent active combat events with calculated state and metadata
3. **Presentation Layer**: UI components render turn schedules, current actions, and player interaction interfaces

---

## TurnSlice Architecture

### Core Hierarchy

```csharp
// Base template layer - ScriptableObject configuration
public abstract class AbstractTurnSliceSO : CustomIconScriptableObject
{
    // Visual representation
    public Sprite barIcon;
    public Color mainColor;
    public string label;
    public bool showInUI = true;
    
    // Scheduling configuration
    public TurnSliceCategory defaultCategory;
    public int[] allowedTurns;        // Turn-based filtering
    public string[] allowedPhases;    // Phase-based filtering
    
    // Factory method for runtime instantiation
    public abstract IEnumerator Turn(TurnSlice slice, CombatSchedulerCoroutine coroutine);
}

// Standard turn slice template
public abstract class TurnSliceSO : AbstractTurnSliceSO
{
    public virtual TurnSlice Wrap(TurnSliceMetadata metadata);
}

// Runtime execution layer
public class TurnSlice : BackendState
{
    public AbstractTurnSliceSO data;         // Template reference
    public TurnSliceMetadata metadata;       // Runtime state
    public bool muted = false;              // Termination flag
    
    // UI integration properties
    public virtual bool ShowInUI => data.showInUI && (!muted);
    public virtual string Label => data.label;
    public virtual Color MainColor => data.mainColor;
    public virtual Sprite BarIcon => data.barIcon;
}
```

### Turn Slice Metadata System

```csharp
public struct TurnSliceMetadata
{
    public TurnSliceCategory category;        // Priority grouping
    public Timestamp timestamp;              // Temporal context
    public ValueGetter<None, int> Priority;  // Execution priority
    public MobData source;                   // Originating mob/system
}

public struct Timestamp
{
    public string currentPhase;     // Combat phase identifier
    public int currentTurnID;       // Global turn counter
}
```

**Category-Based Priority System:**
```csharp
public enum TurnSliceCategory
{
    Inherited = 0,          // Uses default from template
    PlayerTurn,             // Player-controlled actions
    EnemyRegularTurn,       // Standard enemy actions
    EnemySpecialTurn,       // High-priority enemy actions
    AllyRegularTurn,        // Allied unit actions
    AllySpecialTurn,        // High-priority allied actions
    UtilityTurn,           // System/maintenance actions
    Uncategorized          // Fallback category
}
```

### Turn Slice Lifecycle

1. **Template Definition**: ScriptableObject configuration in Inspector
2. **Instantiation**: `Wrap(metadata)` creates runtime instance
3. **Registration**: `RegisterTo(coroutine)` links to scheduler
4. **Queueing**: Added to `TurnScheduleSequence` with priority ordering
5. **Execution**: `Turn()` coroutine processes the action
6. **UI Updates**: Visual representation updated during execution
7. **Cleanup**: `OnRemove(coroutine)` performs resource cleanup
8. **Termination**: Muted turn slices are skipped but remain in memory for reference

### Scheduling Filters

Turn slices can be conditionally included/excluded from scheduling:

```csharp
public virtual bool ScheduleFilter(TurnSliceMetadata meta)
{
    // Turn-based filtering
    bool turnOK = (allowedTurns?.Length == 0) || 
                  allowedTurns.Contains(meta.timestamp.currentTurnID);
    
    // Phase-based filtering
    bool phaseOK = (allowedPhases?.Length == 0) || 
                   allowedPhases.Contains(meta.timestamp.currentPhase);
    
    return turnOK && phaseOK;
}
```

---

## Combat Scheduling System

### CombatSchedulerCoroutine Architecture

The `CombatSchedulerCoroutine` is the central hub managing all combat execution:

```csharp
public partial class CombatSchedulerCoroutine : MonoBehaviour
{
    // Core scheduling
    public TurnScheduleSequence turnSchedule;        // Main execution queue
    public Timestamp now, appendedTurns;             // Time tracking
    
    // State management  
    private TurnSlice currentTurnSlice;              // Currently executing slice
    private TurnSchedulerGeneratorBase turnScheduler; // Turn generation strategy
    
    // Execution context
    public bool playerPhaseEnd = false;
    public bool turnEnd = false;
    
    // UI integration
    public IEnumerator OnBeforeNextTurnSlice;        // Pre-execution hooks
}
```

### Main Combat Loop

```csharp
public IEnumerator Combat()
{
    // System initialization
    yield return new JumpIn(Globals.backend.Initialize());
    yield return new JumpIn(Preparation());
    yield return new JumpIn(StartCombat());
    
    // Main execution loop
    while (!IsCombatFinished())
    {
        // Pre-execution hooks (save/load, special events)
        if (OnBeforeNextTurnSlice != null)
        {
            yield return new JumpIn(OnBeforeNextTurnSlice);
            OnBeforeNextTurnSlice = null;
        }
        
        // Save state for player turns (rollback support)
        if (turnSchedule.First.Value is CommonPlayerTurnSlice)
        {
            SaveDataSerializer.saveSlotBackup = SaveDataSerializer.SerializeEverything();
        }
        
        // Process next turn slice
        UpdateSchedulerUI();                          // Refresh UI display
        currentTurnSlice = turnSchedule.Dequeue();    // Get next action
        
        if (!currentTurnSlice.muted)
            yield return new JumpIn(currentTurnSlice.Turn()); // Execute
        
        // Post-execution processing
        if (turnWaitTime > 0)
            yield return new JumpIn(Chill());         // Animation timing
        
        currentTurnSlice.OnRemove(this);              // Cleanup
        KeepTurnScheduleLength();                     // Maintain queue size
    }
}
```

### Turn Schedule Management

The `TurnScheduleSequence` extends `LinkedListQueue<TurnSlice>` with specialized operations:

#### Dynamic Insertion Methods

```csharp
// Insert at specific index
public void InsertTurnSliceAt(int index, TurnSlice slice);

// Insert relative to existing node
public void InsertTurnSliceBefore(LinkedListNode<TurnSlice> node, TurnSlice slice);
public void InsertTurnSliceRightAfter(LinkedListNode<TurnSlice> node, TurnSlice slice);

// Priority-aware insertion (maintains category ordering)
public void InsertTurnSliceAfter(LinkedListNode<TurnSlice> pivotNode, TurnSlice slice)
{
    var node = pivotNode;
    int priority = slice.metadata.Priority.Eval(null);
    
    // Find insertion point respecting priority and category
    while (node?.Next != null)
    {
        var nextNode = node.Next;
        
        // Insert before lower priority or different category
        if (nextNode.Value.metadata.Priority.Eval(null) < priority ||
            nextNode.Value.metadata.category != slice.metadata.category)
            break;
            
        node = nextNode;
    }
    
    slice.metadata.timestamp.currentTurnID = node.Value.metadata.timestamp.currentTurnID;
    AddAfter(node, slice);
}
```

#### Claim System for Placeholder Turn Slices

```csharp
// Replace dummy placeholder with actual turn slice
public void Claim(LinkedListNode<TurnSlice> dummyNode, TurnSlice slice)
{
    var newSource = slice.metadata.source;
    slice.metadata = dummyNode.Value.metadata;    // Inherit timing
    slice.metadata.source = newSource;            // Preserve source
    
    slice.RegisterTo(parentScheduler);
    dummyNode.Value = slice;                      // Replace in-place
}

// Find and claim first matching placeholder
public void ClaimFirst(AbstractTurnSliceSO target, TurnSlice slice)
    => Claim(this.Where(x => x.Value.data == target).First(), slice);
```

### Turn Generation System

```csharp
public abstract class TurnSchedulerGeneratorBase : ScriptableObject
{
    public abstract TurnScheduleSequence GetNewTurn(ref Timestamp now);
}

// Example: Standard turn structure
public class DefaultTurnGenerator : TurnSchedulerGeneratorBase  
{
    public List<TurnSliceSO> turnSlices;
    
    public override TurnScheduleSequence GetNewTurn(ref Timestamp now)
    {
        now.currentTurnID += 1;
        return new TurnScheduleSequence(
            turnSlices
                .Select(x => x.Wrap(new TurnSliceMetadata(null)))
                .Where(x => x != null));
    }
}
```

---

## TurnSlice Specialized Types

### Mob Action Turn Slices

Base class for all mob-initiated actions:

```csharp
public abstract class MobActionTurnSliceSO : AbstractTurnSliceSO
{
    public virtual MobActionTurnSlice Wrap(MobData mob, RuntimeAction action, TurnSliceMetadata metadata);
}

public class MobActionTurnSlice : TurnSlice
{
    public MobData mob;              // Acting mob
    public RuntimeAction action;     // Action being performed
    
    public override string Label => $"{mob.nickname}: {action.ActionName}";
}
```

### Player Turn Management

Player turns use a specialized locking mechanism to handle multi-character control:

```csharp
public class CommonPlayerTurnSliceSO : TurnSliceSO
{
    public string message = null;                    // Phase announcement
    public Consts.UnitGroup group = Consts.UnitGroup.Player;
    
    public override IEnumerator Turn(TurnSlice slice, CombatSchedulerCoroutine coroutine)
    {
        // Phase announcement
        if (message != null)
        {
            Globals.ui.Instance.combatView.ShowCenterTitle(message);
            yield return new WaitForSeconds(0.4f);
            Globals.ui.Instance.combatView.HideCenterTitle();
        }
        
        // Player input handling
        if (group == Consts.UnitGroup.Player)
        {
            yield return new JumpIn(coroutine.UIWaitPlayerInput());
            yield return new JumpIn(NotifyNewTurn(Databackend.GetSingleton().allMobs));
        }
        
        // Wait for phase completion
        while (coroutine.turnEnd == false)
        {
            yield return null;
        }
    }
}

// Enhanced version with turn slice locking
public class LockedPlayerTurnSlice : TurnSlice
{
    private MobData lockedToPlayer = null;
    private bool isLocked = false;
    
    public void LockToPlayer(MobData player);        // Lock to specific character
    public bool CanPlayerAct(MobData player);        // Check action permission
    
    private IEnumerator OnGlobalActionPostcast(MobData mob, RuntimeAction action, SpellTarget target)
    {
        // Auto-lock to first acting player character
        if (mob.unitGroup == group && !isLocked)
        {
            LockToPlayer(mob);
        }
        yield break;
    }
}
```

### System Turn Slices

**StartTurnTurnSliceSO**: Increments global turn counter
```csharp
public override IEnumerator Turn(TurnSlice slice, CombatSchedulerCoroutine coroutine)
{
    coroutine.now.currentTurnID++;
    Globals.combatTracker.Turns = coroutine.now.currentTurnID;
    yield break;
}
```

**EndTurnTurnSliceSO**: Marks turn completion (currently no-op)

**EmptyTurnSliceSO**: Placeholder for dynamic content

**PhaseTurnSliceSO**: Phase transition management

### Preparable Action Turn Slices

Extended mob action turn slices supporting preparation phases and turn slice buffs:

```csharp
public abstract class PreparableActionTurnSliceSO : MobActionTurnSliceSO
{
    [InlineEditor(InlineEditorObjectFieldModes.Boxed)]
    public List<TurnSliceBuffSO> turnSliceBuffs = new List<TurnSliceBuffSO>();
    
    // Extension hooks
    public virtual void OnConstruction(PreparableActionTurnSlice slice) {}
    public virtual IEnumerator OnGlobalPostAction(
        PreparableActionTurnSlice slice, MobData actionSource, 
        RuntimeAction action, SpellTarget target) { yield break; }
}
```

---

## TurnSliceBuff System

### Overview

The TurnSliceBuff system is a specialized extension of the existing Buff system that allows buffs to terminate their associated turn slices when certain conditions are met. This enables complex mechanics where ongoing actions can be interrupted by accumulated damage, time limits, or other triggers.

### Core Architecture

```csharp
// Template layer - ScriptableObject configuration
public abstract class TurnSliceBuffSO : BuffSO
{
    public override MobListener Wrap(MobData parent)
    {
        return WrapTurnSliceBuff(parent);
    }
    
    protected abstract TurnSliceBuff WrapTurnSliceBuff(MobData parent);
}

// Runtime layer - Active buff with turn slice control
public abstract class TurnSliceBuff : Buff.Buff
{
    protected PreparableActionTurnSlice associatedTurnSlice;
    
    public virtual void AssociateWithTurnSlice(PreparableActionTurnSlice turnSlice)
    {
        associatedTurnSlice = turnSlice;
    }
    
    protected virtual void TerminateAssociatedTurnSlice()
    {
        if (associatedTurnSlice != null)
        {
            associatedTurnSlice.Mute();
            Globals.logger?.Log($"[TurnSliceBuff] Terminated turn slice: {associatedTurnSlice.Label}");
        }
    }
}
```

### Integration with PreparableActionTurnSlice

```csharp
public class PreparableActionTurnSlice : MobActionTurnSlice
{
    [OdinSerialize] protected List<TurnSliceBuff> turnSliceBuffs = new List<TurnSliceBuff>();
    
    protected virtual void ApplyTurnSliceBuffs()
    {
        if (paSliceData.turnSliceBuffs == null) return;
        
        foreach (var buffSO in paSliceData.turnSliceBuffs)
        {
            if (buffSO == null) continue;
            
            var buffInstance = this.mob.AddListener(buffSO);
            if (buffInstance is TurnSliceBuff turnSliceBuff)
            {
                turnSliceBuffs.Add(turnSliceBuff);           // Track for cleanup
                turnSliceBuff.AssociateWithTurnSlice(this);   // Create association
            }
        }
    }
    
    public override void OnRemove(CombatSchedulerCoroutine coroutine)
    {
        // Cleanup all associated turn slice buffs
        turnSliceBuffs.ForEach(x => { x.Destroy(); });
        base.OnRemove(coroutine);
    }
}
```

### Implementation Example: RoarOverloadBuff

**Configuration (ScriptableObject)**:
```csharp
[CreateAssetMenu(menuName = "miniRAID/TurnSliceBuffs/RoarOverloadBuff")]
public class RoarOverloadBuffSO : TurnSliceBuffSO
{
    public float damageThreshold = 100f;
    public SimpleExplosionFx explosionFx;
    public SpellDamageHeal selfDamage;
    
    protected override TurnSliceBuff WrapTurnSliceBuff(MobData parent)
    {
        return new RoarOverloadBuff(parent, this);
    }
}
```

**Runtime Logic**:
```csharp
public class RoarOverloadBuff : TurnSliceBuff
{
    [SerializeField] private float damageAccumulated = 0f;
    private RoarOverloadBuffSO BuffData => (RoarOverloadBuffSO)data;
    
    public override string name
    {
        get
        {
            float remaining = Mathf.Max(0, BuffData.damageThreshold - damageAccumulated);
            return $"{base.name} [{remaining:F0}]";
        }
    }
    
    public override void OnAttach(MobData mob)
    {
        base.OnAttach(mob);
        damageAccumulated = 0f;
        mob.OnDamageReceived.AddListener(OnReceiveDamage);
    }
    
    public IEnumerator OnReceiveDamage(MobData mob, Consts.DamageHeal_Result info)
    {
        // CRITICAL: Prevent self-damage loops
        if (info.source == mob) yield break;
        
        damageAccumulated += info.value;
        
        if (damageAccumulated >= BuffData.damageThreshold)
        {
            // Execute termination effects
            yield return BuffData.explosionFx?.Do(mob.Position);
            yield return BuffData.selfDamage?.Do(this, mob, mob);
            
            // Terminate the associated turn slice
            TerminateAssociatedTurnSlice();
            Destroy();
        }
    }
    
    public override void OnRemove()
    {
        parent.OnDamageReceived.RemoveListener(OnReceiveDamage);
        base.OnRemove();
    }
}
```

### Design Patterns in TurnSliceBuff System

#### Template Method Pattern
- `TurnSliceBuffSO.WrapTurnSliceBuff()` provides consistent factory interface
- Subclasses implement specific buff creation logic
- Maintains compatibility with existing `BuffSO.Wrap()` pattern

#### Observer Pattern Extension
- Builds on existing event-driven Buff system
- TurnSliceBuff observes game events (damage, time, etc.)
- Can notify turn slice to terminate when conditions met

#### Composition over Inheritance
- Extends existing systems rather than replacing them
- TurnSliceBuff has association with TurnSlice (not inheritance)
- Reuses all existing Buff functionality

#### Strategy Pattern
- Different termination conditions as different TurnSliceBuff subclasses
- Same interface (`TerminateAssociatedTurnSlice()`) with different implementations
- Easily extensible for new termination mechanics

---

## Advanced Features

### UI Integration System

The scheduler provides real-time UI updates showing upcoming actions:

```csharp
public void UpdateSchedulerUI()
{
    int length = 12;
    
    // Generate turn sequence display
    string message = String.Join("\n",
        turnSchedule
            .Where(x => x.ShowInUI)
            .Take(length)
            .Select(x => $"<color=#{ColorUtility.ToHtmlStringRGB(x.MainColor)}> {x.Label} </color>")
            .ToArray());
    
    Globals.ui.Instance.combatView.schedulerPlaceholder.text = message;
    
    // Current action highlighting
    var currentDisplaySlice = turnSchedule.First(x => x.ShowInUI);
    Globals.ui.Instance.combatView.currentTurnPlaceholder.text = 
        $"<color=#{ColorUtility.ToHtmlStringRGB(currentDisplaySlice.MainColor)}> {currentDisplaySlice.Label} </color>";
}
```

### Player Input System

Asynchronous player action handling with validation:

```csharp
public IEnumerator UIWaitPlayerInput()
{
    Globals.ui.Instance.EnterState();
    
    while (!playerPhaseEnd)
    {
        if (actionToDo == null) 
        { 
            yield return null; 
        }
        else
        {
            yield return new JumpIn(actionToDo.Invoke());
        }
    }
    
    playerPhaseEnd = false;
}

public bool UIPickedAction(IEnumerator action, IEnumerator onActionFinished)
{
    if (actionToDo != null)
    {
        Debug.LogError("UIPickedAction called before previous action finished!");
        return false;
    }
    
    IEnumerator OnFinishWrapper()
    {
        yield return new JumpIn(action);
        yield return new JumpIn(onActionFinished);
        actionToDo = null;
    };
    
    actionToDo = OnFinishWrapper;
    return true;
}
```

### Serialization and Save/Load Support

Turn slices integrate with miniRAID's save system:

- **Automatic Snapshots**: Player turn slices trigger automatic save state creation for rollback support
- **State Preservation**: Turn slice metadata and execution state preserved across save/load
- **Odin Integration**: `[OdinSerialize]` attributes ensure proper serialization of dynamic collections

### Event Integration

The system provides multiple event hooks:

```csharp
// Global action tracking
Globals.backend.onGlobalActionPostcast.AddListener(OnGlobalPostAction);

// Turn progression events  
mob._OnNextTurn();          // New turn notifications
mob._OnRecoveryStage();     // Recovery phase events

// Custom turn slice hooks
public virtual void OnConstruction(PreparableActionTurnSlice slice);
public virtual IEnumerator OnGlobalPostAction(PreparableActionTurnSlice slice, 
    MobData actionSource, RuntimeAction action, SpellTarget target);
```

---

## Implementation Patterns & Best Practices

### Common Design Patterns

#### Factory Method Pattern
```csharp
// ScriptableObject templates create runtime instances
public virtual TurnSlice Wrap(TurnSliceMetadata metadata)
{
    if (!ScheduleFilter(metadata)) return null;
    return new TurnSlice(this, metadata);
}

// Specialized factories for different contexts
public virtual MobActionTurnSlice Wrap(MobData mob, RuntimeAction action, TurnSliceMetadata metadata)
{
    return new MobActionTurnSlice(mob, action, this, metadata);
}
```

#### Command Pattern
- Each turn slice encapsulates a complete action with execution context
- Supports undo/redo through save state snapshots
- Parameterized requests through metadata system

#### State Machine Integration
- Turn slices represent state transitions in combat flow
- Categories provide state grouping and prioritization
- Muting system allows graceful state termination

### Resource Management Best Practices

#### Proper Event Subscription/Unsubscription
```csharp
public override void OnAttach(MobData mob)
{
    base.OnAttach(mob);
    mob.OnDamageReceived.AddListener(OnReceiveDamage);
}

public override void OnRemove()
{
    parent.OnDamageReceived.RemoveListener(OnReceiveDamage);  // CRITICAL: Always unsubscribe
    base.OnRemove();
}
```

#### Memory Leak Prevention
```csharp
public override void OnRemove(CombatSchedulerCoroutine coroutine)
{
    // Clean up associated resources
    turnSliceBuffs.ForEach(x => { x.Destroy(); });           // Use Destroy(), not RemoveFromMob()
    Globals.backend.onGlobalActionPostcast.RemoveListener(OnGlobalPostAction);
    base.OnRemove(coroutine);
}
```

#### Collection Management
```csharp
[OdinSerialize] protected List<TurnSliceBuff> turnSliceBuffs = new List<TurnSliceBuff>();

// Track associated objects for proper cleanup
public void AssociateResource(IDisposable resource)
{
    managedResources.Add(resource);
}
```

### Common Pitfalls & Solutions

#### Self-Damage Loops
**Problem**: Buff triggers self-damage which retriggers the buff, creating infinite loops  
**Solution**: Always check source identity in damage listeners:
```csharp
public IEnumerator OnReceiveDamage(MobData mob, Consts.DamageHeal_Result info)
{
    if (info.source == mob) yield break;  // CRITICAL: Prevent self-loops
    
    // Process damage...
}
```

#### Timing and Sequencing Issues
**Problem**: Turn slices execute out of intended order  
**Solution**: Use proper priority values and category assignment:
```csharp
// Higher priority executes first within same category
metadata.Priority = 100;
metadata.category = TurnSliceCategory.PlayerTurn;

// Use InsertTurnSliceAfter for priority-aware insertion
turnSchedule.InsertTurnSliceAfter(pivotNode, newSlice);
```

#### UI Consistency Problems
**Problem**: Terminated turn slices still appear in UI  
**Solution**: Implement proper ShowInUI logic:
```csharp
public override bool ShowInUI => data.showInUI && (!muted);
```

#### Metadata Corruption
**Problem**: Timestamp/metadata inconsistencies after dynamic insertion  
**Solution**: Always sync metadata during insertion:
```csharp
public void InsertTurnSliceAfter(LinkedListNode<TurnSlice> pivotNode, TurnSlice slice)
{
    // ... insertion logic ...
    
    // CRITICAL: Sync timestamp with insertion point
    slice.metadata.timestamp.currentTurnID = node.Value.metadata.timestamp.currentTurnID;
    AddAfter(node, slice);
}
```

### Method Naming Conventions

Follow established patterns from the codebase:

- **Factory Methods**: Use `Wrap()` not `Create()` 
  - `BuffSO.Wrap()` → `TurnSliceBuffSO.WrapTurnSliceBuff()`
- **Cleanup Methods**: Use `Destroy()` not `RemoveFromMob()`
- **Configuration Methods**: Use `Apply...()` pattern
  - `ApplyTurnSliceBuffs()`, `ApplyConfiguration()`
- **State Management**: Use `Mute()` / `Unmute()` for termination

### Extension Guidelines

#### When to Create New Turn Slice Types

**Use TurnSlice for**:
- Simple system events (phase transitions, turn increments)
- One-shot effects that don't need ongoing monitoring
- UI/display-only turn slices

**Use MobActionTurnSlice for**:
- Actions performed by specific mobs
- Actions requiring mob context and state
- Actions that should appear in action history

**Use PreparableActionTurnSlice for**:
- Actions with preparation phases
- Actions that need turn slice buffs
- Complex actions requiring global event monitoring
- Actions that can be interrupted or modified during execution

#### Creating Custom Turn Slice Categories

```csharp
// Add new categories to enum (requires code modification)
public enum TurnSliceCategory
{
    // ... existing values ...
    BossSpecialTurn,        // Very high priority boss actions
    EnvironmentalTurn,      // Environmental effects and hazards
    TriggeredResponse       // Reaction-based actions
}

// Configure priority relationships in scheduler
metadata.category = TurnSliceCategory.BossSpecialTurn;
metadata.Priority = 1000;  // Ensure high priority execution
```

#### TurnSliceBuff Extension Patterns

**Condition-Based Termination**:
```csharp
public class HealthThresholdTurnSliceBuff : TurnSliceBuff
{
    protected override void OnAttach(MobData mob)
    {
        base.OnAttach(mob);
        mob.OnHealthChanged.AddListener(CheckHealthThreshold);
    }
    
    private IEnumerator CheckHealthThreshold(MobData mob, float newHealth)
    {
        if (newHealth <= BuffData.threshold)
        {
            TerminateAssociatedTurnSlice();
            Destroy();
        }
        yield break;
    }
}
```

**Time-Based Termination**:
```csharp
public class TimedTurnSliceBuff : TurnSliceBuff
{
    private float timeRemaining;
    
    protected override void OnAttach(MobData mob)
    {
        base.OnAttach(mob);
        timeRemaining = BuffData.duration;
        StartCoroutine(CountdownTimer());
    }
    
    private IEnumerator CountdownTimer()
    {
        while (timeRemaining > 0 && !destroyed)
        {
            timeRemaining -= Time.deltaTime;
            yield return null;
        }
        
        if (!destroyed)
        {
            TerminateAssociatedTurnSlice();
            Destroy();
        }
    }
}
```

### Testing and Debugging Guidelines

#### Debugging Turn Schedule Issues

```csharp
// Log turn schedule state
Debug.Log($"Turn Schedule ({turnSchedule.Count} slices):");
foreach (var slice in turnSchedule)
{
    Debug.Log($"  - {slice.Label} (Cat: {slice.metadata.category}, Pri: {slice.metadata.Priority.Eval(null)}, Muted: {slice.muted})");
}
```

#### Monitoring Turn Slice Lifecycle

```csharp
public override void RegisterTo(CombatSchedulerCoroutine coroutine)
{
    base.RegisterTo(coroutine);
    Globals.logger?.Log($"[TurnSlice] Registered: {Label}");
}

public override void OnRemove(CombatSchedulerCoroutine coroutine)
{
    Globals.logger?.Log($"[TurnSlice] Removed: {Label}");
    base.OnRemove(coroutine);
}
```

#### Validating Metadata Consistency

```csharp
public void ValidateMetadata()
{
    if (metadata.source == null)
        Debug.LogWarning($"[TurnSlice] {Label}: Missing source reference");
        
    if (metadata.timestamp.currentTurnID <= 0)
        Debug.LogWarning($"[TurnSlice] {Label}: Invalid turn ID");
        
    if (metadata.category == TurnSliceCategory.Inherited)
        Debug.LogWarning($"[TurnSlice] {Label}: Unresolved category inheritance");
}
```

---

## Future Extensions and Considerations

### Potential Enhancements

**Multi-Threading Support**: 
- Separate UI updates from turn logic processing
- Background turn generation for smoother gameplay
- Async/await integration for non-blocking operations

**Advanced Priority Systems**:
- Dynamic priority calculation based on game state
- Context-sensitive priority modifiers
- Priority inheritance from source mobs

**Enhanced Metadata System**:
- Rich context information for turn slices
- Dependency tracking between related turn slices
- Execution constraint validation

**Improved Error Recovery**:
- Rollback mechanisms for corrupted turn schedules
- Graceful degradation when turn slices fail
- Diagnostic tools for identifying scheduling issues

### Integration Opportunities

**Action System Enhancement**:
- Tighter integration between RuntimeAction and TurnSlice
- Action-specific turn slice customization
- Shared validation and error handling

**Buff System Expansion**:
- More sophisticated buff-turn slice interactions
- Conditional buff application based on turn context
- Buff dependency resolution

**AI System Integration**:
- AI-driven turn slice generation and modification
- Dynamic strategy adaptation based on turn schedule
- Predictive turn scheduling for AI opponents

---

This comprehensive documentation provides the technical foundation for understanding, extending, and maintaining miniRAID's TurnSlice and Combat Scheduling System. The architecture's flexibility supports both simple turn-based mechanics and complex, conditional combat interactions while maintaining clean separation of concerns and robust error handling.