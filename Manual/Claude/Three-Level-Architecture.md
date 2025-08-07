# Three-Level Architecture: ScriptableObject → Runtime States → Renderers

## Overview

miniRAID employs a sophisticated three-level architecture pattern that cleanly separates data definition, runtime behavior, and visual presentation. This architecture enables robust serialization, dynamic gameplay systems, and flexible rendering while maintaining clear boundaries between concerns.

The three levels are:
1. **ScriptableObject Layer** - Static data definitions and templates
2. **Runtime State Layer (BackendState)** - Dynamic behavior and game logic
3. **Renderer Layer (IStateRenderer)** - Visual presentation and UI

## Architecture Philosophy

### Separation of Concerns
Each layer has distinct responsibilities:
- **Data Layer**: Defines "what" (static templates, configurations, rules)
- **Logic Layer**: Manages "how" and "when" (runtime behavior, state transitions, events)
- **Presentation Layer**: Handles "appearance" (visual feedback, animations, UI updates)

### Backend-Frontend Architecture
The system maintains strict separation between backend (game logic) and frontend (presentation):
- **Backend**: Pure game logic, serializable state, deterministic behavior
- **Frontend**: Visual effects, animations, user interface, platform-specific rendering
- **Interface**: `IStateRenderer` provides the bridge between backend state and frontend rendering

## Level 1: ScriptableObject Layer

### Purpose
ScriptableObjects serve as **immutable data templates** that define the static properties and behavior patterns for game entities. They act as "DNA" or "blueprints" that get instantiated into runtime objects.

### Key Characteristics
- **Asset-based**: Stored as Unity assets, editable in inspector
- **Immutable at runtime**: Never modified during gameplay
- **Addressable References**: Not directly serialized by Odin Serializer; referenced via addressable paths during deserialization
- **Inspector integration**: Full Odin Inspector support for complex data structures

### Primary Implementations

#### ActionDataSO Family
```csharp
public abstract class ActionDataSO : CustomIconScriptableObject
{
    public LocalizedString ActionNameKey;
    public int maxLevel;
    public PowerGetter power, auxPower;
    public virtual EnumerateGridCollider MainShape { get; }
    
    // Pure function - no side effects
    public abstract IEnumerator OnPerform(RuntimeAction ract, MobData mob, SpellTarget target);
}

public class ActionDataSO<TSpellTarget> : ActionDataSO where TSpellTarget : SpellTarget
{
    public Dictionary<Cost.Type, LuaBoundedGetter<(MobData, TSpellTarget), MobData, double>> costs;
    public UI.TargetRequester.TargetRequesterBase<TSpellTarget> Requester;
}
```

**Key Features:**
- Generic type system for different target types (`ActionDataSO<Vector3IntTarget>`, `ActionDataSO<MobTarget>`)
- Level-based scaling through `LeveledStats` and `PowerGetter`
- Cost calculation with bounded getters for UI display
- Target validation and picking integration
- Localization support

#### BuffSO System
```csharp
public partial class BuffSO : StatModifierSO
{
    public bool timed, stackable;
    public int timeMax, maxStack;
    public List<BuffDHOTDef> overtimeEffects;
    public Consts.BuffFlags flags;
    
    // Factory method
    public override MobListener Wrap(MobData parent) => new Buff(parent, this);
}
```

**Design Patterns:**
- **Template Method**: Base behavior defined in SO, specific implementation in runtime
- **Factory Pattern**: `Wrap()` method creates appropriate runtime instances
- **Configuration Object**: Extensive serialized parameters for behavior customization

### Advanced ScriptableObject Features

#### Data-Driven Design
```csharp
public class PowerGetter
{
    public enum PowerGetterType { STATIC, AttackPower, SpellPower, HealPower, BuffPower, DYNAMIC }
    public PowerGetterType powerType;
    public LeveledStats<float> powerFactor;
    
    public float Eval(int level, MobData param)
    {
        switch (powerType)
        {
            case PowerGetterType.AttackPower:
                return param.attackPower * powerFactor.Eval(level);
            // ... other cases
        }
    }
}
```

This pattern allows designers to create complex scaling formulas without programming, using combinations of static values and dynamic mob stats.

## Level 2: Runtime State Layer (BackendState)

### Core Infrastructure

#### BackendState Base Class
```csharp
public class BackendState
{
    public Guid guid;                    // Unique identifier for serialization
    [NonSerialized] 
    public IStateRenderer renderer;      // Frontend connection
    
    public void Register() => Globals.backend.RegisterState(this);
    public void DestroyRenderer() => renderer?.Destroy();
}
```

**Key Responsibilities:**
- **Identity Management**: Each state has unique GUID for persistence
- **Lifecycle Management**: Registration with backend, renderer cleanup
- **Serialization Control**: `[NonSerialized]` renderer prevents circular references

#### IRenderableState Interface
```csharp
public interface IRenderableState
{
    public void ConstructRenderer();     // Create visual representation
    public void UpdateRenderer();        // Sync state to renderer
}
```

**Rendering Pipeline:**
1. State registers with backend
2. Backend calls `ConstructRenderer()` on next coroutine tick
3. Game events trigger `UpdateRenderer()` calls
4. Renderer stays synchronized with state changes

### Runtime Action System

#### RuntimeAction Architecture
```csharp
public class RuntimeAction<TSpellTarget> : RuntimeAction where TSpellTarget : SpellTarget
{
    public ActionDataSO<TSpellTarget> actionData;  // Reference to template
    [NonSerialized] public dNumber power, auxPower;  // Calculated stats
    public List<(Cost, Cost)> costBounds;         // Dynamic cost ranges
    
    public void RecalculateStats(MobData mob)
    {
        // 1. Calculate base values from ScriptableObject
        power = dNumber.CreateComposite(actionData.power.Eval(level, mob), "actionBase");
        
        // 2. Trigger mob events for modifications
        // mob.OnActionStatCalculation event will modify power values
        
        // 3. Finalize in OnRecalculateStatsFinish()
    }
}
```

**State Management Features:**
- **Dynamic Recalculation**: Stats recalculated based on current mob state
- **Event-Driven Updates**: Mob events can modify action properties
- **Cost Bounds**: Calculates min/max costs for UI display before final values
- **Cooldown Tracking**: Runtime-only state not stored in ScriptableObject

#### Event Integration Pattern
```csharp
public override void OnAttach(MobData mob)
{
    mob.OnNextTurn.AddListener(OnNextTurn);
    mob.OnRecoveryStage.AddListener(OnRecoveryStage);
    mob.OnStatCalculationFinish.AddListener(OnRecalculateStatsFinish);
}
```

This demonstrates the **Observer Pattern** where runtime states subscribe to mob events for automatic updates.

### Buff System Runtime

#### Buff Runtime Behavior
```csharp
public class Buff : StatModifier, IRenderableState
{
    public int timeRemain, stacks;          // Runtime-only state
    protected BuffSO buffData;              // Reference to template
    public MobData source;                  // Buff source (for snapshot calculations)
    
    public bool IsDuplicated(Buff other)
    {
        return (other.data == this.data) && (other.source.Equals(this.source));
    }
    
    public virtual bool TryAdd(MobData target)
    {
        if (base.TryAdd(target))
        {
            Buff duplicated = target.FindListener(x => this.IsDuplicated(x as Buff));
            if (duplicated != null)
            {
                return buffData.stackable ? duplicated.Stack() : duplicated.Refresh();
            }
        }
    }
}
```

**Advanced Runtime Features:**
- **Snapshot System**: `snapShot` flag determines if buff power is calculated once or continuously
- **Stacking Logic**: Complex stacking rules with refresh behavior
- **Source Tracking**: Maintains reference to buff source for power calculations
- **Overtime Effects**: DOT/HOT processing integrated with turn system

### State Serialization

#### Serialization Strategy
The architecture supports sophisticated save/load using Odin Serializer with strategic `[NonSerialized]` usage:

```csharp
public class BackendState
{
    public Guid guid;                    // Serialized - persistent identity
    [NonSerialized] 
    public IStateRenderer renderer;      // Not serialized - recreated on load
}

public class RuntimeAction : MobListener
{
    public ActionDataSO data;            // Addressable path stored, loaded from disk
    [NonSerialized] public dNumber power;  // Not serialized - recalculated
    [NonSerialized] public List<(Cost, Cost)> costBounds;  // Not serialized
}
```

**Odin Serialization Principles:**
- **ScriptableObject Handling**: Odin Serializer cannot serialize Unity.Object types directly
- **GUID-Based Reference System**: `CustomIconScriptableObject.Guid` field stores asset GUID (auto-updated via `OnValidate()`)
- **Manual Loading Required**: ScriptableObjects must be loaded manually from GUIDs during deserialization (currently unimplemented)
- **Calculated Values**: Runtime-calculated values are marked `[NonSerialized]`
- **Renderers**: Visual components never serialized, recreated on load

**Current Serialization Limitation**: The project currently lacks the implementation to reload ScriptableObjects from GUIDs during deserialization, but has the infrastructure in place (commented-out `CustomIconScriptableObjectFormatter`).

## Level 3: Renderer Layer (IStateRenderer)

### Renderer Interface
```csharp
public interface IStateRenderer
{
    public abstract void Refresh();                    // Update visual representation
    public virtual void OnReload() { }                // Handle deserialization
    public abstract void Destroy();                   // Clean up resources
}
```

### Renderer Implementations

#### MobRenderer - Complex State Renderer
```csharp
public partial class MobRenderer : MonoBehaviour, IStateRenderer
{
    public MobData data;                    // Reference to backend state
    public Animator animator;
    
    public void Refresh()
    {
        SyncRendererPosition();             // Sync transform to backend position
        UpdateStatusColor();                // Update visual status
    }
    
    public void SyncRendererPosition()
    {
        transform.position = backend.GridToWorldPosCenteredGrounded(data.Position);
    }
}
```

**MobRenderer Features:**
- **Transform Synchronization**: Keeps Unity transform in sync with backend position
- **Animation Integration**: Manages Unity Animator for visual feedback
- **Status Visualization**: Color coding and visual effects
- **Event-Driven Updates**: Responds to backend state changes

#### Indicator Renderers - Lightweight Visualization

##### SimpleSpriteIndicator
```csharp
public class SimpleSpriteIndicator : FollowMobStateRenderer
{
    public static SimpleSpriteIndicator Instantiate(Sprite sprite, Vector3 pos, int sortOrder = 0)
    {
        SimpleSpriteIndicator s = GameObject.Instantiate(Globals.prefabs.Instance.spriteIndicator, pos, Quaternion.identity)
            .AddComponent<SimpleSpriteIndicator>();
        s.GetComponentInChildren<SpriteRenderer>().sprite = sprite;
        return s;
    }
}
```

##### FollowMobStateRenderer Pattern
```csharp
public abstract class FollowMobStateRenderer : MonoBehaviour, IStateRenderer
{
    protected MobData follow;
    
    public virtual void Refresh()
    {
        transform.parent = follow?.mobRenderer?.transform;
        transform.localPosition = Vector3.zero;
    }
    
    public FollowMobStateRenderer Follow(MobData follow)
    {
        this.follow = follow;
        Refresh();
        return this;
    }
}
```

**Indicator System Design:**
- **Factory Pattern**: Static `Instantiate()` methods for easy creation
- **Fluent Interface**: `Follow()` method returns self for method chaining
- **Automatic Parenting**: Follows mob transforms automatically
- **Lightweight**: Minimal overhead for status indicators

#### Batched Rendering System
```csharp
public class BatchedRenderer : IStateRenderer
{
    public HashSet<IStateRenderer> renderers = new HashSet<IStateRenderer>();
    
    public virtual void Refresh()
    {
        renderers.ForEach(x => x.Refresh());
    }
    
    public virtual void Destroy()
    {
        renderers.ForEach(x => x.Destroy());
        renderers.Clear();
    }
}
```

**Optimization Features:**
- **Composite Pattern**: Manages multiple renderers as single unit
- **Batch Operations**: Single call updates multiple visual elements
- **Memory Management**: Proper cleanup of all child renderers

### Renderer Integration with Backend

#### Automatic Renderer Creation
```csharp
// In Buff class implementing IRenderableState
public virtual void ConstructRenderer()
{
    if (Globals.cc.animation && buffData.alwaysOnIndicator)
    {
        renderer = SimpleSpriteIndicator.Instantiate(
            buffData.alwaysOnIndicator,
            Globals.backend.GridToWorldPosCentered(parentMob.Position), 3)
            .Follow(parentMob);
    }
}

public virtual void UpdateRenderer()
{
    (renderer as FollowMobStateRenderer)?.Follow(parentMob);
}
```

**Integration Patterns:**
- **Conditional Rendering**: Only create renderers when needed (animation enabled, sprite available)
- **Type-Safe Casting**: Safe casting to specific renderer types for extended functionality
- **Fluent Chains**: Method chaining for concise renderer setup

## Data Flow and Interaction Patterns

### Complete Action Execution Flow

The action execution follows a sophisticated multi-stage pipeline:

#### 1. UI Entry Point
- **File**: `Assets/Scripts/UI/States/UnitMenu.cs:162-171`
- Player selects unit → `UnitMenu.PrepareTopMenu()` creates action entries
- **File**: `Assets/Scripts/UI/UIE/UnitMenuController.cs:256-271`
- Action button click triggers `RuntimeAction.RequestInUI()`

#### 2. Target Selection & Validation
```csharp
// In RuntimeAction.RequestInUI()
actionData.Requester.Request(mob, this, onTargetSelected, onCanceled);
while(catchedTarget == null && canceled == false) { yield return null; }
```

#### 3. Cost Calculation & Validation
```csharp
List<Cost> cost = costs.Select(pair =>
    new Cost(dNumber.CreateComposite(pair.Value.Eval((mob, catchedTarget))), pair.Key)).ToList();
yield return new JumpIn(mob.DoAction(this, catchedTarget, cost));
```

#### 4. Action Execution Pipeline
- **File**: `Assets/Scripts/Backend/MobData/MobData.actions.cs:238-276`
- `MobData.DoAction()` orchestrates the complete execution:

1. **Action Chosen**: `ActionPrecheck()` → Triggers `OnActionChosen` event
2. **Cost Validation**: Check all costs via `CheckCost()`
3. **Cost Application**: Apply costs via `ApplyCost()`
4. **Action Begin**: `ActionBegin()` → Triggers `OnActionPrecast` event
5. **Core Execution**: `RuntimeAction.Do()` → `ActionDataSO.OnPerform()`
6. **Action Done**: `ActionDone()` → Triggers `OnActionPostcast` event
7. **Stat Recalculation**: Updates all mob stats and renderer states

#### 5. Custom Action Logic
```csharp
// In specific ActionDataSO implementations
public override IEnumerator OnPerform(RuntimeAction<TSpellTarget> ract, MobData mob, TSpellTarget target)
{
    yield return new JumpIn(damageOrHeal.Do(ract, mob, target));
    yield return new JumpIn(buffApplication.Do(ract, mob, target));
}
```

### Buff Application Flow

1. **Template Definition**: `BuffSO` defines buff properties and behavior
2. **Runtime Creation**: `Buff` instance created with source mob context
3. **Duplicate Detection**: Check for existing buffs with same data and source
4. **Stack/Refresh Logic**: Apply stacking or refresh rules
5. **Event Integration**: Subscribe to mob events for automatic updates
6. **Renderer Creation**: Create visual indicator if specified

### State Synchronization Pattern

```csharp
// Backend state change
mobData.Position = newPosition;

// Automatic renderer update
Globals.backend.TriggerUpdateRenderers();  // Calls UpdateRenderer() on all IRenderableState

// Renderer syncs to new state
public void UpdateRenderer()
{
    (renderer as MobRenderer)?.SyncRendererPosition();
}
```

### Cost Calculation System Deep Dive

**File**: `Assets/Scripts/Backend/SOCore/Value/LuaGetter.cs:197-232`

The `LuaBoundedGetter` system enables dynamic cost calculation with UI preview:

```csharp
[System.Serializable]
public class LuaBoundedGetter<TIn, TBoundIn, TOut> : LuaGetter<TIn, TOut>
{
    public LuaGetter<TBoundIn, TOut> lowerBound, upperBound;

    public (TOut, TOut) PrecalculatedBounds(TBoundIn args)
    {
        if (type == LuaGetterType.STATIC)
            return (staticOut, staticOut);
        else if (type == LuaGetterType.DYNAMIC)
            return (lowerBound.Eval(args), upperBound.Eval(args));
    }
}
```

**Cost Flow in Actions**:
1. **Design Time**: `ActionDataSO.costs` defines cost formulas using `LuaBoundedGetter`
2. **UI Display**: `GetCostBounds()` calculates min/max costs for tooltips
3. **Runtime Execution**: `costs.Eval((mob, target))` calculates actual costs
4. **Mob Events**: `GetModifiedCost()` allows buffs to modify final costs

### Backend Registration and Renderer Timing

**File**: `Assets/Scripts/Backend/Serialization/Databackend.serialization.cs:17-29`

```csharp
public void RegisterState(BackendState state)
{
    Debug.Log($"Registered {state}");
    allStates.Add(state);
    
    // Defer renderer creation to next coroutine frame
    Globals.combatCoroutine.Instance.RequireOnNextFrameEnd(() =>
    {
        if (state.renderer == null)
        {
            (state as IRenderableState)?.ConstructRenderer();
        }
    });
}
```

**Critical Timing**: Renderer creation is deferred to the next coroutine frame to ensure:
- Backend state is fully initialized
- All event subscriptions are established  
- Parent-child relationships are resolved
- Visual components create after logical state is stable

### MobListener Event Architecture

**File**: `Assets/Scripts/Backend/MobListenerSO.cs` & `Assets/Scripts/Backend/StatModifierSO.cs`

#### Core Hierarchy
1. **`MobListenerSO`** (Template): Defines 13 listener types with factory methods
2. **`MobListener`** (Runtime): Base runtime class inheriting from `BackendState`
3. **`StatModifier`** (Specialized): Handles stat modifications with power calculations
4. **`Buff`** (Advanced): Adds stacking, timing, DOT/HOT effects, and visual indicators

#### Event Subscription Patterns

**Basic Pattern** (StatModifier):
```csharp
public override void OnAttach(MobData mob)
{
    base.OnAttach(mob);  // Registers with backend, may create renderer
    mob.OnBaseStatCalculation.AddListener(ModifyBaseStats);
    mob.OnStatCalculation.AddListener(ModifyMoreStats);
}

protected virtual void ModifyBaseStats(MobData m)
{
    RecalculateStats(m);  // Update power values from source mob
    statData.ModifyBaseStats(this, m, statData.modifiers, stacks);
}
```

**Advanced Pattern** (Buff with lifecycle):
```csharp
private IEnumerator BuffBase_OnNextTurn(MobData mob)
{
    OnNextTurn(mob);  // Virtual for derived classes
    
    // Process DOT/HOT effects
    foreach (var dhotinfo in buffData.overtimeEffects)
    {
        yield return new JumpIn(Globals.backend.DealDmgHeal(mob, damageInfo));
    }
    
    // Handle timed buff expiration
    if(buffData.timed && !buffData.phaseTimed)
    {
        StepTimer();  // May call Destroy() if expired
    }
}
```

## Advanced Architecture Concepts

### Serialization and Persistence

The three-level architecture enables sophisticated save/load capabilities:

#### What Gets Serialized
- **ScriptableObject GUIDs**: `CustomIconScriptableObject.Guid` field contains asset GUID
- **BackendState Identity**: State GUIDs maintain object relationships  
- **Essential Runtime State**: Health, buffs, positions, stacks, timeRemain, etc.
- **Power Values**: `dNumber power, auxPower` in buffs (for snapshotting)

#### What Gets Reconstructed  
- **ScriptableObject Loading**: Manual loading from GUIDs needed (currently unimplemented)
- **Calculated Values**: Cost bounds, hit/crit values recalculated on load
- **Renderers**: Visual components recreated through `ConstructRenderer()` 
- **Event Subscriptions**: Runtime listeners re-established via `OnAttach()` calls

#### Deserialization Flow
**File**: `Assets/Scripts/Backend/Serialization/Databackend.serialization.cs:38-104`

1. **Pass 1**: `RestoreFromDeserialization()` on all mobs
2. **Pass 2**: Re-link mob renderers and refresh visual state
3. **Pass 3**: Reuse existing renderers by GUID or create new ones
4. **Cleanup**: Destroy orphaned renderers from previous sessions

### Performance Optimizations

#### Lazy Renderer Creation
```csharp
public void ConstructRenderer()
{
    // Only create renderer if animation is enabled
    if (Globals.cc.animation && buffData.alwaysOnIndicator)
    {
        renderer = CreateIndicatorRenderer();
    }
}
```

#### Batched Updates
```csharp
// Global update trigger updates all renderers at once
Globals.backend.OnGlobalActionPostCast += () => 
{
    foreach(var state in renderableStates)
    {
        state.UpdateRenderer();
    }
};
```

#### Memory Management
- **Automatic Cleanup**: `DestroyRenderer()` called on state destruction
- **Weak References**: Renderers don't hold strong references to prevent cycles
- **Pooling**: Indicator renderers can be pooled for frequent creation/destruction

### Extensibility Patterns

#### Plugin Architecture
New functionality can be added at any level:

```csharp
// New ScriptableObject type
public class CustomActionSO : ActionDataSO<Vector3IntTarget>
{
    public override IEnumerator OnPerform(RuntimeAction<Vector3IntTarget> ract, MobData mob, Vector3IntTarget target)
    {
        // Custom behavior
    }
}

// New runtime state
public class CustomEffect : BackendState, IRenderableState
{
    public void ConstructRenderer() => renderer = new CustomRenderer();
    public void UpdateRenderer() => (renderer as CustomRenderer)?.Update();
}

// New renderer
public class CustomRenderer : IStateRenderer
{
    public void Refresh() { /* Custom visual updates */ }
    public void Destroy() { /* Cleanup */ }
}
```

#### Event-Driven Extensions
The MobListener system allows runtime states to hook into any mob event:

```csharp
public override void OnAttach(MobData mob)
{
    mob.OnDamage.AddListener(OnDamageReceived);
    mob.OnHeal.AddListener(OnHealReceived);
    mob.OnActionPrecast.AddListener(OnActionPrecast);
}
```

## Best Practices and Guidelines

### ScriptableObject Design
- **Pure Functions**: Never modify external state in ScriptableObject methods - seen in all `ActionDataSO.OnPerform()` implementations
- **Immutable Data**: Treat ScriptableObject fields as read-only at runtime
- **GUID Management**: `CustomIconScriptableObject.OnValidate()` automatically maintains asset GUIDs
- **Generic Design**: Use `ActionDataSO<TSpellTarget>` pattern for type-safe target handling

### Runtime State Management
- **Event-Driven**: Subscribe to `MobData` events in `OnAttach()` - pattern seen in all `MobListener` implementations  
- **Power Snapshotting**: Use `StatModifier.snapShot` flag to control whether buffs recalculate power continuously
- **Proper Cleanup**: Always unsubscribe from events in `OnRemove()` - critical for preventing memory leaks
- **Registration Timing**: Call `Register()` in `OnAttach()` to enable renderer creation
- **Serialization Awareness**: Mark calculated values as `[NonSerialized]` - seen in `RuntimeAction.power`, `costBounds`

### Renderer Implementation  
- **Deferred Creation**: Renderers created via `RequireOnNextFrameEnd()` after state registration
- **Conditional Creation**: Only create renderers when needed (`Globals.cc.animation` checks)
- **Null-Safe Updates**: Handle null backend state in `Refresh()` methods
- **Renderer Reuse**: Deserialization attempts to reuse existing renderers by GUID matching

### Integration Patterns
- **Cost Bounds Pattern**: Use `LuaBoundedGetter` for UI preview with `PrecalculatedBounds()`
- **Factory Pattern**: `MobListenerSO.Wrap()` and `LeveledWrap()` create runtime instances
- **Fluent Chaining**: `SimpleSpriteIndicator.Instantiate().Follow(mob)` pattern
- **Event Symmetry**: Every `AddListener()` in `OnAttach()` needs corresponding `RemoveListener()` in `OnRemove()`

## Conclusion

The three-level architecture in miniRAID provides a robust foundation for complex game systems by cleanly separating data definition, runtime behavior, and visual presentation. This separation enables:

- **Maintainable Code**: Clear boundaries between concerns reduce coupling
- **Flexible Design**: New functionality can be added at any level independently
- **Robust Serialization**: Sophisticated save/load without renderer complications  
- **Performance Optimization**: Conditional rendering and batched updates
- **Designer Empowerment**: Data-driven design through ScriptableObjects

The architecture demonstrates advanced software engineering principles applied to game development, creating a system that scales well with project complexity while maintaining clean, understandable code organization.