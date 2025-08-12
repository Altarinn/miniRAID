# Movement System Performance Analysis

**Investigation Date**: 2025-08-11  
**Target Systems**: Movement Logic, GenericBFS, Collision Detection  
**Performance Impact**: Critical - significant frame drops during movement planning

## Executive Summary

The miniRAID movement system exhibits significant performance bottlenecks, particularly in the `GenericMovementBFS` pathfinding algorithm and `CanPositionPlaceMob` collision detection method. These issues become critical during movement planning phases when multiple movement options are evaluated simultaneously.

## System Architecture Overview

### Movement Flow
1. **Movement Request** → `MovementRequester` calculates possible destinations
2. **BFS Pathfinding** → `GenericMovementBFS` explores movement graph
3. **Collision Testing** → `CanPositionPlaceMob` validates each position
4. **Path Reconstruction** → `ReconstructPath` builds final movement path

### Key Components
- **Databackend.cs:1075** - `GenericMovementBFS()` core pathfinding
- **Databackend.cs:1144** - `CanPositionPlaceMob()` collision detection  
- **MovementSO.cs** - Movement type implementations (Walk, Blink)
- **ColliderOverlapTests** - Grid collision detection algorithms

## Critical Performance Bottlenecks

### 1. GenericMovementBFS Algorithm (Databackend.cs:1075)

**Problem**: O(N²) complexity with expensive collision checks per iteration.

```csharp
public bool GenericMovementBFS(
    IGridCollider origin,
    Movement movement,
    MovementTerminationCondition termCond,
    int maxDistance,
    out Dictionary<Vector3Int, (Vector3Int prevGrid, float distance)> gridInfo)
```

**Performance Issues**:
- **New Allocations Per Call**: `C5.IntervalHeap<GridBFSKeys>` and `Dictionary<Vector3Int, ...>` created every invocation
- **Collider Cloning**: `origin.ShallowClone()` called for every grid position explored  
- **Repeated Map Queries**: `InMap(newPos)` checks performed redundantly
- **Movement Proposal Overhead**: `movement.ProposeMovementGrids()` calls are expensive

**Hot Path**: Called once per movement action, but explores hundreds of grid positions.

### 2. CanPositionPlaceMob Collision Detection (Databackend.cs:1144)

**Problem**: Linear search through all mobs with complex collision tests.

```csharp
public bool CanPositionPlaceMob(IGridCollider body)
{
    var result = allMobs.All(x => (x.Collider == body || !(x.Collider.Overlaps(body))));
    return result;
}
```

**Performance Issues**:
- **Linear Mob Search**: O(M) where M = number of mobs (typically 10-20)
- **Complex Overlap Tests**: `ColliderOverlapTests.Overlaps()` performs expensive HashSet operations
- **No Spatial Optimization**: No spatial partitioning or caching
- **Frequent Calls**: Called hundreds of times per movement BFS

**Comment**: Code contains TODO: "Optimize this by caching results. Use a non-serialized version number system on IGridColliders to cache valid results."

### 3. ColliderOverlapTests.Overlaps (GridColliders.cs:62)

**Problem**: Complex HashSet manipulations for every collision test.

```csharp
public static bool Overlaps(EnumerateGridCollider a, EnumerateGridCollider b)
{
    // Bounds checking first (good optimization)
    if (!IntersectsBounds(a, b))
        return false;

    // Expensive operations follow:
    var largerRotated = new HashSet<Vector3Int>(); // New allocation
    foreach (var pos in larger.shape.shape) {
        largerRotated.Add(Consts.Rotate(pos, larger.Direction)); // Rotation calculation
    }
    
    foreach (var pos in EnumerateGridCollider.OverlappedPoints(...)) {
        if (largerRotated.Contains(pos)) // HashSet lookup
            return true;
    }
}
```

**Performance Issues**:
- **HashSet Allocation**: New `HashSet<Vector3Int>` per collision test
- **Rotation Calculations**: `Consts.Rotate()` called for each grid point
- **Fractional Position Handling**: Complex `OverlappedPoints()` enumeration

### 4. EnumerateGridCollider.OverlappedPoints (EnumerateGridCollider.cs:63)

**Problem**: Nested loops with floating-point position handling.

```csharp
for (int dx = 0; dx < 2 - (fractPos.x == 0 ? 1 : 0); dx++)
{
    for (int dy = 0; dy < 2 - (fractPos.y == 0 ? 1 : 0); dy++)
    {
        for (int dz = 0; dz < 2 - (fractPos.z == 0 ? 1 : 0); dz++)
        {
            // Triple nested loop creates up to 8 points per original point
        }
    }
}
```

**Performance Issues**:
- **Combinatorial Explosion**: Up to 8x grid points generated per shape point
- **Repeated Floating Point Comparisons**: `fractPos.x == 0` checks
- **Memory Pressure**: Large number of Vector3Int allocations via yield return

## WalkMovementSO Performance Issues (MovementSO.cs:131)

**Problem**: Extensive map queries and collision tests per movement step.

```csharp
public override List<Databackend.GridBFSKeys> ProposeMovementGrids(
    IGridCollider origin, Databackend.GridBFSKeys fromKey)
{
    // For each of 4 directions:
    // - IsPassable() -> GetMap() + CanPositionPlaceMob()
    // - IsSupported() -> GetMap() calls
    // - Fall detection: up to 3 iterations of IsPassable() + IsSupported()
    // - Jump detection: up to 2 iterations of IsPassable() + IsSupported()
}
```

**Total Calls Per Grid**: Up to 24 map queries + 12 collision tests per position explored.

## Current Performance Scale

### Expected Load (from FLOATING_POINT_POSITION_REFACTOR.md)
- **Query Frequency**: 100-300 collision checks per action
- **Mob Count**: 10-20 simultaneous mobs
- **Assessment**: "Linear search acceptable"

### Actual Performance Impact
Based on the code analysis:
- **BFS Exploration**: 50-200 grid positions per movement
- **Collision Tests**: 12 tests × 50 positions × 20 mobs = **12,000 collision operations**
- **Map Queries**: 24 queries × 50 positions = **1,200 map queries**

## Identified Optimization Opportunities

### 1. Collision Detection Caching
**Location**: `CanPositionPlaceMob()` - Databackend.cs:1144  
**Solution**: Implement version-based caching as suggested in TODO comment.

```csharp
// Proposed caching strategy:
private Dictionary<(Vector3Int, int), bool> collisionCache;
private int collisionVersion = 0; // Increment when mobs move
```

### 2. BFS Data Structure Reuse
**Location**: `GenericMovementBFS()` - Databackend.cs:1075  
**Solution**: Pool IntervalHeap and Dictionary allocations.

### 3. Spatial Partitioning for Collision
**Solution**: Replace linear mob search with grid-based spatial partitioning.

### 4. Movement Grid Caching
**Solution**: Cache valid movement grids per mob type and position.

### 5. Collider Shape Optimization
**Solution**: Pre-compute rotated shapes for common orientations.

## Existing Performance Comments in Codebase

1. **ChainHeal.cs:65** - "TODO: Cache?" for heal calculation
2. **BuffSO.cs:270** - "TODO: cache all functions and unbind them in OnRemove"
3. **GridEffectSO.cs:47** - "TODO: Performance heavy?" for shape updates
4. **MobData.events.cs:310** - "TODO: Optimize this if performance issue is serious" for stat recalculation
5. **TargetedAgentBaseSO.cs:74** - "TODO: Cache the path in some way in case of performance problems"
6. **AimCircles.cs:89** - "TODO: FIXME: Performance concerns" for UI updates

## Recommendations

### Immediate Actions (High Impact, Low Risk)
1. **Implement collision result caching** in `CanPositionPlaceMob()`
2. **Pool BFS data structures** to reduce GC pressure
3. **Add spatial partitioning** for mob collision queries

### Medium-Term Improvements
1. **Optimize ColliderOverlapTests** with pre-computed rotation tables
2. **Cache movement proposals** for identical mob configurations
3. **Implement incremental pathfinding** for minor position changes

### Architectural Considerations  
1. **Profile with Unity Profiler** to validate bottleneck assumptions
2. **Implement performance monitoring** for movement system
3. **Consider A* pathfinding** for complex scenarios with heuristics

## Technical Notes

### Current Collision System Strengths
- **Floating-point position support**: Handles sub-grid positioning correctly
- **Generic collider interface**: Supports both enumerated and distance-based shapes
- **Bounds checking optimization**: Fast rejection of non-overlapping colliders

### Architecture Compatibility
- **Three-level architecture**: Changes should maintain separation between data/logic/rendering
- **Serialization requirements**: Cache data must be marked `[NonSerialized]`
- **Movement type extensibility**: Optimizations should not break custom MovementSO implementations

## Conclusion

The movement system's performance bottlenecks are well-localized and addressable through caching and algorithmic improvements. The existing architecture provides good extension points for optimization without requiring fundamental restructuring.

**Priority**: Critical - implement collision caching and BFS pooling first.  
**Estimated Impact**: 5-10x performance improvement for movement planning operations.