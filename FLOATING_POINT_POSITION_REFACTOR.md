# Floating-Point Position Refactoring Task

## Overview

Refactor miniRAID's position system from `Vector3Int` to `Vector3` to support sub-grid movement with 1/(2^N) precision increments (e.g., 0.125 units). This requires separating mob storage from the grid-based map system and extending the existing collider system for grid-position overlap detection.

## Current Architecture Analysis

### Existing Position System ✅ 
- **`MobData.Position`**: Already `Vector3` (floating-point support exists!)
- **`MobData.SetPosition(Vector3Int)`**: Method signature constrains to integers despite `Vector3` storage
- **Grid Storage**: Mobs stored directly in `GridData.mob` field, limiting to 1:1 grid-to-mob relationship

### Existing Collider System Status
- **`IGridCollider` Interface** (`Assets/Scripts/Backend/BackendCore/Colliders/GridColliders.cs`):
  ```csharp
  public interface IGridCollider
  {
      bool Overlaps(IGridCollider other);
      void SetPosition(Vector3 position);
      void SetDirection(Consts.Direction dirc);
  }
  ```
- **`EnumerateGridCollider__`**: Broken wrapper with `NotImplementedException`s
- **`GridShape`**: Complete implementation with `HashSet<Vector3Int> shape`, position, direction, and transform logic
- **Built-in Overlap Detection**: Uses efficient .NET `HashSet<T>.Overlaps()` method

### Usage Pattern Analysis ✅
- **Collider Usage**: Needs position, direction, transforms (visual indicators, collision detection)
- **Pure Shape Usage**: Only needs `HashSet<Vector3Int>` for spell area calculations
- **Clear Separation**: Different use cases require different data structures

### Architecture Issues Identified
- **Mob-Grid Coupling**: Direct storage in `GridData.mob` prevents multi-cell occupation
- **Method Signature Mismatch**: `SetPosition(Vector3Int)` vs `Vector3` storage
- **Incomplete Collider**: `EnumerateGridCollider__` unusable, `GridShape` has all needed functionality
- **Missing Grid Queries**: No `OverlapsGrid()` method for dynamic mob-to-grid lookup

## Implementation Status

### ✅ Completed: Enhanced Collider System

#### New `IGridCollider` Interface
```csharp
public interface IGridCollider
{
    bool Overlaps(IGridCollider other);
    bool OverlapsGrid(Vector3Int gridCell);  // NEW: For dynamic grid lookup
    
    void SetPosition(Vector3 position);
    void SetDirection(Consts.Direction dirc);
}
```

#### ✅ `EnumerateGridCollider` Implementation
- **Merged `GridShape` completely**: All functionality integrated
- **Floating-point positions**: Uses `Vector3 position` internally
- **Grid overlap detection**: `OverlapsGrid()` method implemented using `ApplyTransform().Contains()`
- **Transform support**: Maintains direction, rotation, and positioning logic
- **Efficient collision**: Uses `HashSet<T>.Overlaps()` for collider-to-collider tests

#### Key Implementation Details
```csharp
// Position uses floating-point, transforms to grid coordinates when needed
private Vector3Int ApplyTransformToPoint(Vector3Int p)
{
    Vector3Int basePos = new Vector3Int(
        Mathf.FloorToInt(position.x), 
        Mathf.FloorToInt(position.y), 
        Mathf.FloorToInt(position.z)
    );
    // ... direction transforms
}

// Grid overlap check for GetMap() queries
public bool OverlapsGrid(Vector3Int gridCell)
{
    var occupiedGrids = ApplyTransform();
    return occupiedGrids.Contains(gridCell);
}
```

### Next Phase: Remaining Tasks

#### 1. Position System Changes
- **Change `SetPosition(Vector3Int)` to `SetPosition(Vector3)`**: Remove integer constraint
- **Update callers**: Fix all code using the old method signature

#### 2. Mob Storage Separation  
- **Add `Databackend.allMobs`**: `List<MobData>` for all active mobs
- **Remove `GridData.mob`**: Eliminate direct grid storage
- **Refactor `GetMap()`**: Use `mob.body.OverlapsGrid(gridCell)` for dynamic lookup

#### 3. Non-Collider Shape Usage
- **Replace `GridShape` with `HashSet<Vector3Int>`**: Where only shape data needed (spell areas)
- **Update `CaptureTargetsInGridShape`**: Use direct hash set operations

### Future Collider Extensions
#### Planned Collider Types
- **`SphereGridCollider`**: Manhattan distance calculations, no position enumeration
- **`BoxGridCollider`**: Rectangular bounds with efficient overlap math
- **`CapsuleGridCollider`**: For elongated entities

#### Design Benefits
- **Efficient Grid Queries**: `OverlapsGrid()` enables O(1) lookups for spheres
- **No Position Iteration**: Sphere colliders compute distance mathematically
- **Extensible**: Interface supports both enumeration and calculation-based colliders

### 4. Multi-Cell Mob Support
- Large mobs (dragons, bosses) can occupy multiple grid cells simultaneously
- `GetMap(x, y, z).mob` returns any mob whose collider overlaps that cell
- Multiple `GetMap()` calls for the same large mob will return the same `MobData` instance

## Implementation Plan

### Phase 1: Complete Collider System
1. **Extend `IGridCollider` interface** with grid-position query methods
2. **Replace `EnumerateGridCollider__`** with proper implementation using `GridShape`
3. **Implement grid overlap methods** using existing `GridShape.ApplyTransform()` logic
4. **Add unit tests** for collider overlap detection

### Phase 2: MobData Position Changes  
1. **Change `SetPosition(Vector3Int)`** to `SetPosition(Vector3)`
2. **Add `MobData.body`** property of type `IGridCollider`
3. **Update position validation** to support sub-grid coordinates
4. **Modify position-related events** and coroutines

### Phase 3: Databackend Refactoring
1. **Add `List<MobData> allMobs`** to `Databackend`
2. **Remove `GridData.mob`** field  
3. **Refactor `GetMap()`** to dynamically find overlapping mobs:
   ```csharp
   public GridData GetMap(int x, int y, int z)
   {
       var gridData = // ... existing terrain/effects logic
       gridData.mob = allMobs.FirstOrDefault(m => 
           m.body.OverlapsGrid(new Vector3Int(x, y, z)));
       return gridData;
   }
   ```
4. **Update `MoveMob()`** methods to work with new storage system

### Phase 4: Integration & Testing
1. **Update all `Vector3Int` position references** throughout codebase
2. **Fix compilation errors** in dependent systems  
3. **Test mob movement**, collision detection, and combat positioning
4. **Verify large mob behavior** (multi-cell occupation)

## Technical Considerations

### Performance
- **Query Frequency**: 100-300 collision checks per action expected
- **Mob Count**: 10-20 simultaneous mobs - linear search acceptable
- **HashSet Overlaps**: Leverages efficient .NET built-in method
- **Optimization Later**: Can add spatial partitioning if needed

### Precision & Floating Point
- **Safe Increments**: Use 1/(2^N) values to avoid floating-point precision issues
- **Grid Alignment**: Ensure mob positions align properly with grid boundaries  
- **Comparison Tolerance**: May need epsilon-based comparisons for edge cases

### Existing Code Integration
- **GridShape Reuse**: Leverage existing `GridShape` class and its `ApplyTransform()` method
- **Built-in HashSet.Overlaps**: Continue using .NET's efficient overlap detection
- **Direction System**: Maintain compatibility with existing `Consts.Direction` enum

### Backward Compatibility
- **Grid-Based Logic**: Core grid system remains unchanged
- **ScriptableObject Assets**: Existing mob/action data should work unchanged
- **Event System**: Position-related events need parameter type updates

## Files to Modify

### New/Updated Files
- `Assets/Scripts/Backend/BackendCore/Colliders/IGridCollider.cs` - Extended interface
- `Assets/Scripts/Backend/BackendCore/Colliders/EnumerateGridCollider.cs` - Complete implementation
- `Assets/Scripts/Backend/BackendCore/Colliders/SphereGridCollider.cs` - New collider type

### Modified Files  
- `Assets/Scripts/Backend/MobData/MobData.cs` - Position method signature changes
- `Assets/Scripts/Backend/Databackend.cs` - Mob storage refactoring
- All files referencing `SetPosition(Vector3Int)` or `MobData.Position` as integer
- Movement and positioning-related UI components
- Action/spell targeting systems using positions

## Success Criteria
- [ ] Mobs can be positioned at sub-grid coordinates (e.g., 1.125, 2.75, 3.0)
- [ ] Large mobs properly occupy multiple grid cells
- [ ] `GetMap()` correctly identifies overlapping mobs  
- [ ] Combat positioning and spell targeting work correctly
- [ ] No performance regression in collision detection
- [ ] All existing gameplay mechanics function properly
- [ ] `EnumerateGridCollider` fully implements `IGridCollider` interface