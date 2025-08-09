using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Backend.Map
{
    public class MapSystem
    {
        // Chunk loading range around player (5x3x5 as requested)
        public static readonly Vector3Int LoadingRange = new Vector3Int(1, 1, 1);
        private static readonly Vector3Int LoadingOffset = new Vector3Int(0, 0, 0); // Center offset

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
            Vector3Int gridPos = Vector3Int.FloorToInt(worldPos);
            return WorldToChunkCoordinate(gridPos);
        }

        // Convert grid position to chunk coordinate
        public static Vector3Int WorldToChunkCoordinate(Vector3Int gridPos)
        {
            return new Vector3Int(
                Mathf.FloorToInt((float)gridPos.x / MapChunk.SIZE),
                Mathf.FloorToInt((float)gridPos.y / MapChunk.SIZE),
                Mathf.FloorToInt((float)gridPos.z / MapChunk.SIZE)
            );
        }

        // Convert world position to local chunk coordinate  
        public static Vector3Int WorldToLocalChunkCoordinate(Vector3 worldPos)
        {
            Vector3Int gridPos = Vector3Int.FloorToInt(worldPos);
            return WorldToLocalChunkCoordinate(gridPos);
        }

        // Convert grid position to local chunk coordinate
        public static Vector3Int WorldToLocalChunkCoordinate(Vector3Int gridPos)
        {
            return new Vector3Int(
                ((gridPos.x % MapChunk.SIZE) + MapChunk.SIZE) % MapChunk.SIZE,
                ((gridPos.y % MapChunk.SIZE) + MapChunk.SIZE) % MapChunk.SIZE,
                ((gridPos.z % MapChunk.SIZE) + MapChunk.SIZE) % MapChunk.SIZE
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
            Vector3Int gridPos = Vector3Int.FloorToInt(worldPos);
            return GetMap(gridPos);
        }

        // Get map data at grid position (core implementation)
        public miniRAID.GridData GetMap(Vector3Int gridPos)
        {
            Vector3Int chunkCoord = WorldToChunkCoordinate(gridPos);
            Vector3Int localCoord = WorldToLocalChunkCoordinate(gridPos);

            if (!loadedChunks.TryGetValue(chunkCoord, out MapChunk chunk))
                return CreateDefaultGridData(); // Return default if chunk not loaded

            // Convert MapChunk data to GridData format
            var gridData = new miniRAID.GridData
            {
                solid = chunk.GetIsSolid(localCoord.x, localCoord.y, localCoord.z),
                standable = chunk.GetIsStandable(localCoord.x, localCoord.y, localCoord.z),
                passable = chunk.GetIsPassable(localCoord.x, localCoord.y, localCoord.z),
                type = chunk.GetTerrainType(localCoord.x, localCoord.y, localCoord.z),
                mob = null, // Will be set by collision detection in calling code
            };

            return gridData;
        }

        // Helper methods to check block properties across chunk boundaries
        private bool GetIsStandableAtGridPos(Vector3Int gridPos)
        {
            Vector3Int chunkCoord = WorldToChunkCoordinate(gridPos);
            Vector3Int localCoord = WorldToLocalChunkCoordinate(gridPos);

            if (!loadedChunks.TryGetValue(chunkCoord, out MapChunk chunk))
            {
                return false; // Assume not standable if chunk not loaded
            }

            return chunk.GetIsStandable(localCoord.x, localCoord.y, localCoord.z);
        }

        private bool GetIsSolidAtGridPos(Vector3Int gridPos)
        {
            Vector3Int chunkCoord = WorldToChunkCoordinate(gridPos);
            Vector3Int localCoord = WorldToLocalChunkCoordinate(gridPos);

            if (!loadedChunks.TryGetValue(chunkCoord, out MapChunk chunk))
            {
                return false; // Assume not solid if chunk not loaded
            }

            return chunk.GetIsSolid(localCoord.x, localCoord.y, localCoord.z);
        }

        private bool GetIsPassableAtGridPos(Vector3Int gridPos)
        {
            Vector3Int chunkCoord = WorldToChunkCoordinate(gridPos);
            Vector3Int localCoord = WorldToLocalChunkCoordinate(gridPos);

            if (!loadedChunks.TryGetValue(chunkCoord, out MapChunk chunk))
            {
                return true; // Assume passable if chunk not loaded
            }

            return chunk.GetIsPassable(localCoord.x, localCoord.y, localCoord.z);
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

        // Public methods for cross-chunk block queries
        public bool IsStandable(Vector3 worldPos)
        {
            Vector3Int gridPos = Vector3Int.FloorToInt(worldPos);
            return GetIsStandableAtGridPos(gridPos);
        }

        public bool IsSolid(Vector3 worldPos)
        {
            Vector3Int gridPos = Vector3Int.FloorToInt(worldPos);
            return GetIsSolidAtGridPos(gridPos);
        }

        public bool IsPassable(Vector3 worldPos)
        {
            Vector3Int gridPos = Vector3Int.FloorToInt(worldPos);
            return GetIsPassableAtGridPos(gridPos);
        }

        // Vector3Int versions for direct grid queries
        public bool IsStandable(Vector3Int gridPos)
        {
            return GetIsStandableAtGridPos(gridPos);
        }

        public bool IsSolid(Vector3Int gridPos)
        {
            return GetIsSolidAtGridPos(gridPos);
        }

        public bool IsPassable(Vector3Int gridPos)
        {
            return GetIsPassableAtGridPos(gridPos);
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
            };
        }
        
        // Raycast
        public enum RaycastTarget
        {
            Solid,
            Standable,
            SolidOrStandable
        }
        
        public (Vector3Int hitPos, Vector3Int faceNormal)? DDAGridRaycast(Ray ray, RaycastTarget raycastTarget)
        {
            Vector3 rayPos = ray.origin;
            Vector3 rayDir = ray.direction.normalized;
            
            // Maximum distance to check
            float maxDistance = 1000f;
            
            // Current grid position
            Vector3Int gridPos = Vector3Int.FloorToInt(rayPos);
            
            // Calculate step direction (1 or -1 for each axis)
            Vector3Int step = new Vector3Int(
                rayDir.x > 0 ? 1 : -1,
                rayDir.y > 0 ? 1 : -1,
                rayDir.z > 0 ? 1 : -1
            );
            
            // Calculate distance to next grid boundary on each axis
            Vector3 deltaDist = new Vector3(
                rayDir.x != 0 ? Mathf.Abs(1f / rayDir.x) : float.MaxValue,
                rayDir.y != 0 ? Mathf.Abs(1f / rayDir.y) : float.MaxValue,
                rayDir.z != 0 ? Mathf.Abs(1f / rayDir.z) : float.MaxValue
            );
            
            // Calculate initial distance to next grid boundary
            Vector3 sideDist;
            if (rayDir.x < 0)
                sideDist.x = (rayPos.x - gridPos.x) * deltaDist.x;
            else
                sideDist.x = (gridPos.x + 1.0f - rayPos.x) * deltaDist.x;
                
            if (rayDir.y < 0)
                sideDist.y = (rayPos.y - gridPos.y) * deltaDist.y;
            else
                sideDist.y = (gridPos.y + 1.0f - rayPos.y) * deltaDist.y;
                
            if (rayDir.z < 0)
                sideDist.z = (rayPos.z - gridPos.z) * deltaDist.z;
            else
                sideDist.z = (gridPos.z + 1.0f - rayPos.z) * deltaDist.z;
            
            // DDA stepping
            Vector3Int faceNormal = Vector3Int.zero;
            float currentDist = 0f;
            
            while (currentDist < maxDistance)
            {
                // Check if current grid position matches our raycast target
                if (IsRaycastTarget(gridPos, raycastTarget))
                {
                    return (gridPos, faceNormal);
                }
                
                // Step to next grid boundary and track which face we crossed
                if (sideDist.x < sideDist.y && sideDist.x < sideDist.z)
                {
                    sideDist.x += deltaDist.x;
                    gridPos.x += step.x;
                    faceNormal = new Vector3Int(-step.x, 0, 0); // Face normal opposite to step direction
                    currentDist = sideDist.x;
                }
                else if (sideDist.y < sideDist.z)
                {
                    sideDist.y += deltaDist.y;
                    gridPos.y += step.y;
                    faceNormal = new Vector3Int(0, -step.y, 0);
                    currentDist = sideDist.y;
                }
                else
                {
                    sideDist.z += deltaDist.z;
                    gridPos.z += step.z;
                    faceNormal = new Vector3Int(0, 0, -step.z);
                    currentDist = sideDist.z;
                }
            }
            
            return null;
        }
        
        bool IsRaycastTarget(Vector3Int gridPos, RaycastTarget raycastTarget)
        {
            Vector3Int chunkCoord = MapSystem.WorldToChunkCoordinate(gridPos);
            Vector3Int localCoord = MapSystem.WorldToLocalChunkCoordinate(gridPos);
            
            var chunk = GetLoadedChunk(chunkCoord);
            if (chunk == null) return false;
            
            switch (raycastTarget)
            {
                case RaycastTarget.Solid:
                    return chunk.GetIsSolid(localCoord.x, localCoord.y, localCoord.z);
                case RaycastTarget.Standable:
                    return chunk.GetIsStandable(localCoord.x, localCoord.y, localCoord.z);
                case RaycastTarget.SolidOrStandable:
                    return chunk.GetIsSolid(localCoord.x, localCoord.y, localCoord.z) || 
                           chunk.GetIsStandable(localCoord.x, localCoord.y, localCoord.z);
                default:
                    return false;
            }
        }
        
    }
}