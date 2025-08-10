# Map System Migration: From PersistentDataPath to Addressables + .bytes Assets

**Date**: August 2025  
**Author**: Claude  
**Status**: Planning Phase  

## Overview

Migrate the map system from file-based storage (`Application.persistentDataPath`) to Unity Addressables with .bytes assets to support WebGL builds and improve asset management.

## Current System Analysis

### Current Architecture
- **File Location**: `Application.persistentDataPath + "/MapChunks/"`
- **File Format**: `chunk_{x}_{y}_{z}.chunk` (binary with gzip compression)
- **Loading**: `File.ReadAllBytes()` → `MapChunk.DeserializeFromBytes()`
- **Saving**: `MapChunk.SerializeToBytes()` → `File.WriteAllBytes()`
- **Integration**: 
  - `Databackend` constructor initializes `MapSystem`
  - `CombatSchedulerCoroutine.Combat()` starts without explicit map loading
  - Chunks load dynamically based on player position via `mapSystem.UpdateChunkLoading()`

### Key Methods in MapSystem.cs
- `LoadChunkFromDisk(Vector3Int chunkCoordinate)` → Returns `MapChunk` or null
- `SaveChunkToDisk(MapChunk chunk)` → Saves to file system
- `UpdateChunkLoading(Vector3 playerPosition)` → Dynamic chunk management

## New Architecture Design

### Addressable Asset Structure
```
Assets/GameContent/MapChunks/{MapName}/
├── MapChunk_0_0_0.bytes
├── MapChunk_0_0_1.bytes
├── MapChunk_1_0_0.bytes
└── ...
```

### Addressable Addresses
- **Pattern**: `"mapchunk_{mapName}_{x}_{y}_{z}"`
- **Examples**: 
  - `"mapchunk_slimeking_0_0_0"`
  - `"mapchunk_tutorial_-1_0_1"`

### Modified MapSystem Methods
```csharp
// Replace LoadChunkFromDisk
private IEnumerator LoadChunkFromAddressable(Vector3Int chunkCoordinate, string mapName)

// Replace SaveChunkToDisk  
private void SaveChunkToAddressable(MapChunk chunk, string mapName) // Editor only

// New method for map initialization
public IEnumerator InitializeMapAsync(string mapName, Vector3 playerStartPosition)
```

## Integration Points

### 1. Combat Initialization
**Location**: `CombatSchedulerCoroutine.Combat()` line 79  
**Change**: Add map loading before `Preparation()`

```csharp
public IEnumerator Combat()
{
    yield return new JumpIn(Globals.localizer.Initialization());
    
    // NEW: Initialize map system
    yield return new JumpIn(InitializeMap());
    
    yield return new JumpIn(Preparation());
    // ... rest unchanged
}

private IEnumerator InitializeMap()
{
    // Get map name from scene config or default
    SceneConfig config = FindFirstObjectByType<SceneConfig>();
    string mapName = config?.mapName ?? "default";
    Vector3 playerStartPos = config?.playerStartPosition ?? Vector3.zero;
    
    yield return new JumpIn(Globals.backend.GetMapSystem().InitializeMapAsync(mapName, playerStartPos));
}
```

### 2. Databackend Integration
**Location**: `Databackend` constructor  
**Change**: Remove immediate chunk loading, defer to combat initialization

```csharp
private Databackend()
{
    // Initialize new map system (but don't load chunks yet)
    mapSystem = new MapSystem();
    
    // REMOVED: mapSystem.UpdateChunkLoading(Vector3.zero);
    // This will now happen in CombatSchedulerCoroutine.Combat()
}
```

### 3. Map Name Configuration
**New Component**: `SceneConfig` enhancement  
```csharp
public class SceneConfig : MonoBehaviour
{
    // ... existing fields
    
    [Header("Map Configuration")]
    public string mapName = "default";
    public Vector3 playerStartPosition = Vector3.zero;
}
```

## Implementation Steps

### Phase 1: Core Infrastructure Changes

#### 1.1 Modify MapSystem.cs
```csharp
public class MapSystem
{
    private string currentMapName;
    
    // NEW: Map initialization with addressables
    public IEnumerator InitializeMapAsync(string mapName, Vector3 playerStartPosition)
    {
        currentMapName = mapName;
        yield return StartCoroutine(UpdateChunkLoadingAsync(playerStartPosition));
    }
    
    // MODIFIED: Async chunk loading
    public IEnumerator UpdateChunkLoadingAsync(Vector3 playerPosition)
    {
        // Convert existing logic to async
        // Load required chunks using LoadChunkFromAddressable
    }
    
    // REPLACE: LoadChunkFromDisk
    private IEnumerator LoadChunkFromAddressable(Vector3Int chunkCoordinate)
    {
        string address = $"mapchunk_{currentMapName}_{chunkCoordinate.x}_{chunkCoordinate.y}_{chunkCoordinate.z}";
        
        var handle = Addressables.LoadAssetAsync<TextAsset>(address);
        yield return handle;
        
        if (handle.Status == AsyncOperationStatus.Succeeded && handle.Result != null)
        {
            var textAsset = handle.Result;
            var chunk = MapChunk.DeserializeFromBytes(textAsset.bytes);
            chunk.ChunkCoordinate = chunkCoordinate;
            loadedChunks[chunkCoordinate] = chunk;
        }
        else
        {
            // Create default chunk if addressable doesn't exist
            var chunk = new MapChunk(chunkCoordinate);
            GenerateDefaultTerrain(chunk);
            loadedChunks[chunkCoordinate] = chunk;
        }
    }
    
    // REPLACE: SaveChunkToDisk (Editor only)
#if UNITY_EDITOR
    private void SaveChunkToAddressable(MapChunk chunk)
    {
        string folderPath = $"Assets/GameContent/MapChunks/{currentMapName}";
        string fileName = $"MapChunk_{chunk.ChunkCoordinate.x}_{chunk.ChunkCoordinate.y}_{chunk.ChunkCoordinate.z}.bytes";
        string assetPath = Path.Combine(folderPath, fileName);
        
        // Ensure directory exists
        Directory.CreateDirectory(folderPath);
        
        // Serialize and save
        byte[] data = chunk.SerializeToBytes();
        File.WriteAllBytes(assetPath, data);
        
        // Refresh asset database
        AssetDatabase.Refresh();
        
        // Auto-configure addressable
        var assetGUID = AssetDatabase.AssetPathToGUID(assetPath);
        var addressableSettings = AddressableAssetSettingsDefaultObject.Settings;
        var entry = addressableSettings.CreateOrMoveEntry(assetGUID, addressableSettings.DefaultGroup);
        entry.address = $"mapchunk_{currentMapName}_{chunk.ChunkCoordinate.x}_{chunk.ChunkCoordinate.y}_{chunk.ChunkCoordinate.z}";
        
        // Mark settings dirty
        EditorUtility.SetDirty(addressableSettings);
    }
#endif
}
```

#### 1.2 Modify CombatSchedulerCoroutine.cs
```csharp
public IEnumerator Combat()
{
    yield return new JumpIn(Globals.localizer.Initialization());
    
    // NEW: Initialize map system with addressables
    yield return new JumpIn(InitializeMap());
    
    yield return new JumpIn(Preparation());
    yield return new JumpIn(StartCombat());
    
    // ... rest unchanged
}

private IEnumerator InitializeMap()
{
    SceneConfig config = FindFirstObjectByType<SceneConfig>();
    string mapName = config?.mapName ?? "default";
    Vector3 playerStartPos = config?.playerStartPosition ?? Vector3.zero;
    
    yield return new JumpIn(Globals.backend.GetMapSystem().InitializeMapAsync(mapName, playerStartPos));
}
```

#### 1.3 Update SceneConfig.cs
Add map configuration fields to existing `SceneConfig` component.

### Phase 2: Editor Tools

#### 2.1 Create Chunk Migration Tool
```csharp
// Assets/Scripts/Editor/Utils/ChunkMigrationTool.cs
public class ChunkMigrationTool : EditorWindow
{
    [MenuItem("miniRAID/Map System/Migrate Chunks to Addressables")]
    public static void ShowWindow()
    {
        GetWindow<ChunkMigrationTool>("Chunk Migration");
    }
    
    void OnGUI()
    {
        // UI for:
        // 1. Select source directory (persistentDataPath/MapChunks)
        // 2. Select target map name
        // 3. Migrate button
        // 4. Progress bar
    }
    
    void MigrateChunksToAddressables(string sourceDir, string mapName)
    {
        // 1. Read all .chunk files from source
        // 2. Convert to .bytes assets
        // 3. Configure addressables
        // 4. Refresh asset database
    }
}
```

#### 2.2 Enhance MapEditor
Modify existing `MapEditor` to save chunks as .bytes assets instead of files.

### Phase 3: Asset Management

#### 3.1 Addressable Group Setup
- Create dedicated group: `"Map Chunks"`
- Configure for efficient loading (local/remote as needed)
- Set appropriate compression settings

#### 3.2 Build Integration
- Ensure chunks are included in WebGL builds
- Test loading performance
- Verify memory management

## Migration Workflow

### For Developers
1. Run `miniRAID/Map System/Migrate Chunks to Addressables` tool
2. Set map names in scene configurations
3. Test loading in Play mode
4. Verify WebGL builds

### For Existing Projects
1. **Backup**: Save existing chunk files
2. **Migrate**: Use migration tool to convert .chunk → .bytes
3. **Update Scenes**: Configure `SceneConfig` with appropriate map names
4. **Test**: Verify all functionality works
5. **Clean**: Remove old chunk files from persistentDataPath

## Testing Checklist

- [ ] Chunk loading works in editor Play mode
- [ ] Map editing still functions correctly  
- [ ] WebGL builds load chunks properly
- [ ] Memory usage is reasonable
- [ ] Loading times are acceptable
- [ ] Multiple map support works
- [ ] Editor tools function correctly
- [ ] Migration tool works for existing data

## Benefits

### Technical
- ✅ **WebGL Compatibility**: .bytes assets work in all Unity build targets
- ✅ **Version Control**: Chunks tracked in Git alongside code
- ✅ **Build Integration**: Assets automatically included in builds
- ✅ **Memory Management**: Addressables handle loading/unloading efficiently
- ✅ **Asset Organization**: Clear folder structure for multiple maps

### Development Workflow
- ✅ **Team Collaboration**: Map data shared via version control
- ✅ **Build Automation**: No manual file copying required
- ✅ **Asset Validation**: Unity validates chunk integrity
- ✅ **Performance Profiling**: Built-in addressable performance tools

## Future Considerations

### Potential Enhancements
1. **Streaming**: Load/unload chunks based on distance for memory optimization
2. **Compression**: Experiment with different compression methods for .bytes assets
3. **Caching**: Cache frequently accessed chunks in memory
4. **Remote Loading**: Support loading chunks from remote servers for updates
5. **Player Modifications**: Hybrid system for player-created content

### Performance Optimization
1. **Batch Loading**: Load multiple chunks in parallel
2. **Predictive Loading**: Load chunks before player reaches them
3. **Memory Pooling**: Reuse chunk objects to reduce GC pressure
4. **Async Operations**: Ensure UI remains responsive during loading

## Implementation Notes

### Coroutine Integration
- Map loading happens early in `Combat()` coroutine
- Uses existing `JumpIn()` pattern for consistency
- Leverages `SerialCoroutine` infrastructure

### Error Handling
- Graceful fallback to default terrain if addressable loading fails
- Clear error messages for debugging
- Recovery options for corrupted chunks

### Editor Workflow
- MapEditor continues to work normally
- Automatic .bytes asset generation
- Addressable configuration happens automatically
- Clear migration path for existing projects

This plan provides a comprehensive migration path while maintaining the existing architecture's strengths and adding WebGL compatibility.