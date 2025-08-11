# BlockIntrude Implementation Plan

## Overview
Add BlockIntrude functionality to MapSystem with bit-wise representation for partial block occupancy (slabs, stairs, etc.).

**Target**: 3-bit intrusion levels (0-7) for each face using layout: `X+X-Y+Y-Z+Z-`  
**Example**: Bottom slab Y+ = `0b000000100000000000` (Y+ face intrusion level 4)

---

## Phase 1: Data Structure Changes
**Files**: `MapChunk.cs`

### ✅ TODO: Change byte[] to int[] 
- [ ] **Line 48**: Replace `public byte[] BlockIntrude` with `public int[] BlockIntrude`
- [ ] **Line 65**: Update constructor initialization from `BlockIntrude[i] = 0;` (byte) to `BlockIntrude[i] = 0;` (int)

### ✅ TODO: Add IntrusionBits utility class
```csharp
public static class IntrusionBits
{
    // Bit layout: [17-15][14-12][11-9][8-6][5-3][2-0] = Z-,Z+,Y-,Y+,X-,X+
    private const int X_POS_SHIFT = 0;   // bits 0-2
    private const int X_NEG_SHIFT = 3;   // bits 3-5  
    private const int Y_POS_SHIFT = 6;   // bits 6-8
    private const int Y_NEG_SHIFT = 9;   // bits 9-11
    private const int Z_POS_SHIFT = 12;  // bits 12-14
    private const int Z_NEG_SHIFT = 15;  // bits 15-17
    private const int FACE_MASK = 0x7;   // 3 bits (111 binary)
    
    public static int GetFaceIntrusion(int intrusionData, Vector3Int faceNormal);
    public static int SetFaceIntrusion(int intrusionData, Vector3Int faceNormal, int level);
    private static int GetShiftForFace(Vector3Int faceNormal);
}
```

### ✅ TODO: Add getter/setter methods
- [ ] Add `GetBlockIntrude(int x, int y, int z)` method
- [ ] Add `SetBlockIntrude(int x, int y, int z, int intrusionData)` method

---

## Phase 2: MapEditor Integration (SIMPLIFIED)
**Files**: `MapEditor.cs`

### ✅ TODO: Add global mouse wheel support
- [ ] **Add editor state**: `private int currentYPlusIntrusionLevel = 0;`
- [ ] **In OnGUI()**: Add intrusion level slider display (readonly, shows current wheel level)
- [ ] **In HandleMouseInput()**: Add scroll wheel handler that modifies `currentYPlusIntrusionLevel`
  - Scroll Up: Increase level (0→1→2...→7)
  - Scroll Down: Decrease level (7→6→5...→0)
  - Works in ALL paint modes, not just a specific mode

### ✅ TODO: Integrate with existing paint system
- [ ] **Modify ApplyPaint()**: After existing paint logic, ALWAYS apply current Y+ intrusion level
```csharp
void ApplyPaint(MapChunk chunk, int x, int y, int z)
{
    // Existing paint logic (terrain, solid, standable, etc.)
    switch (editMode) { /* all existing cases remain unchanged */ }
    
    // NEW: Always apply current Y+ intrusion level to painted blocks
    if (currentYPlusIntrusionLevel > 0)
    {
        int currentIntrusion = chunk.GetBlockIntrude(x, y, z);
        int newIntrusion = IntrusionBits.SetFaceIntrusion(currentIntrusion, Vector3Int.up, currentYPlusIntrusionLevel);
        chunk.SetBlockIntrude(x, y, z, newIntrusion);
    }
}
```

### ✅ TODO: Visual feedback in scene view
- [ ] **Modify DrawGizmos()**: Show current Y+ intrusion level as overlay text
- [ ] Display intrusion preview on selected block with cyan wireframe

---

## Phase 3: MapRenderer Changes
**Files**: `MapRenderer.cs`

### ✅ TODO: Add intrusion mesh generation
- [ ] **Modify AddCubeWithFaces()**: Check for intrusion data and branch to new method
- [ ] **Add AddIntrudedBlockMesh()**: Generate custom geometry for intruded blocks
- [ ] **Add CalculateIntrudedBounds()**: Apply intrusion levels to shrink block bounds
- [ ] **Add AddBoxMesh()**: Generate mesh for arbitrary box with face culling

### ✅ TODO: Integration points
- [ ] Maintain existing face culling optimization for performance
- [ ] Support color coding for intruded vs full blocks
- [ ] Handle edge cases (all faces intruded = no volume)

---

## Phase 4: DDA Algorithm Updates  
**Files**: `MapSystem.cs`

### ✅ TODO: Add intrusion-aware raycast
- [ ] **Add DDAGridRaycastIntrusion()**: New method that considers intrusion data
- [ ] **Add IsRaycastTargetWithIntrusion()**: Check if ray hits intruded volume
- [ ] **Add RayIntersectsIntrudedBlock()**: Ray-AABB intersection math
- [ ] **Add RayAABBIntersect()**: Standard ray-box intersection utility

### ✅ TODO: Integration strategy
- [ ] Keep existing `DDAGridRaycast()` for backward compatibility
- [ ] Add optional parameter to enable intrusion checking
- [ ] Update UI raycast calls to use intrusion-aware version

---

## Phase 5: Testing & Integration

### ✅ TODO: Unit tests
- [ ] Test `IntrusionBits` utility functions with various bit patterns
- [ ] Verify bit layout matches specification (Y+ level 4 = `0b000000100000000000`)
- [ ] Test edge cases (all 0s, all 7s, mixed levels)

### ✅ TODO: Editor testing
- [ ] Create test blocks with mouse wheel in different paint modes
- [ ] Verify intrusion level changes with scroll wheel
- [ ] Test rendering of various intrusion combinations
- [ ] Validate raycast selection works with intruded blocks

### ✅ TODO: Performance validation
- [ ] Measure mesh generation time for intruded vs full blocks
- [ ] Profile raycast performance with intrusion checking
- [ ] Optimize hot paths if needed

---

## Implementation Notes

### Bit Layout Reference
```
Bits: 17 16 15 | 14 13 12 | 11 10 9 | 8 7 6 | 5 4 3 | 2 1 0
Face:    Z-    |    Z+    |   Y-    |  Y+   |  X-   |  X+
```

### Intrusion Level Meanings
- **0**: No intrusion (full face)
- **1**: 1/8 unit intrusion (0.125 Unity units)
- **4**: 4/8 unit intrusion (0.5 Unity units) - half block
- **7**: 7/8 unit intrusion (0.875 Unity units) - minimal remaining

### MapEditor Workflow
1. **Select paint mode** (Solid, Standable, etc.) - works as before
2. **Adjust Y+ intrusion** with mouse wheel (0-7 levels) - global setting
3. **Paint blocks** with Ctrl+Click - applies both paint mode AND current intrusion level
4. **Visual feedback** shows intrusion level in scene view

### Migration Strategy
- Existing `byte[] BlockIntrude` values (0-7) convert directly to `int[]`
- Old single-direction intrusion becomes Y+ face intrusion in new system
- Zero intrusion data (most blocks) remains zero in new format

---

## File Checklist
- [ ] `Assets/Scripts/Backend/Map/MapChunk.cs` - Data structure changes
- [ ] `Assets/Scripts/Editor/MapEditor.cs` - Mouse wheel integration  
- [ ] `Assets/Scripts/Backend/Map/MapRenderer.cs` - Intrusion mesh generation
- [ ] `Assets/Scripts/Backend/Map/MapSystem.cs` - DDA algorithm updates
- [ ] **NEW**: Unit test file for IntrusionBits utilities

## Completion Criteria
- [x] Plan created and reviewed
- [ ] Phase 1: Data structures working with bit manipulation
- [ ] Phase 2: Mouse wheel editing functional in MapEditor  
- [ ] Phase 3: Intruded blocks render correctly
- [ ] Phase 4: Raycast selection works with intrusion
- [ ] Phase 5: All tests pass, performance acceptable

---

*This plan maintains backward compatibility while adding comprehensive intrusion support with the requested X+X-Y+Y-Z+Z- bit layout and global mouse wheel editing.*