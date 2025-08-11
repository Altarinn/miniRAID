using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using miniRAID;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
#endif

namespace Backend.Map
{
    public class MapSystem
    {
        // Chunk loading range around player (5x3x5 as requested)
        public static readonly Vector3Int LoadingRange = new Vector3Int(1, 1, 1);
        private static readonly Vector3Int LoadingOffset = new Vector3Int(0, 0, 0); // Center offset

        private Dictionary<Vector3Int, MapChunk> loadedChunks = new Dictionary<Vector3Int, MapChunk>();
        private Vector3Int currentPlayerChunk;
        private string currentMapName = "default";
        
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

        // Initialize map system with addressables (async)
        public IEnumerator InitializeMapAsync(string mapName, Vector3 playerStartPosition)
        {
            currentMapName = mapName;
            yield return UpdateChunkLoadingAsync(playerStartPosition);
        }

        public void UpdateChunkLoading(Vector3 playerPosition)
        {
            Globals.combatCoroutine.Instance.RunCoroutine(UpdateChunkLoadingAsync(playerPosition));
        }

        // Load chunks around player position (async version)
        public IEnumerator UpdateChunkLoadingAsync(Vector3 playerPosition)
        {
            Vector3Int newPlayerChunk = WorldToChunkCoordinate(playerPosition);
            
            if (newPlayerChunk == currentPlayerChunk && loadedChunks.Count > 0)
                yield break; // No change needed
            
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

            // Load new chunks (async)
            foreach (var chunkCoord in requiredChunks)
            {
                if (!loadedChunks.ContainsKey(chunkCoord))
                {
                    yield return LoadChunkAsync(chunkCoord);
                }
            }
        }

        // Load a single chunk (async)
        private IEnumerator LoadChunkAsync(Vector3Int chunkCoordinate)
        {
            yield return LoadChunkFromAddressable(chunkCoordinate);
        }

        // Unload a single chunk (save to disk if dirty)
        private void UnloadChunk(Vector3Int chunkCoordinate)
        {
            if (loadedChunks.TryGetValue(chunkCoordinate, out MapChunk chunk))
            {
                // TODO: Track dirty chunks and only save when modified
                SaveChunkToAddressable(chunk);
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
                intrusion = chunk.GetBlockIntrude(localCoord.x, localCoord.y, localCoord.z),
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

        // Public API for editor
        public void SetMapName(string mapName)
        {
            currentMapName = mapName;
        }

        public string GetMapName()
        {
            return currentMapName;
        }

        // Public method for editor to save chunks
        public bool SaveChunk(Vector3Int chunkCoordinate)
        {
            var chunk = GetLoadedChunk(chunkCoordinate);
            if (chunk == null) return false;
            
            try
            {
                SaveChunkToAddressable(chunk);
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to save chunk {chunkCoordinate}: {ex.Message}");
                return false;
            }
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

        // Save chunk to addressable (editor only)
#if UNITY_EDITOR
        private void SaveChunkToAddressable(MapChunk chunk)
        {
            try
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
                if (!string.IsNullOrEmpty(assetGUID))
                {
                    var addressableSettings = AddressableAssetSettingsDefaultObject.Settings;
                    if (addressableSettings != null)
                    {
                        var entry = addressableSettings.CreateOrMoveEntry(assetGUID, addressableSettings.DefaultGroup);
                        entry.address = $"mapchunk_{currentMapName}_{chunk.ChunkCoordinate.x}_{chunk.ChunkCoordinate.y}_{chunk.ChunkCoordinate.z}";
                        
                        // Mark settings dirty
                        EditorUtility.SetDirty(addressableSettings);
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to save chunk {chunk.ChunkCoordinate}: {ex.Message}");
            }
        }
#else
        private void SaveChunkToAddressable(MapChunk chunk)
        {
            // Runtime saving not supported - chunks are read-only in builds
            Debug.LogWarning($"Cannot save chunk {chunk.ChunkCoordinate} at runtime. Chunks are read-only in builds.");
        }
#endif

        // Load chunk from addressable
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
                
                // Release the handle after use
                Addressables.Release(handle);
            }
            else
            {
                // Create default chunk if addressable doesn't exist
                var chunk = new MapChunk(chunkCoordinate);
                GenerateDefaultTerrain(chunk);
                loadedChunks[chunkCoordinate] = chunk;
                
                if (handle.IsValid())
                {
                    Addressables.Release(handle);
                }
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
        
        public (Vector3Int hitPos, Vector3Int faceNormal, Vector3 hitPoint)? DDAGridRaycast(Ray ray, RaycastTarget raycastTarget)
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
                // Check if current grid position matches our raycast target WITH intrusion checking
                if (IsRaycastTargetWithIntrusion(gridPos, raycastTarget, ray, out Vector3 hitPoint))
                {
                    return (gridPos, faceNormal, hitPoint);
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
        
        /// <summary>
        /// Check if a ray intersects the intruded volume of a block.
        /// </summary>
        bool IsRaycastTargetWithIntrusion(Vector3Int gridPos, RaycastTarget raycastTarget, Ray ray, out Vector3 hitPoint)
        {
            hitPoint = Vector3.zero;
            
            Vector3Int chunkCoord = MapSystem.WorldToChunkCoordinate(gridPos);
            Vector3Int localCoord = MapSystem.WorldToLocalChunkCoordinate(gridPos);
            
            var chunk = GetLoadedChunk(chunkCoord);
            if (chunk == null) return false;
            
            // First check if block matches raycast target (solid/standable)
            bool isTarget = false;
            switch (raycastTarget)
            {
                case RaycastTarget.Solid:
                    isTarget = chunk.GetIsSolid(localCoord.x, localCoord.y, localCoord.z);
                    break;
                case RaycastTarget.Standable:
                    isTarget = chunk.GetIsStandable(localCoord.x, localCoord.y, localCoord.z);
                    break;
                case RaycastTarget.SolidOrStandable:
                    isTarget = chunk.GetIsSolid(localCoord.x, localCoord.y, localCoord.z) || 
                              chunk.GetIsStandable(localCoord.x, localCoord.y, localCoord.z);
                    break;
            }
            
            if (!isTarget) return false;
            
            // Get intrusion data for this block
            int intrusionData = chunk.GetBlockIntrude(localCoord.x, localCoord.y, localCoord.z);
            
            if (intrusionData == 0)
            {
                // No intrusion - full block, use simple ray-AABB intersection
                Vector3 blockMin = new Vector3(gridPos.x, gridPos.y, gridPos.z);
                Vector3 blockMax = blockMin + Vector3.one;
                return RayAABBIntersect(ray, blockMin, blockMax, out hitPoint);
            }
            else
            {
                // Has intrusion - check ray against intruded block bounds
                return RayIntersectsIntrudedBlock(ray, gridPos, intrusionData, out hitPoint);
            }
        }
        
        /// <summary>
        /// Ray-AABB intersection test for intruded blocks.
        /// </summary>
        bool RayIntersectsIntrudedBlock(Ray ray, Vector3Int blockPos, int intrusionData, out Vector3 hitPoint)
        {
            hitPoint = Vector3.zero;
            
            // Calculate intruded bounds using same logic as renderer
            Vector3 min = new Vector3(blockPos.x, blockPos.y, blockPos.z);
            Vector3 max = min + Vector3.one;
            
            // Apply intrusion for each face (intrusion reduces the block volume)
            const float intrusionUnit = 1.0f / 8.0f;
            
            // X+ face intrusion (shrinks from right side)
            int xPlusLevel = IntrusionBits.GetFaceIntrusion(intrusionData, Vector3Int.right);
            max.x -= xPlusLevel * intrusionUnit;
            
            // X- face intrusion (shrinks from left side)
            int xMinusLevel = IntrusionBits.GetFaceIntrusion(intrusionData, Vector3Int.left);
            min.x += xMinusLevel * intrusionUnit;
            
            // Y+ face intrusion (shrinks from top)
            int yPlusLevel = IntrusionBits.GetFaceIntrusion(intrusionData, Vector3Int.up);
            max.y -= yPlusLevel * intrusionUnit;
            
            // Y- face intrusion (shrinks from bottom)
            int yMinusLevel = IntrusionBits.GetFaceIntrusion(intrusionData, Vector3Int.down);
            min.y += yMinusLevel * intrusionUnit;
            
            // Z+ face intrusion (shrinks from front)
            int zPlusLevel = IntrusionBits.GetFaceIntrusion(intrusionData, Vector3Int.forward);
            max.z -= zPlusLevel * intrusionUnit;
            
            // Z- face intrusion (shrinks from back)
            int zMinusLevel = IntrusionBits.GetFaceIntrusion(intrusionData, Vector3Int.back);
            min.z += zMinusLevel * intrusionUnit;
            
            // Ensure valid bounds
            max = Vector3.Max(min, max);
            
            // Check if bounds have volume
            if (max.x <= min.x || max.y <= min.y || max.z <= min.z)
                return false;
            
            return RayAABBIntersect(ray, min, max, out hitPoint);
        }
        
        /// <summary>
        /// Standard ray-AABB intersection utility.
        /// </summary>
        bool RayAABBIntersect(Ray ray, Vector3 boxMin, Vector3 boxMax, out Vector3 hitPoint)
        {
            hitPoint = Vector3.zero;
            
            Vector3 rayOrigin = ray.origin;
            Vector3 rayDir = ray.direction;
            
            // Handle zero direction components
            if (Mathf.Abs(rayDir.x) < 1e-6f) rayDir.x = 1e-6f;
            if (Mathf.Abs(rayDir.y) < 1e-6f) rayDir.y = 1e-6f;
            if (Mathf.Abs(rayDir.z) < 1e-6f) rayDir.z = 1e-6f;
            
            Vector3 invDir = new Vector3(1f / rayDir.x, 1f / rayDir.y, 1f / rayDir.z);
            
            Vector3 t1 = new Vector3(
                (boxMin.x - rayOrigin.x) * invDir.x,
                (boxMin.y - rayOrigin.y) * invDir.y,
                (boxMin.z - rayOrigin.z) * invDir.z
            );
            
            Vector3 t2 = new Vector3(
                (boxMax.x - rayOrigin.x) * invDir.x,
                (boxMax.y - rayOrigin.y) * invDir.y,
                (boxMax.z - rayOrigin.z) * invDir.z
            );
            
            Vector3 tMin = Vector3.Min(t1, t2);
            Vector3 tMax = Vector3.Max(t1, t2);
            
            float tNear = Mathf.Max(Mathf.Max(tMin.x, tMin.y), tMin.z);
            float tFar = Mathf.Min(Mathf.Min(tMax.x, tMax.y), tMax.z);
            
            // Check if ray intersects AABB and intersection is in front of ray origin
            if (tNear <= tFar && tFar >= 0)
            {
                float t = tNear >= 0 ? tNear : tFar; // Use closest positive intersection
                hitPoint = rayOrigin + rayDir * t;
                return true;
            }
            
            return false;
        }
        
    }
}