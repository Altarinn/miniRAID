# Map System Architecture

## Overview

The Map System is a chunk-based world streaming solution designed to replace the fixed-size map array in `Databackend`. It provides efficient storage, loading, and editing of large 3D worlds using a 32x32x32 chunk system with gzip compression.

## Core Components

### 1. MapChunk (Data Layer)
**Location**: `Assets/Scripts/Backend/Map/MapChunk.cs`

The fundamental data unit representing a 32x32x32 block of terrain.

**Key Features:**
- **Terrain Properties**: Solid, Standable, Passable flags per block
- **Terrain Types**: Normal, Mountain (merged from old GridData system)  
- **Memory Layout**: Y-axis optimized for gzip compression (`y + x*SIZE + z*SIZE*SIZE`)
- **Serialization**: Custom binary format with gzip compression
- **Coordinate Conversion**: Efficient 3D ↔ 1D index conversion

**Data Structure:**
```csharp
public class MapChunk
{
    const int SIZE = 32;
    bool[] IsSolid;      // Blocks line of sight, projectiles
    bool[] IsStandable;  // Can be stood on top
    bool[] IsPassable;   // Can contain mobs  
    TerrainType[] TerrainTypes; // Visual/gameplay terrain types
    Vector3Int ChunkCoordinate;  // World chunk position
}
```

**Usage Patterns:**
- **Block Access**: Use `GetIsSolid(x,y,z)` / `SetIsSolid(x,y,z,value)` for safe bounds checking
- **Performance**: Direct array access for bulk operations  
- **Persistence**: `SerializeToBytes()` / `DeserializeFromBytes()` with gzip compression

### 2. MapSystem (Management Layer)
**Location**: `Assets/Scripts/Backend/Map/MapSystem.cs`

Central manager handling chunk loading, unloading, and world queries.

**Key Features:**
- **Dynamic Loading**: 5x3x5 chunk area around player position
- **Coordinate Conversion**: World ↔ Chunk ↔ Local coordinate systems
- **Storage Management**: Automatic save/load from persistent data path
- **GridData Compatibility**: Seamless integration with existing combat system

**Core Methods:**
```csharp
// Coordinate conversion (Vector3 floors to Vector3Int internally)
Vector3Int WorldToChunkCoordinate(Vector3 worldPos)
Vector3Int WorldToChunkCoordinate(Vector3Int gridPos)
Vector3Int WorldToLocalChunkCoordinate(Vector3 worldPos)  
Vector3Int WorldToLocalChunkCoordinate(Vector3Int gridPos)

// Chunk management  
void UpdateChunkLoading(Vector3 playerPosition)
MapChunk GetLoadedChunk(Vector3Int chunkCoordinate)

// World queries (replaces Databackend.GetMap)
GridData GetMap(Vector3 worldPos)      // Floors to Vector3Int internally
GridData GetMap(Vector3Int gridPos)    // Core implementation
bool IsMoveable(Vector3 worldPos, MovementType type, out int cost)
bool IsMoveable(Vector3Int gridPos, MovementType type, out int cost)

// Cross-chunk block queries (handles chunk boundaries)
bool IsStandable(Vector3 worldPos)     // Floors to Vector3Int internally
bool IsStandable(Vector3Int gridPos)   // Core implementation
bool IsSolid(Vector3 worldPos)
bool IsSolid(Vector3Int gridPos)
bool IsPassable(Vector3 worldPos)
bool IsPassable(Vector3Int gridPos)
```

**Loading Strategy:**
- **Range**: 5x3x5 chunks (375 total)
- **Player-Centered**: Automatically updates when player moves
- **Lazy Loading**: Chunks created on-demand if missing from disk
- **Default Terrain**: Ground plane at Y=0 for new chunks

### 3. MapRenderer (Visualization Layer)
**Location**: `Assets/Scripts/Backend/Map/MapRenderer.cs`

Real-time visualization of loaded chunks using merged transparent meshes.

**Key Features:**
- **Dynamic Rendering**: Automatically updates as chunks load/unload
- **Transparent Cubes**: Color-coded by terrain type and block properties
- **Chunk Boundaries**: Optional wireframe boundaries for editing
- **Performance**: Single mesh per chunk with vertex colors

**Visual Design:**
- **Solid Blocks**: Full opacity, terrain-type colored
- **Standable Blocks**: 50% opacity, green tint  
- **Chunk Boundaries**: Yellow wireframe (toggleable)
- **Materials**: Standard transparent shader with alpha blending

### 4. MapEditorWindow (Editor Layer)
**Location**: `Assets/Scripts/Editor/MapEditor.cs`

Unity Editor window for visual map editing with brush tools.

**Key Features:**
- **Brush Painting**: Ctrl+Click to paint with configurable brush size
- **Multi-Mode Editing**: Terrain type, solid, standable, passable properties
- **Visual Feedback**: Real-time chunk boundaries and block selection
- **Test Generation**: Built-in terrain generation for testing
- **Live Integration**: Works with MapRenderer in Play Mode

**Editor Workflow:**
1. **Setup**: Find/Create MapRenderer component
2. **Navigation**: Move player position to load chunks
3. **Selection**: Mouse over blocks to inspect properties
4. **Painting**: Ctrl+Click with brush tools to modify terrain
5. **Testing**: Generate test terrain or clear chunks

## Chunk Border Handling

### Cross-Chunk Queries

The MapSystem provides seamless access across chunk boundaries for critical gameplay mechanics:

**Standable Detection:**
```csharp
// OLD: Limited to single chunk
bool hasFloor = chunk.GetIsStandable(x, y-1, z);

// NEW: Cross-chunk detection
bool hasFloor = mapSystem.IsStandable(worldPos + Vector3.down);
```

**Movement Validation:**
```csharp
public bool IsMoveable(Vector3 worldPos, MovementType type, out int cost)
{
    Vector3Int gridPos = Vector3Int.FloorToInt(worldPos); // Floor early!
    return IsMoveable(gridPos, type, out cost);
}

public bool IsMoveable(Vector3Int gridPos, MovementType type, out int cost) 
{
    // All internal operations use Vector3Int
    bool isPassable = GetIsPassableAtGridPos(gridPos);
    bool hasFloor = GetIsStandableAtGridPos(gridPos) || 
                   GetIsStandableAtGridPos(gridPos + Vector3Int.down);
    
    return isPassable && hasFloor;
}
```

**Edge Cases Handled:**
- **Chunk Y-boundaries**: Walking on blocks spanning Y=0/Y=32 boundaries
- **Unloaded Chunks**: Safe fallback behavior when adjacent chunks not loaded
- **Coordinate Wrapping**: Proper local coordinate calculation across chunk edges

**Coordinate System Design:**
- **Vector3 API**: Public methods accept floating-point positions (mob compatibility)
- **Early Flooring**: `Vector3Int.FloorToInt()` applied immediately at entry points
- **Vector3Int Internal**: All grid operations use integer coordinates for precision  
- **Dual API**: Both `Vector3` and `Vector3Int` overloads available for performance

**Performance Considerations:**
- **Cached Chunk Lookups**: Efficient chunk coordinate calculations
- **Boundary Optimization**: Only cross-chunk queries when necessary
- **Fallback Behavior**: Graceful degradation for unloaded adjacent chunks
- **Integer Operations**: Grid math uses Vector3Int for better performance and precision

## Integration with Existing Systems

### Databackend Integration

The MapSystem replaces the old fixed-size map array while maintaining API compatibility:

```csharp
// OLD: Fixed array access
GridData[,,] map = new GridData[16,1,16];
var gridData = map[x,y,z];

// NEW: Dynamic chunk system  
private MapSystem mapSystem = new MapSystem();
var gridData = mapSystem.GetMap(worldPos);
```

**Migration Strategy:**
- **GetMap() Methods**: Updated to use MapSystem internally
- **Mob Collision**: Preserved existing collision detection workflow
- **Coordinate Systems**: Maintained existing backend ↔ render coordinate conversion
- **Backward Compatibility**: Kept mapSizeX/Y/Z properties for legacy code

### Combat System Compatibility

**Pathfinding Integration:**
```csharp
// Enhanced IsMoveable with MapSystem
public bool IsMoveable(Vector3 worldPos, MovementType type, out int cost)
{
    return mapSystem.IsMoveable(worldPos, type, out cost);
}
```

**Grid Effects**: Existing GridEffect system unchanged
**Mob Positioning**: Floating-point positions preserved
**Collision Detection**: IGridCollider system fully compatible

### Performance Characteristics

**Memory Usage:**
- **Per Chunk**: ~512KB uncompressed (32³ × 6 arrays)
- **Compressed**: ~50-200KB with gzip (depends on terrain complexity)  
- **Loading Range**: ~20-75MB total for 5x3x5 chunk area

**I/O Performance:**
- **Save/Load**: Async gzip compression/decompression
- **Chunk Streaming**: Only modified chunks written to disk
- **Storage Location**: `Application.persistentDataPath/MapChunks/`

## Usage Examples

### Basic Map Queries
```csharp
// Get terrain at world position
var mapSystem = Globals.backend.GetMapSystem();
var gridData = mapSystem.GetMap(new Vector3(100, 5, 200));

// Check if position is walkable
if (mapSystem.IsMoveable(playerPos, MobData.MovementType.Walk, out int cost))
{
    // Valid move destination
}
```

### Editor Scripting
```csharp
// Load chunks around position
mapSystem.UpdateChunkLoading(new Vector3(0, 0, 0));

// Get specific chunk for editing  
var chunk = mapSystem.GetLoadedChunk(new Vector3Int(0, 0, 0));
chunk.SetTerrainType(15, 5, 20, GridData.TerrainType.Mountain);
mapSystem.MarkChunkDirty(new Vector3Int(0, 0, 0));
```

### Custom Terrain Generation
```csharp
void GenerateHillTerrain(MapChunk chunk)
{
    for (int x = 0; x < MapChunk.SIZE; x++)
    {
        for (int z = 0; z < MapChunk.SIZE; z++)
        {
            int height = ComputeHeightAt(x, z);
            for (int y = 0; y <= height; y++)
            {
                chunk.SetIsSolid(x, y, z, true);
                chunk.SetIsStandable(x, y, z, y == height);
                chunk.SetIsPassable(x, y, z, false);
            }
        }
    }
}
```

## Architecture Benefits

### Scalability
- **Infinite Worlds**: No hardcoded size limits
- **Memory Efficient**: Only active areas loaded
- **Stream-Friendly**: Predictable loading/unloading patterns

### Performance  
- **Y-Axis Compression**: Optimized memory layout for vertical terrain layers
- **Chunk Batching**: Render entire chunks as single meshes
- **Lazy Loading**: Chunks created only when accessed

### Developer Experience
- **Visual Editor**: Real-time WYSIWYG editing in Unity
- **Debug Tools**: Chunk boundary visualization, block inspection
- **API Compatibility**: Drop-in replacement for existing map queries

### Extensibility
- **Modular Design**: Easy to add new block properties
- **Custom Generators**: Pluggable terrain generation system
- **Renderer Flexibility**: Separate rendering from data logic

## Future Extensions

### Planned Improvements
- **Dirty Tracking**: Only save modified chunks
- **Background Loading**: Async chunk streaming
- **LOD System**: Distance-based chunk detail levels
- **Networking**: Multi-player chunk synchronization

### Possible Enhancements
- **Texture Atlasing**: More efficient chunk rendering
- **Physics Integration**: Automatic collider generation
- **Biome System**: Advanced terrain type hierarchies
- **Compression Improvements**: Custom compression algorithms for terrain data

This architecture provides a solid foundation for scalable world streaming while maintaining compatibility with miniRAID's existing combat and positioning systems.