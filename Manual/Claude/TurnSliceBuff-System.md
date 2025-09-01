# TurnSliceBuff System Architecture

## Overview

The TurnSliceBuff system is a specialized extension of the existing Buff system that allows buffs to terminate their associated turn slices when certain conditions are met. This enables complex mechanics where ongoing actions can be interrupted by accumulated damage, time limits, or other triggers.

## Architecture

### Core Classes

```csharp
// Base ScriptableObject for Inspector integration
public abstract class TurnSliceBuffSO : BuffSO
{
    protected abstract TurnSliceBuff CreateTurnSliceBuff(MobData parent);
}

// Runtime implementation with turn slice termination capability
public abstract class TurnSliceBuff : Buff
{
    protected PreparableActionTurnSlice associatedTurnSlice;
    
    public virtual void AssociateWithTurnSlice(PreparableActionTurnSlice turnSlice);
    protected virtual void TerminateAssociatedTurnSlice();
}
```

### Integration Points

**PreparableActionTurnSliceSO Extension:**
```csharp
public List<TurnSliceBuffSO> turnSliceBuffs = new List<TurnSliceBuffSO>();

protected virtual void ApplyTurnSliceBuffs(PreparableActionTurnSlice slice)
{
    foreach (var buffSO in turnSliceBuffs)
    {
        var buffInstance = slice.mob.AddListener(buffSO);
        if (buffInstance is TurnSliceBuff turnSliceBuff)
        {
            turnSliceBuff.AssociateWithTurnSlice(slice);
        }
    }
}
```

## Design Patterns Applied

### 1. Template Method Pattern
- `TurnSliceBuffSO.CreateTurnSliceBuff()` is an abstract factory method
- Subclasses implement specific buff creation logic
- Follows existing `BuffSO.Wrap()` pattern

### 2. Strategy Pattern
- Different termination conditions implemented as different TurnSliceBuff subclasses
- Same interface (`TerminateAssociatedTurnSlice()`) with different implementations
- Easily extensible for new termination mechanics

### 3. Observer Pattern Extended
- Builds on existing event-driven Buff system
- TurnSliceBuff observes game events (damage, time, etc.)
- Can notify turn slice to terminate when conditions met

### 4. Composition over Inheritance
- Extends existing systems rather than replacing them
- TurnSliceBuff "has-a" relationship with TurnSlice (association)
- Reuses all existing Buff functionality

## Implementation Example: RoarOverloadBuff

### Configuration (ScriptableObject)
```csharp
[CreateAssetMenu(menuName = "miniRAID/TurnSliceBuffs/RoarOverloadBuff")]
public class RoarOverloadBuffSO : TurnSliceBuffSO
{
    [Title("Overload Settings")]
    public float damageThreshold = 100f;
    public SimpleExplosionFx explosionFx;
    public SpellDamageHeal selfDamage;
    
    protected override TurnSliceBuff CreateTurnSliceBuff(MobData parent)
    {
        return new RoarOverloadBuff(parent, this);
    }
}
```

### Runtime Logic
```csharp
public class RoarOverloadBuff : TurnSliceBuff
{
    private RoarOverloadBuffSO BuffData => (RoarOverloadBuffSO)data;
    
    public override void OnAttach(MobData mob)
    {
        base.OnAttach(mob);
        mob.OnDamageReceived.AddListener(OnReceiveDamage);
    }
    
    public IEnumerator OnReceiveDamage(MobData mob, DamageHeal_Result info)
    {
        damageAccumulated += info.value;
        
        if (damageAccumulated >= BuffData.damageThreshold)
        {
            // Play effects
            yield return BuffData.explosionFx?.Do(mob.Position);
            yield return BuffData.selfDamage?.Do(null, mob, mob);
            
            // Terminate the turn slice
            TerminateAssociatedTurnSlice();
            RemoveFromMob();
        }
    }
}
```

## Key Benefits

### 1. Modularity
- TurnSliceBuffs can be reused across different turn slices
- No need for custom turn slice classes for each mechanic
- Configuration-driven through ScriptableObject assets

### 2. Maintainability
- Clear separation between configuration and runtime logic
- Follows established codebase patterns
- Easy to debug and extend

### 3. Designer Friendly
- All configuration exposed in Inspector
- Can be applied to any PreparableActionTurnSlice via drag-and-drop
- Visual feedback through buff UI system

### 4. Performance
- No additional overhead beyond regular buffs
- Event-driven, only processes when relevant events occur
- Automatic cleanup when conditions met or turn slice ends

## Usage Guidelines

### When to Use TurnSliceBuff
- Actions that can be interrupted by game events
- Time-limited or resource-limited abilities
- Conditional termination based on damage, healing, movement, etc.
- Any mechanic where a buff needs to control its associated turn slice lifetime

### When NOT to Use TurnSliceBuff
- Simple stat modifications (use regular Buff)
- Effects that should persist beyond the turn slice
- One-shot effects that don't need ongoing monitoring

### Implementation Steps
1. Create `YourBuffSO : TurnSliceBuffSO` with configuration fields
2. Create `YourBuff : TurnSliceBuff` with termination logic
3. Add buff to `PreparableActionTurnSliceSO.turnSliceBuffs` list
4. Create ScriptableObject assets with appropriate configuration

## Future Extensions

### Potential Enhancements
- **Conditional Association**: Only associate with certain turn slice types
- **Multiple Termination Conditions**: AND/OR logic for complex triggers
- **Termination Effects**: Standardized effects when turn slices are terminated
- **Turn Slice Communication**: Buffs that can modify turn slice behavior before termination

### Related Systems
- Could be extended for **Preparable Action Modifiers** (change action parameters)
- Integration with **Turn Schedule System** for complex timing mechanics
- **Conditional Turn Insertion** based on buff states