using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Backend.Map;

namespace Backend.Map
{
    public class MapRenderer : MonoBehaviour
    {
        [Header("Rendering Settings")]
        public Material chunkMaterial;
        public bool showChunkBoundaries = true;
        public bool enableRendering = true;
        
        [Header("Terrain Colors")]
        public Color normalTerrainColor = Color.white;
        public Color mountainTerrainColor = Color.gray;
        public Color solidBlockColor = Color.red;
        public Color standableBlockColor = Color.green;
        
        private MapSystem mapSystem;
        private Dictionary<Vector3Int, ChunkRenderer> chunkRenderers = new Dictionary<Vector3Int, ChunkRenderer>();
        
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
                gameObject.transform.position = new Vector3(
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
            if (mapSystem == null)
            {
                mapSystem = Globals.backend?.GetMapSystem();
            }
            
            if (chunkMaterial == null)
            {
                // Create a default transparent material
                chunkMaterial = new Material(Shader.Find("Standard"));
                chunkMaterial.color = new Color(1, 1, 1, 0.5f);
                chunkMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                chunkMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                chunkMaterial.SetInt("_ZWrite", 0);
                chunkMaterial.DisableKeyword("_ALPHATEST_ON");
                chunkMaterial.EnableKeyword("_ALPHABLEND_ON");
                chunkMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                chunkMaterial.renderQueue = 3000;
            }
        }
        
        void Update()
        {
            if (!enableRendering || mapSystem == null) return;
            
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
            
            var renderer = new ChunkRenderer(gameObject, chunkCoord);
            chunkRenderers[chunkCoord] = renderer;
            
            GenerateChunkMesh(chunk, renderer);
            renderer.meshRenderer.material = chunkMaterial;
        }
        
        private void GenerateChunkMesh(MapChunk chunk, ChunkRenderer renderer)
        {
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();
            List<Color> colors = new List<Color>();
            
            // Generate cubes for solid blocks
            for (int x = 0; x < MapChunk.SIZE; x++)
            {
                for (int y = 0; y < MapChunk.SIZE; y++)
                {
                    for (int z = 0; z < MapChunk.SIZE; z++)
                    {
                        if (chunk.GetIsSolid(x, y, z))
                        {
                            AddCube(vertices, triangles, colors, 
                                new Vector3(x, y, z), 
                                GetTerrainColor(chunk.GetTerrainType(x, y, z)));
                        }
                        else if (chunk.GetIsStandable(x, y, z))
                        {
                            // Show standable blocks as wireframe or lighter color
                            AddCube(vertices, triangles, colors, 
                                new Vector3(x, y, z), 
                                standableBlockColor * 0.5f);
                        }
                    }
                }
            }
            
            // Add chunk boundaries if enabled
            if (showChunkBoundaries)
            {
                AddChunkBoundary(vertices, triangles, colors);
            }
            
            renderer.mesh.Clear();
            if (vertices.Count > 0)
            {
                renderer.mesh.vertices = vertices.ToArray();
                renderer.mesh.triangles = triangles.ToArray();
                renderer.mesh.colors = colors.ToArray();
                renderer.mesh.RecalculateNormals();
                renderer.mesh.RecalculateBounds();
            }
            
            renderer.meshFilter.mesh = renderer.mesh;
        }
        
        private Color GetTerrainColor(miniRAID.GridData.TerrainType terrainType)
        {
            switch (terrainType)
            {
                case miniRAID.GridData.TerrainType.Normal:
                    return normalTerrainColor;
                case miniRAID.GridData.TerrainType.Mountain:
                    return mountainTerrainColor;
                default:
                    return normalTerrainColor;
            }
        }
        
        private void AddCube(List<Vector3> vertices, List<int> triangles, List<Color> colors, Vector3 position, Color color)
        {
            int startVertex = vertices.Count;
            
            // Cube vertices (8 vertices)
            Vector3[] cubeVertices = new Vector3[]
            {
                position + new Vector3(0, 0, 0), // 0
                position + new Vector3(1, 0, 0), // 1
                position + new Vector3(1, 1, 0), // 2
                position + new Vector3(0, 1, 0), // 3
                position + new Vector3(0, 0, 1), // 4
                position + new Vector3(1, 0, 1), // 5
                position + new Vector3(1, 1, 1), // 6
                position + new Vector3(0, 1, 1)  // 7
            };
            
            vertices.AddRange(cubeVertices);
            
            // Add colors for all vertices
            for (int i = 0; i < 8; i++)
            {
                colors.Add(color);
            }
            
            // Cube triangles (12 triangles, 2 per face)
            int[] cubeTriangles = new int[]
            {
                // Front face
                0, 2, 1, 0, 3, 2,
                // Back face  
                4, 5, 6, 4, 6, 7,
                // Left face
                0, 7, 3, 0, 4, 7,
                // Right face
                1, 2, 6, 1, 6, 5,
                // Top face
                3, 7, 6, 3, 6, 2,
                // Bottom face
                0, 1, 5, 0, 5, 4
            };
            
            // Offset triangle indices by startVertex
            for (int i = 0; i < cubeTriangles.Length; i++)
            {
                triangles.Add(cubeTriangles[i] + startVertex);
            }
        }
        
        private void AddChunkBoundary(List<Vector3> vertices, List<int> triangles, List<Color> colors)
        {
            // Add wireframe boundary for chunk
            Color boundaryColor = Color.yellow;
            boundaryColor.a = 0.8f;
            
            // Add corner vertices for boundary
            Vector3[] boundaryVertices = new Vector3[]
            {
                new Vector3(0, 0, 0),
                new Vector3(MapChunk.SIZE, 0, 0),
                new Vector3(MapChunk.SIZE, MapChunk.SIZE, 0),
                new Vector3(0, MapChunk.SIZE, 0),
                new Vector3(0, 0, MapChunk.SIZE),
                new Vector3(MapChunk.SIZE, 0, MapChunk.SIZE),
                new Vector3(MapChunk.SIZE, MapChunk.SIZE, MapChunk.SIZE),
                new Vector3(0, MapChunk.SIZE, MapChunk.SIZE)
            };
            
            int startVertex = vertices.Count;
            vertices.AddRange(boundaryVertices);
            
            for (int i = 0; i < 8; i++)
            {
                colors.Add(boundaryColor);
            }
            
            // Add lines as thin triangles (wireframe effect)
            int[] boundaryLines = new int[]
            {
                // Bottom face edges
                0, 1, 1, 2, 2, 3, 3, 0,
                // Top face edges  
                4, 5, 5, 6, 6, 7, 7, 4,
                // Vertical edges
                0, 4, 1, 5, 2, 6, 3, 7
            };
            
            // Create thin triangles for wireframe effect
            for (int i = 0; i < boundaryLines.Length; i += 2)
            {
                int v1 = boundaryLines[i] + startVertex;
                int v2 = boundaryLines[i + 1] + startVertex;
                
                // Create a thin triangle (degenerate for wireframe effect)
                triangles.Add(v1);
                triangles.Add(v2);
                triangles.Add(v1);
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
    }
}