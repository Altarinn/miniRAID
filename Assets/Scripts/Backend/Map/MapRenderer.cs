using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Backend.Map;
using miniRAID;

namespace Backend.Map
{
    public class MapRenderer : MonoBehaviour
    {
        [Header("Rendering Settings")]
        public Material chunkMaterial;
        public bool showChunkBoundaries = true;
        public bool enableRendering = true;
        public bool autoConnectToMapSystem = true; // Auto-connect in play mode
        
        [Header("Block State Colors")]
        public Color solidBlockColor = new Color(0.8f, 0.2f, 0.2f, 1f); // Opaque red
        public Color standableBlockColor = new Color(0.2f, 0.8f, 0.2f, 1f); // Opaque green
        public Color passableBlockColor = new Color(0.2f, 0.2f, 0.8f, 1f); // Opaque blue
        public Color solidStandableBlockColor = new Color(0.9f, 0.9f, 0.2f, 1f); // Opaque yellow
        
        [Header("Terrain Colors")]
        public Color normalTerrainColor = Color.white;
        public Color mountainTerrainColor = Color.gray;
        
        [Header("Rendering Options")]
        public bool showSolidBlocks = true;
        public bool showStandableBlocks = true;
        public bool showPassableBlocks = false; // Usually air, off by default
        public bool enableOcclusionCulling = true;
        public bool debugMode = false; // Show debug info in console
        
        private MapSystem mapSystem;
        private Dictionary<Vector3Int, ChunkRenderer> chunkRenderers = new Dictionary<Vector3Int, ChunkRenderer>();
        private bool wasInPlayMode = false; // Track play mode transitions
        
        private class ChunkRenderer
        {
            public GameObject gameObject;
            public MeshRenderer meshRenderer;
            public MeshFilter meshFilter;
            public Mesh mesh;
            
            public ChunkRenderer(GameObject parent, Vector3Int chunkCoord)
            {
                gameObject = new GameObject($"Chunk_{chunkCoord.x}_{chunkCoord.y}_{chunkCoord.z}");
                gameObject.transform.SetParent(parent.transform);
                gameObject.transform.localPosition = new Vector3(
                    chunkCoord.x * MapChunk.SIZE,
                    chunkCoord.y * MapChunk.SIZE,
                    chunkCoord.z * MapChunk.SIZE
                );
                
                meshFilter = gameObject.AddComponent<MeshFilter>();
                meshRenderer = gameObject.AddComponent<MeshRenderer>();
                mesh = new Mesh();
                mesh.name = $"ChunkMesh_{chunkCoord.x}_{chunkCoord.y}_{chunkCoord.z}";
            }
            
            public void Destroy()
            {
                if (mesh != null)
                {
                    DestroyImmediate(mesh);
                }
                if (gameObject != null)
                {
                    DestroyImmediate(gameObject);
                }
            }
        }
        
        void Start()
        {
            if (mapSystem == null && autoConnectToMapSystem)
            {
                if (Application.isPlaying)
                {
                    SetMapSystem(Globals.backend?.GetMapSystem());
                }
            }
        }
        
        // Method for editor to set MapSystem outside play mode
        public void SetMapSystem(MapSystem mapSystem)
        {
            this.mapSystem = mapSystem;
            
            if (debugMode)
            {
                Debug.Log($"[MapRenderer] SetMapSystem called in {(Application.isPlaying ? "Play" : "Edit")} mode");
            }
            
            // Ensure material is created
            EnsureMaterial();
            
            // Force chunk rendering update immediately
            if (mapSystem != null)
            {
                UpdateChunkRendering();
                
                if (debugMode)
                {
                    var loadedChunks = mapSystem.GetLoadedChunkCoordinates().ToList();
                    Debug.Log($"[MapRenderer] Found {loadedChunks.Count} loaded chunks: {string.Join(", ", loadedChunks)}");
                }
            }
        }
        
        private void EnsureMaterial()
        {
            if (chunkMaterial == null)
            {
                // Create a default lit material if none assigned
                chunkMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                if (chunkMaterial.shader == null)
                {
                    // Fallback to built-in Standard if URP not available
                    chunkMaterial = new Material(Shader.Find("Standard"));
                }
                
                chunkMaterial.color = Color.white; // Use vertex colors for coloring
                chunkMaterial.SetFloat("_Metallic", 0.2f);
                chunkMaterial.SetFloat("_Smoothness", 0.3f);
                
                if (debugMode)
                {
                    Debug.Log($"[MapRenderer] Created default material with shader: {chunkMaterial.shader.name}");
                }
            }
        }
        
        void Update()
        {
            if (!enableRendering) return;
            
            // Auto-connect to map system in play mode if not connected
            if (mapSystem == null && autoConnectToMapSystem && Application.isPlaying)
            {
                SetMapSystem(Globals.backend?.GetMapSystem());
                return;
            }
            
            if (mapSystem == null) return;
            
            UpdateChunkRendering();
        }
        
        private void UpdateChunkRendering()
        {
            var loadedChunks = mapSystem.GetLoadedChunkCoordinates().ToHashSet();
            
            // Remove renderers for unloaded chunks
            var chunksToRemove = chunkRenderers.Keys.Where(coord => !loadedChunks.Contains(coord)).ToList();
            foreach (var coord in chunksToRemove)
            {
                chunkRenderers[coord].Destroy();
                chunkRenderers.Remove(coord);
            }
            
            // Add renderers for newly loaded chunks
            foreach (var coord in loadedChunks)
            {
                if (!chunkRenderers.ContainsKey(coord))
                {
                    CreateChunkRenderer(coord);
                }
            }
        }
        
        private void CreateChunkRenderer(Vector3Int chunkCoord)
        {
            var chunk = mapSystem.GetLoadedChunk(chunkCoord);
            if (chunk == null) return;
            
            // Ensure material exists before creating renderer
            EnsureMaterial();
            
            var renderer = new ChunkRenderer(gameObject, chunkCoord);
            chunkRenderers[chunkCoord] = renderer;
            
            GenerateChunkMesh(chunk, renderer);
            renderer.meshRenderer.material = chunkMaterial;
            renderer.meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            renderer.meshRenderer.receiveShadows = true;
            
            if (debugMode)
            {
                Debug.Log($"[MapRenderer] Created chunk renderer for {chunkCoord} with material: {chunkMaterial?.name}");
            }
        }
        
        private void GenerateChunkMesh(MapChunk chunk, ChunkRenderer renderer)
        {
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();
            List<Color> colors = new List<Color>();
            List<Vector2> uvs = new List<Vector2>();
            
            // Generate cubes based on multi-state visualization
            for (int x = 0; x < MapChunk.SIZE; x++)
            {
                for (int y = 0; y < MapChunk.SIZE; y++)
                {
                    for (int z = 0; z < MapChunk.SIZE; z++)
                    {
                        // Skip if occlusion culling is enabled and block is occluded
                        if (enableOcclusionCulling && IsBlockOccluded(chunk, x, y, z))
                            continue;
                            
                        bool isSolid = chunk.GetIsSolid(x, y, z);
                        bool isStandable = chunk.GetIsStandable(x, y, z);
                        bool isPassable = chunk.GetIsPassable(x, y, z);
                        
                        Color blockColor = GetBlockStateColor(isSolid, isStandable, isPassable, chunk.GetTerrainType(x, y, z));
                        bool shouldRender = ShouldRenderBlock(isSolid, isStandable, isPassable);
                        
                        if (shouldRender)
                        {
                            // Check if block has intrusion data
                            int intrusionData = chunk.GetBlockIntrude(x, y, z);
                            if (intrusionData > 0)
                            {
                                AddIntrudedBlockMesh(vertices, triangles, colors, uvs, new Vector3(x, y, z), blockColor, chunk, x, y, z, intrusionData);
                            }
                            else
                            {
                                AddCubeWithFaces(vertices, triangles, colors, uvs, new Vector3(x, y, z), blockColor, chunk, x, y, z);
                            }
                        }
                    }
                }
            }
            
            // Add chunk boundaries if enabled
            if (showChunkBoundaries)
            {
                AddChunkBoundary(vertices, triangles, colors, uvs);
            }
            
            renderer.mesh.Clear();
            if (vertices.Count > 0)
            {
                renderer.mesh.vertices = vertices.ToArray();
                renderer.mesh.triangles = triangles.ToArray();
                renderer.mesh.colors = colors.ToArray();
                renderer.mesh.uv = uvs.ToArray();
                
                // Let Unity calculate proper normals for lighting instead of using manual ones
                renderer.mesh.RecalculateNormals();
                renderer.mesh.RecalculateBounds();
            }
            
            renderer.meshFilter.mesh = renderer.mesh;
        }
        
        private bool ShouldRenderBlock(bool isSolid, bool isStandable, bool isPassable)
        {
            if (isSolid && showSolidBlocks) return true;
            if (isStandable && !isSolid && showStandableBlocks) return true;
            if (isPassable && !isSolid && !isStandable && showPassableBlocks) return true;
            return false;
        }
        
        private Color GetBlockStateColor(bool isSolid, bool isStandable, bool isPassable, miniRAID.GridData.TerrainType terrainType)
        {
            // Priority: solid+standable > solid > standable > passable
            if (isSolid && isStandable)
            {
                return solidStandableBlockColor * GetTerrainModifier(terrainType);
            }
            else if (isSolid)
            {
                return solidBlockColor * GetTerrainModifier(terrainType);
            }
            else if (isStandable)
            {
                return standableBlockColor * GetTerrainModifier(terrainType);
            }
            else if (isPassable)
            {
                return passableBlockColor * GetTerrainModifier(terrainType);
            }
            else
            {
                // Default fallback (shouldn't happen)
                return Color.gray * GetTerrainModifier(terrainType);
            }
        }
        
        private Color GetTerrainModifier(miniRAID.GridData.TerrainType terrainType)
        {
            switch (terrainType)
            {
                case miniRAID.GridData.TerrainType.Normal:
                    return Color.white;
                case miniRAID.GridData.TerrainType.Mountain:
                    return new Color(0.7f, 0.7f, 0.7f, 1f); // Darker modifier
                default:
                    return Color.white;
            }
        }
        
        private bool IsBlockOccluded(MapChunk chunk, int x, int y, int z)
        {
            // A block is occluded if all 6 adjacent positions are solid
            Vector3Int[] neighbors = {
                new Vector3Int(x-1, y, z), new Vector3Int(x+1, y, z),
                new Vector3Int(x, y-1, z), new Vector3Int(x, y+1, z),
                new Vector3Int(x, y, z-1), new Vector3Int(x, y, z+1)
            };
            
            foreach (var neighbor in neighbors)
            {
                if (!IsSolidAt(chunk, neighbor.x, neighbor.y, neighbor.z))
                    return false; // Not occluded if any neighbor is not solid
            }
            
            return true; // All neighbors are solid, so this block is occluded
        }
        
        private bool IsSolidAt(MapChunk chunk, int x, int y, int z)
        {
            // Handle out-of-chunk boundaries
            if (x < 0 || x >= MapChunk.SIZE || y < 0 || y >= MapChunk.SIZE || z < 0 || z >= MapChunk.SIZE)
            {
                // For cross-chunk queries, we need to check neighboring chunks
                // For now, assume non-solid (could be optimized to check actual neighboring chunks)
                return false;
            }
            
            return chunk.GetIsSolid(x, y, z);
        }
        
        // Add cube with proper face culling for better performance and lighting
        private void AddCubeWithFaces(List<Vector3> vertices, List<int> triangles, List<Color> colors, 
            List<Vector2> uvs, Vector3 position, Color color, MapChunk chunk, int x, int y, int z)
        {
            // Check each face and only add if it's exposed (not adjacent to another solid block)
            Vector3Int[] faceDirections = {
                Vector3Int.up,    // Top
                Vector3Int.down,  // Bottom
                Vector3Int.right, // Right
                Vector3Int.left,  // Left
                Vector3Int.forward, // Front
                Vector3Int.back   // Back
            };
            
            Vector3[] faceNormals = {
                Vector3.up, Vector3.down, Vector3.right, Vector3.left, Vector3.forward, Vector3.back
            };
            
            for (int face = 0; face < 6; face++)
            {
                var neighborPos = new Vector3Int(x, y, z) + faceDirections[face];
                
                // Check if this face should be rendered (not occluded by adjacent block)
                if (!ShouldRenderFace(chunk, neighborPos))
                    continue;
                
                AddQuadFace(vertices, triangles, colors, uvs, position, face, color, faceNormals[face]);
            }
        }
        
        private bool ShouldRenderFace(MapChunk chunk, Vector3Int neighborPos)
        {
            // Check if neighbor position is within chunk bounds
            if (neighborPos.x >= 0 && neighborPos.x < MapChunk.SIZE &&
                neighborPos.y >= 0 && neighborPos.y < MapChunk.SIZE &&
                neighborPos.z >= 0 && neighborPos.z < MapChunk.SIZE)
            {
                // Only hide face if neighbor is solid AND has no intrusion (is a full block)
                bool neighborIsSolid = chunk.GetIsSolid(neighborPos.x, neighborPos.y, neighborPos.z);
                if (neighborIsSolid)
                {
                    int neighborIntrusionData = chunk.GetBlockIntrude(neighborPos.x, neighborPos.y, neighborPos.z);
                    
                    // Only cull face if neighbor is a FULL solid block (no intrusion)
                    // If neighbor has any intrusion, always render the face to avoid seams
                    return neighborIntrusionData > 0;
                }
                
                return true; // Neighbor not solid, render face
            }
            
            // Render faces on chunk boundaries (could be optimized to check neighboring chunks)
            return true;
        }
        
        private void AddQuadFace(List<Vector3> vertices, List<int> triangles, List<Color> colors,
           List<Vector2> uvs, Vector3 position, int faceIndex, Color color, Vector3 normal)
        {
            int startVertex = vertices.Count;
            
            // Define quad vertices for each face
            Vector3[] faceVertices = new Vector3[4];
            
            switch (faceIndex)
            {
                case 0: // Top
                    faceVertices[0] = position + new Vector3(0, 1, 0);
                    faceVertices[1] = position + new Vector3(0, 1, 1);
                    faceVertices[2] = position + new Vector3(1, 1, 1);
                    faceVertices[3] = position + new Vector3(1, 1, 0);
                    break;
                case 1: // Bottom
                    faceVertices[0] = position + new Vector3(0, 0, 1);
                    faceVertices[1] = position + new Vector3(0, 0, 0);
                    faceVertices[2] = position + new Vector3(1, 0, 0);
                    faceVertices[3] = position + new Vector3(1, 0, 1);
                    break;
                case 2: // Right
                    faceVertices[0] = position + new Vector3(1, 0, 0);
                    faceVertices[1] = position + new Vector3(1, 1, 0);
                    faceVertices[2] = position + new Vector3(1, 1, 1);
                    faceVertices[3] = position + new Vector3(1, 0, 1);
                    break;
                case 3: // Left
                    faceVertices[0] = position + new Vector3(0, 0, 1);
                    faceVertices[1] = position + new Vector3(0, 1, 1);
                    faceVertices[2] = position + new Vector3(0, 1, 0);
                    faceVertices[3] = position + new Vector3(0, 0, 0);
                    break;
                case 4: // Front
                    faceVertices[0] = position + new Vector3(0, 0, 1);
                    faceVertices[1] = position + new Vector3(1, 0, 1);
                    faceVertices[2] = position + new Vector3(1, 1, 1);
                    faceVertices[3] = position + new Vector3(0, 1, 1);
                    break;
                case 5: // Back
                    faceVertices[0] = position + new Vector3(1, 0, 0);
                    faceVertices[1] = position + new Vector3(0, 0, 0);
                    faceVertices[2] = position + new Vector3(0, 1, 0);
                    faceVertices[3] = position + new Vector3(1, 1, 0);
                    break;
            }
            
            vertices.AddRange(faceVertices);
            
            // Add colors, normals and UVs for the quad
            for (int i = 0; i < 4; i++)
            {
                colors.Add(color);
                uvs.Add(new Vector2(i % 2, i / 2)); // Simple UV mapping
            }
            
            // Add triangles for the quad (2 triangles)
            triangles.Add(startVertex + 0);
            triangles.Add(startVertex + 1);
            triangles.Add(startVertex + 2);
            
            triangles.Add(startVertex + 0);
            triangles.Add(startVertex + 2);
            triangles.Add(startVertex + 3);
        }
        
        /// <summary>
        /// Generate mesh for a block with intrusion data. Creates custom geometry based on face intrusion levels.
        /// </summary>
        private void AddIntrudedBlockMesh(List<Vector3> vertices, List<int> triangles, List<Color> colors, 
            List<Vector2> uvs, Vector3 position, Color color, MapChunk chunk, int x, int y, int z, int intrusionData)
        {
            // Calculate intruded bounds for this block
            var bounds = CalculateIntrudedBounds(intrusionData);
            
            // Check if block has any volume left after intrusion
            if (bounds.size.x <= 0 || bounds.size.y <= 0 || bounds.size.z <= 0)
                return; // No volume to render
            
            // Offset bounds by block position
            Vector3 min = position + bounds.min;
            Vector3 max = position + bounds.max;
            
            // Generate box mesh with face culling
            AddBoxMesh(vertices, triangles, colors, uvs, min, max, color, chunk, x, y, z, intrusionData);
        }
        
        /// <summary>
        /// Calculate the bounding box for a block after applying intrusion levels.
        /// Returns bounds relative to block origin (0,0,0) to (1,1,1).
        /// </summary>
        private Bounds CalculateIntrudedBounds(int intrusionData)
        {
            Vector3 min = Vector3.zero;
            Vector3 max = Vector3.one;
            
            // Apply intrusion for each face (intrusion reduces the block volume)
            // Each intrusion level represents 1/8th of a unit (0.125f)
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
            
            return new Bounds((min + max) * 0.5f, max - min);
        }
        
        /// <summary>
        /// Generate a box mesh with face culling optimization.
        /// </summary>
        private void AddBoxMesh(List<Vector3> vertices, List<int> triangles, List<Color> colors, List<Vector2> uvs,
            Vector3 minPos, Vector3 maxPos, Color color, MapChunk chunk, int x, int y, int z, int intrusionData)
        {
            // Define box faces with their normals and neighbor checking
            var faces = new[]
            {
                new { normal = Vector3Int.up, vertices = new Vector3[] { 
                    new Vector3(minPos.x, maxPos.y, minPos.z), new Vector3(minPos.x, maxPos.y, maxPos.z), 
                    new Vector3(maxPos.x, maxPos.y, maxPos.z), new Vector3(maxPos.x, maxPos.y, minPos.z) } },
                new { normal = Vector3Int.down, vertices = new Vector3[] { 
                    new Vector3(minPos.x, minPos.y, maxPos.z), new Vector3(minPos.x, minPos.y, minPos.z), 
                    new Vector3(maxPos.x, minPos.y, minPos.z), new Vector3(maxPos.x, minPos.y, maxPos.z) } },
                new { normal = Vector3Int.right, vertices = new Vector3[] { 
                    new Vector3(maxPos.x, minPos.y, minPos.z), new Vector3(maxPos.x, maxPos.y, minPos.z), 
                    new Vector3(maxPos.x, maxPos.y, maxPos.z), new Vector3(maxPos.x, minPos.y, maxPos.z) } },
                new { normal = Vector3Int.left, vertices = new Vector3[] { 
                    new Vector3(minPos.x, minPos.y, maxPos.z), new Vector3(minPos.x, maxPos.y, maxPos.z), 
                    new Vector3(minPos.x, maxPos.y, minPos.z), new Vector3(minPos.x, minPos.y, minPos.z) } },
                new { normal = Vector3Int.forward, vertices = new Vector3[] { 
                    new Vector3(minPos.x, minPos.y, maxPos.z), new Vector3(maxPos.x, minPos.y, maxPos.z), 
                    new Vector3(maxPos.x, maxPos.y, maxPos.z), new Vector3(minPos.x, maxPos.y, maxPos.z) } },
                new { normal = Vector3Int.back, vertices = new Vector3[] { 
                    new Vector3(maxPos.x, minPos.y, minPos.z), new Vector3(minPos.x, minPos.y, minPos.z), 
                    new Vector3(minPos.x, maxPos.y, minPos.z), new Vector3(maxPos.x, maxPos.y, minPos.z) } }
            };
            
            foreach (var face in faces)
            {
                var neighborPos = new Vector3Int(x, y, z) + face.normal;
                
                // Check if this face should be rendered (not occluded by adjacent block)
                if (ShouldRenderFace(chunk, neighborPos))
                {
                    AddQuadFromVertices(vertices, triangles, colors, uvs, face.vertices, color);
                }
            }
        }
        
        /// <summary>
        /// Add a quad face from pre-calculated vertices.
        /// </summary>
        private void AddQuadFromVertices(List<Vector3> vertices, List<int> triangles, List<Color> colors,
            List<Vector2> uvs, Vector3[] quadVertices, Color color)
        {
            int startVertex = vertices.Count;
            vertices.AddRange(quadVertices);
            
            // Add colors and UVs
            for (int i = 0; i < 4; i++)
            {
                colors.Add(color);
                uvs.Add(new Vector2(i % 2, i / 2)); // Simple UV mapping
            }
            
            // Add triangles for the quad (2 triangles)
            triangles.Add(startVertex + 0);
            triangles.Add(startVertex + 1);
            triangles.Add(startVertex + 2);
            
            triangles.Add(startVertex + 0);
            triangles.Add(startVertex + 2);
            triangles.Add(startVertex + 3);
        }
        
        private void AddChunkBoundary(List<Vector3> vertices, List<int> triangles, List<Color> colors, List<Vector2> uvs)
        {
            // Add wireframe boundary for chunk (simplified - just corner markers)
            Color boundaryColor = new Color(1f, 1f, 0f, 1f); // Opaque yellow
            
            // Add small cubes at chunk corners as markers
            Vector3[] cornerPositions = {
                new Vector3(0, 0, 0),
                new Vector3(MapChunk.SIZE - 1, 0, 0),
                new Vector3(0, MapChunk.SIZE - 1, 0),
                new Vector3(0, 0, MapChunk.SIZE - 1)
            };
            
            foreach (var corner in cornerPositions)
            {
                AddSmallMarkerCube(vertices, triangles, colors, uvs, corner, boundaryColor, 0.2f);
            }
        }
        
        private void AddSmallMarkerCube(List<Vector3> vertices, List<int> triangles, List<Color> colors,
            List<Vector2> uvs, Vector3 position, Color color, float size)
        {
            int startVertex = vertices.Count;
            
            // Small cube vertices
            Vector3[] cubeVertices = new Vector3[]
            {
                position + new Vector3(0, 0, 0) * size,
                position + new Vector3(1, 0, 0) * size,
                position + new Vector3(1, 1, 0) * size,
                position + new Vector3(0, 1, 0) * size,
                position + new Vector3(0, 0, 1) * size,
                position + new Vector3(1, 0, 1) * size,
                position + new Vector3(1, 1, 1) * size,
                position + new Vector3(0, 1, 1) * size
            };
            
            vertices.AddRange(cubeVertices);
            
            // Add properties for all vertices
            for (int i = 0; i < 8; i++)
            {
                colors.Add(color);
                uvs.Add(new Vector2(i % 2, (i / 2) % 2)); // Basic UV mapping
            }
            
            // Simple cube triangles
            int[] cubeTriangles = new int[]
            {
                0, 2, 1, 0, 3, 2, // Front
                4, 5, 6, 4, 6, 7, // Back
                0, 7, 3, 0, 4, 7, // Left
                1, 2, 6, 1, 6, 5, // Right
                3, 7, 6, 3, 6, 2, // Top
                0, 1, 5, 0, 5, 4  // Bottom
            };
            
            for (int i = 0; i < cubeTriangles.Length; i++)
            {
                triangles.Add(cubeTriangles[i] + startVertex);
            }
        }
        
        // Public method to refresh a specific chunk
        public void RefreshChunk(Vector3Int chunkCoordinate)
        {
            if (chunkRenderers.TryGetValue(chunkCoordinate, out ChunkRenderer renderer))
            {
                var chunk = mapSystem.GetLoadedChunk(chunkCoordinate);
                if (chunk != null)
                {
                    GenerateChunkMesh(chunk, renderer);
                }
            }
        }
        
        // Public method to refresh all chunks
        public void RefreshAllChunks()
        {
            foreach (var kvp in chunkRenderers)
            {
                var chunk = mapSystem.GetLoadedChunk(kvp.Key);
                if (chunk != null)
                {
                    GenerateChunkMesh(chunk, kvp.Value);
                }
            }
        }
        
        void OnDestroy()
        {
            // Clean up all chunk renderers
            foreach (var renderer in chunkRenderers.Values)
            {
                renderer.Destroy();
            }
            chunkRenderers.Clear();
        }
        
        void OnDisable()
        {
            // Cleanup chunk renderers when disabled or mode changes
            CleanupAllChunkRenderers();
        }
        
        void OnApplicationPause(bool pauseStatus)
        {
            // Handle play mode transitions
            if (!Application.isPlaying)
            {
                CleanupAllChunkRenderers();
            }
        }
        
        private void CleanupAllChunkRenderers()
        {
            if (debugMode)
            {
                Debug.Log($"[MapRenderer] Cleaning up {chunkRenderers.Count} chunk renderers");
            }
            
            foreach (var renderer in chunkRenderers.Values.ToList())
            {
                if (renderer?.gameObject != null)
                {
                    if (Application.isPlaying)
                    {
                        Destroy(renderer.gameObject);
                    }
                    else
                    {
                        DestroyImmediate(renderer.gameObject);
                    }
                }
            }
            chunkRenderers.Clear();
        }
    }
}