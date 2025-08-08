using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Backend.Map
{
    public class MapSystem
    {
        // Chunk loading range around player (5x3x5 as requested)
        public static readonly Vector3Int LoadingRange = new Vector3Int(5, 3, 5);
        private static readonly Vector3Int LoadingOffset = new Vector3Int(2, 1, 2); // Center offset

        private Dictionary<Vector3Int, MapChunk> loadedChunks = new Dictionary<Vector3Int, MapChunk>();
        private Vector3Int currentPlayerChunk;
        
        // Storage paths
        private static readonly string ChunkStoragePath = Path.Combine(Application.persistentDataPath, "MapChunks");

        public MapSystem()
        {
            // Ensure storage directory exists
            if (!Directory.Exists(ChunkStoragePath))
            {
                Directory.CreateDirectory(ChunkStoragePath);
            }
        }

        // Convert world position to chunk coordinate
        public static Vector3Int WorldToChunkCoordinate(Vector3 worldPos)
        {
            return new Vector3Int(
                Mathf.FloorToInt(worldPos.x / MapChunk.SIZE),
                Mathf.FloorToInt(worldPos.y / MapChunk.SIZE), 
                Mathf.FloorToInt(worldPos.z / MapChunk.SIZE)
            );
        }

        // Convert world position to local chunk coordinate  
        public static Vector3Int WorldToLocalChunkCoordinate(Vector3 worldPos)
        {
            return new Vector3Int(
                ((int)Mathf.Floor(worldPos.x) % MapChunk.SIZE + MapChunk.SIZE) % MapChunk.SIZE,
                ((int)Mathf.Floor(worldPos.y) % MapChunk.SIZE + MapChunk.SIZE) % MapChunk.SIZE,
                ((int)Mathf.Floor(worldPos.z) % MapChunk.SIZE + MapChunk.SIZE) % MapChunk.SIZE
            );
        }

        // Load chunks around player position
        public void UpdateChunkLoading(Vector3 playerPosition)
        {
            Vector3Int newPlayerChunk = WorldToChunkCoordinate(playerPosition);
            
            if (newPlayerChunk == currentPlayerChunk && loadedChunks.Count > 0)
                return; // No change needed
            
            currentPlayerChunk = newPlayerChunk;
            
            // Calculate required chunks
            var requiredChunks = new HashSet<Vector3Int>();
            for (int x = -LoadingOffset.x; x <= LoadingOffset.x; x++)
            {
                for (int y = -LoadingOffset.y; y <= LoadingOffset.y; y++)
                {
                    for (int z = -LoadingOffset.z; z <= LoadingOffset.z; z++)
                    {
                        requiredChunks.Add(currentPlayerChunk + new Vector3Int(x, y, z));
                    }
                }
            }

            // Unload chunks that are no longer needed
            var chunksToUnload = loadedChunks.Keys.Where(chunk => !requiredChunks.Contains(chunk)).ToList();
            foreach (var chunkCoord in chunksToUnload)
            {
                UnloadChunk(chunkCoord);
            }

            // Load new chunks
            foreach (var chunkCoord in requiredChunks)
            {
                if (!loadedChunks.ContainsKey(chunkCoord))
                {
                    LoadChunk(chunkCoord);
                }
            }
        }

        // Load a single chunk
        private void LoadChunk(Vector3Int chunkCoordinate)
        {
            MapChunk chunk = LoadChunkFromDisk(chunkCoordinate);
            if (chunk == null)
            {
                // Create new empty chunk if doesn't exist
                chunk = new MapChunk(chunkCoordinate);
                GenerateDefaultTerrain(chunk); // Generate some default terrain
            }
            
            loadedChunks[chunkCoordinate] = chunk;
        }

        // Unload a single chunk (save to disk if dirty)
        private void UnloadChunk(Vector3Int chunkCoordinate)
        {
            if (loadedChunks.TryGetValue(chunkCoordinate, out MapChunk chunk))
            {
                // TODO: Track dirty chunks and only save when modified
                SaveChunkToDisk(chunk);
                loadedChunks.Remove(chunkCoordinate);
            }
        }

        // Get map data at world position (replaces Databackend.GetMap)
        public miniRAID.GridData GetMap(Vector3 worldPos)
        {
            Vector3Int chunkCoord = WorldToChunkCoordinate(worldPos);
            Vector3Int localCoord = WorldToLocalChunkCoordinate(worldPos);

            if (!loadedChunks.TryGetValue(chunkCoord, out MapChunk chunk))
                return CreateDefaultGridData(); // Return default if chunk not loaded

            // Convert MapChunk data to GridData format
            var gridData = new miniRAID.GridData
            {
                solid = chunk.GetIsSolid(localCoord.x, localCoord.y, localCoord.z),
                mob = null, // Will be set by collision detection in calling code
                effects = new HashSet<miniRAID.GridEffect>() // Effects handled separately
            };

            // Set terrain type from private field using reflection or make it public
            var terrainField = typeof(miniRAID.GridData).GetField("type", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            terrainField?.SetValue(gridData, chunk.GetTerrainType(localCoord.x, localCoord.y, localCoord.z));

            return gridData;
        }

        // Get map data at grid position
        public miniRAID.GridData GetMap(Vector3Int gridPos)
        {
            return GetMap(new Vector3(gridPos.x, gridPos.y, gridPos.z));
        }

        // Check if position is moveable (for pathfinding)
        public bool IsMoveable(Vector3 worldPos, miniRAID.MobData.MovementType movementType, out int cost)
        {
            cost = 1;
            
            Vector3Int chunkCoord = WorldToChunkCoordinate(worldPos);
            Vector3Int localCoord = WorldToLocalChunkCoordinate(worldPos);

            if (!loadedChunks.TryGetValue(chunkCoord, out MapChunk chunk))
            {
                // If chunk not loaded, assume passable for flying, not for walking
                return movementType == miniRAID.MobData.MovementType.Fly;
            }

            if (movementType == miniRAID.MobData.MovementType.Fly)
            {
                return chunk.GetIsPassable(localCoord.x, localCoord.y, localCoord.z);
            }
            
            // For walking, need passable space and standable ground
            bool isPassable = chunk.GetIsPassable(localCoord.x, localCoord.y, localCoord.z);
            bool hasFloor = chunk.GetIsStandable(localCoord.x, localCoord.y, localCoord.z) || 
                           (localCoord.y > 0 && chunk.GetIsStandable(localCoord.x, localCoord.y - 1, localCoord.z));
            
            return isPassable && hasFloor;
        }

        // Get loaded chunk for direct access (for editor)
        public MapChunk GetLoadedChunk(Vector3Int chunkCoordinate)
        {
            return loadedChunks.TryGetValue(chunkCoordinate, out MapChunk chunk) ? chunk : null;
        }

        // Get all loaded chunk coordinates
        public IEnumerable<Vector3Int> GetLoadedChunkCoordinates()
        {
            return loadedChunks.Keys;
        }

        // Mark chunk as dirty (for editor modifications)
        public void MarkChunkDirty(Vector3Int chunkCoordinate)
        {
            // TODO: Implement dirty tracking
        }

        // Save chunk to disk
        private void SaveChunkToDisk(MapChunk chunk)
        {
            try
            {
                string filename = $"chunk_{chunk.ChunkCoordinate.x}_{chunk.ChunkCoordinate.y}_{chunk.ChunkCoordinate.z}.chunk";
                string filepath = Path.Combine(ChunkStoragePath, filename);
                
                byte[] data = chunk.SerializeToBytes();
                File.WriteAllBytes(filepath, data);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to save chunk {chunk.ChunkCoordinate}: {ex.Message}");
            }
        }

        // Load chunk from disk
        private MapChunk LoadChunkFromDisk(Vector3Int chunkCoordinate)
        {
            try
            {
                string filename = $"chunk_{chunkCoordinate.x}_{chunkCoordinate.y}_{chunkCoordinate.z}.chunk";
                string filepath = Path.Combine(ChunkStoragePath, filename);
                
                if (!File.Exists(filepath))
                    return null;
                
                byte[] data = File.ReadAllBytes(filepath);
                MapChunk chunk = MapChunk.DeserializeFromBytes(data);
                chunk.ChunkCoordinate = chunkCoordinate; // Ensure coordinate is set
                
                return chunk;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to load chunk {chunkCoordinate}: {ex.Message}");
                return null;
            }
        }

        // Generate default terrain for new chunks
        private void GenerateDefaultTerrain(MapChunk chunk)
        {
            // Simple default terrain: ground plane at Y=0
            for (int x = 0; x < MapChunk.SIZE; x++)
            {
                for (int z = 0; z < MapChunk.SIZE; z++)
                {
                    // Ground level
                    chunk.SetIsStandable(x, 0, z, true);
                    chunk.SetIsPassable(x, 0, z, false); // Ground blocks are not passable
                    chunk.SetIsSolid(x, 0, z, true);
                    chunk.SetTerrainType(x, 0, z, miniRAID.GridData.TerrainType.Normal);
                    
                    // Air above ground
                    for (int y = 1; y < MapChunk.SIZE; y++)
                    {
                        chunk.SetIsPassable(x, y, z, true);
                        chunk.SetIsStandable(x, y, z, false);
                        chunk.SetIsSolid(x, y, z, false);
                        chunk.SetTerrainType(x, y, z, miniRAID.GridData.TerrainType.Normal);
                    }
                }
            }
        }

        // Create default GridData for fallback
        private miniRAID.GridData CreateDefaultGridData()
        {
            return new miniRAID.GridData
            {
                solid = false,
                mob = null,
                effects = new HashSet<miniRAID.GridEffect>()
            };
        }
    }
}