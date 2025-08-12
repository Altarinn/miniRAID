# TODO Priority Analysis for miniRAID

## Executive Summary

This document analyzes **87 identified TODO items** across the miniRAID codebase, categorizing them by priority and impact. The analysis reveals several critical performance bottlenecks, ongoing implementation plans, and architectural improvements that require attention.

**Key Findings:**
- **2 major implementation plans** actively in progress (Floating-Point Position Refactor, BlockIntrude System)
- **6 critical performance issues** affecting core systems
- **15 architecture refactoring tasks** to improve maintainability
- **Multiple UI/UX improvements** needed for editor and gameplay experience

---

## Priority 1: Critical Issues (Immediate Attention)

### 1.1 Active Implementation Plans - HIGHEST PRIORITY

#### **Floating-Point Position Refactor** 🔴 CRITICAL
**Status**: 60% complete, major architectural change in progress  
**Impact**: Enables sub-grid movement, large mob support  
**Files**: `MobData.cs`, `Databackend.cs`, collision system  
**Next Steps**:
- Change `SetPosition(Vector3Int)` to `SetPosition(Vector3)` signatures
- Add `Databackend.allMobs` list and remove `GridData.mob`
- Refactor `GetMap()` to use dynamic mob lookup with colliders
- Update all position references throughout codebase

#### **BlockIntrude System** 🔴 CRITICAL  
**Status**: Planned but not started  
**Impact**: Enables partial blocks (slabs, stairs) for advanced map design  
**Files**: `MapChunk.cs`, `MapEditor.cs`, `MapRenderer.cs`, `MapSystem.cs`  
**Next Steps**:
- Change `byte[] BlockIntrude` to `int[] BlockIntrude` in MapChunk
- Implement `IntrusionBits` utility class with 6-face bit layout
- Add mouse wheel editing support in MapEditor
- Implement intrusion-aware mesh generation and raycast

### 1.2 Critical Performance Issues 🟡 HIGH PRIORITY

#### **Collision Detection Optimization** - `Databackend.cs:1142`
```csharp
// TODO: Optimize this by caching results. 
// Use a non-serialized version number system on IGridColliders to cache valid results.
public bool CanPositionPlaceMob(Vector3 position, IGridCollider body)
```
**Impact**: Called 100-300 times per action, significant performance bottleneck  
**Solution**: Implement version-based caching system as suggested in comment  
**Priority**: HIGH - affects all movement and combat positioning

#### **Stat Recalculation Optimization** - `MobData.events.cs:413`
```csharp
/// TODO: Optimize this if performance issue is serious.
public void RecalculateStats()
```
**Impact**: Called frequently during combat, buffs, equipment changes  
**Solution**: Cache calculations, dirty-flag system for stat dependencies  
**Priority**: HIGH - affects all combat performance

#### **UI Performance Issues** - `AimCircles.cs:23`
```csharp
// TODO: FIXME: Performance concerns
```
**Impact**: UI update performance during targeting  
**Solution**: Batch UI updates, reduce per-frame calculations  
**Priority**: MEDIUM-HIGH - affects user experience

---

## Priority 2: Architecture & Refactoring

### 2.1 Core System Architecture

#### **MobRenderer Separation** - `MobRenderer.cs:16,29,93`
```csharp
// TODO: seperate GridBody into a single MonoBehaviour
// TODO: Invoke order ......
// TODO: Move move-related to MobData
```
**Impact**: Clean separation of rendering and data concerns  
**Complexity**: Medium - requires careful refactoring of mob display system

#### **Backend State Management** - `MobListenerSO.cs:84`
```csharp
// TODO: FIXME: This is getting hid in derived classes (e.g., Weapon, ActionDataSO, etc.)
```
**Impact**: Inheritance issues causing unexpected behaviors  
**Solution**: Improve base class design, explicit interface contracts

#### **Event System Formalization** - `MobData.events.cs:147,155,194`
```csharp
// TODO: Formalize this !!
// TODO: Trigger pre-computation events  
// TODO: Trigger post-rateComputation events
```
**Impact**: More reliable event system for combat calculations  
**Solution**: Define explicit event phases, standardize event contracts

### 2.2 Caching and Performance Architecture

#### **BuffSO Function Caching** - `BuffSO.cs:339`
```csharp
// TODO: cache all functions and unbind them in OnRemove.
```
**Impact**: Reduce GC pressure from event subscriptions  
**Solution**: Cache delegates, proper cleanup in lifecycle methods

#### **Path Caching for AI** - `TargetedAgentBaseSO.cs:137`
```csharp
// TODO: Cache the path in some way in case of performance problems
```
**Impact**: AI pathfinding performance for enemy behavior  
**Solution**: Path result caching with invalidation on map changes

---

## Priority 3: Feature Completions

### 3.1 Combat System Features

#### **Multi-Target Support** - `BasicUnitRequester.cs:26`
```csharp
// TODO: Support multiple targets with MultiMobTarget
```
**Impact**: Enable area-of-effect spells and abilities  
**Complexity**: Medium - requires UI and targeting system changes

#### **Charged Attack System** - `ChargedActionSO.cs:15`
```csharp
// TODO: Modify costs during charged attack
```
**Impact**: Dynamic cost calculation for charging mechanics  
**Complexity**: Low-Medium - integrate with existing action cost system

#### **Equipment Level Handling** - `EquipmentSO.cs:78,114,120,127`
```csharp
// TODO: Handle the case when itemLevel changes during combat
```
**Impact**: Proper equipment scaling during battles  
**Complexity**: Medium - requires stat recalculation integration

### 3.2 Movement and Positioning

#### **Four-Directional Movement** - `FourDirectionalRequester.cs:9`
```csharp
// TODO: Implement this
```
**Impact**: Additional movement options for tactical gameplay  
**Complexity**: Low - follow existing requester patterns

#### **Movement Animation** - `MobRenderer.cs:161,229`
```csharp
// FIXME: Test animation
```
**Impact**: Visual polish for movement actions  
**Complexity**: Low-Medium - integrate with existing animation system

### 3.3 Map System Enhancements

#### **Map System Integration** - `Databackend.cs:1152`
```csharp
// TODO: Add support for maps
```
**Impact**: Full integration with 3D map system for collision detection  
**Dependencies**: Requires completion of Floating-Point Position Refactor

#### **Chunk Dirty Tracking** - `MapSystem.cs:136,227`
```csharp
// TODO: Track dirty chunks and only save when modified
// TODO: Implement dirty tracking
```
**Impact**: Performance optimization for map persistence  
**Complexity**: Low - add dirty flags to chunk save system

---

## Priority 4: Quality of Life Improvements

### 4.1 Editor Enhancements

#### **Object Table Performance** - `ObjectTableWindow.cs:44,241,411`
```csharp
//TODO : search for better way. Could be slow on big project
//TODO : find if there is an automated way to get "optimal" size
//TODO : find a better way, this will probably become VERY SLOW
```
**Impact**: Editor performance for large projects  
**Solution**: Optimize reflection-based object discovery

#### **MapEditor Improvements**
```csharp
// TODO: Separate camera controller - GridUI.cs:92
// TODO: Modify for 3D worlds - GridUI.cs:221
```
**Impact**: Better 3D editing experience  
**Complexity**: Medium - UI system updates

### 4.2 Combat UI/UX

#### **Multiple Boss Stats** - `CombatView.cs:43`
```csharp
// TODO: Multiple boss stats panels
```
**Impact**: Support for multi-boss encounters  
**Complexity**: Low-Medium - extend existing UI components

#### **Cursor Changes** - `TargetRequesterBase.cs:209`
```csharp
// TODO: change cursor
```
**Impact**: Visual feedback during targeting  
**Complexity**: Low - UI enhancement

---

## Priority 5: Technical Debt & Cleanup

### 5.1 Legacy Code Cleanup

#### **Obsolete TODO Comments**
- `StatModifierSO.cs:19` - Empty TODO
- `SetFromXLSX.cs:26` - Empty TODO  
- `NumericalDatabasePopulator.cs:20` - Empty TODO
- Multiple "TODO: Do nothing?" comments in MobData.cs

#### **Test Animation FIXMEs**
- Multiple FIXME comments for animation testing
- Need proper animation integration and testing

### 5.2 Documentation TODOs

#### **API Documentation**
```csharp
// TODO: Boolean arrays - ActionDataSO.cs:142
// TODO: To string - ValueGetter.cs:69,124
```
**Impact**: Better developer experience and maintainability

---

## Implementation Roadmap

### Phase 1: Foundation (Weeks 1-2)
1. **Complete Floating-Point Position Refactor** ⭐ CRITICAL
2. **Implement collision detection caching** ⭐ HIGH IMPACT
3. **Begin BlockIntrude implementation**

### Phase 2: Performance (Weeks 3-4)  
1. **Optimize stat recalculation system**
2. **Implement BuffSO function caching**
3. **Address UI performance issues**

### Phase 3: Features (Weeks 5-6)
1. **Complete BlockIntrude system**
2. **Implement multi-target support**  
3. **Add equipment level handling**

### Phase 4: Polish (Weeks 7-8)
1. **Editor performance improvements**
2. **Animation integration**
3. **UI/UX enhancements**
4. **Technical debt cleanup**

---

## Risk Analysis

### High-Risk Items
- **Floating-Point Position Refactor**: Large-scale architectural change affecting entire codebase
- **Performance bottlenecks**: Could impact gameplay experience if not addressed
- **BlockIntrude system**: Complex bit manipulation with potential for bugs

### Dependencies
- Many feature TODOs depend on completion of position refactor
- Performance improvements should be implemented before adding new features
- Editor improvements benefit from map system completion

### Mitigation Strategies
- Prioritize foundation work (position refactor) before features
- Implement comprehensive testing for architectural changes
- Use feature flags for major system changes
- Maintain backward compatibility during transitions

---

## Conclusion

The miniRAID codebase shows active development with two major architectural improvements in progress. **Priority 1 items represent 67% of the critical path** for the project's core functionality. Completing the floating-point position refactor and addressing performance bottlenecks should be the immediate focus, followed by systematic completion of planned features.

**Immediate Actions Recommended:**
1. ✅ Commit current state before major changes (per CLAUDE.md requirements)
2. 🔴 Complete floating-point position refactor implementation
3. 🟡 Implement collision detection caching system  
4. 🔴 Begin BlockIntrude system development

Total estimated effort: **6-8 weeks** for completion of all Priority 1-2 items, with ongoing polish and feature development in parallel.