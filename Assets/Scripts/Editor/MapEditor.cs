using UnityEngine;
using UnityEditor;
using Backend.Map;
using System.Collections.Generic;
using System.Linq;

namespace Backend.Map.Editor
{
    public class MapEditorWindow : EditorWindow
    {
        [MenuItem("miniRAID/Map Editor")]
        public static void ShowWindow()
        {
            GetWindow<MapEditorWindow>("Map Editor");
        }

        private MapSystem mapSystem;
        private Vector3 playerPosition = Vector3.zero;
        private Vector3Int selectedChunk = Vector3Int.zero;
        private Vector3Int selectedBlock = Vector3Int.zero;
        
        // Editor settings
        private bool showChunkBoundaries = true;
        private bool autoLoadChunks = true;
        private int brushSize = 1;
        private TerrainEditMode editMode = TerrainEditMode.TerrainType;
        
        // Painting settings
        private miniRAID.GridData.TerrainType selectedTerrainType = miniRAID.GridData.TerrainType.Normal;
        private bool paintSolid = false;
        private bool paintStandable = true;
        private bool paintPassable = true;
        
        // Rendering
        private MapRenderer mapRenderer;
        
        private enum TerrainEditMode
        {
            TerrainType,
            Solid,
            Standable,
            Passable
        }
        
        void OnEnable()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }
        
        void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }
        
        void OnGUI()
        {
            EditorGUILayout.LabelField("Map Editor", EditorStyles.boldLabel);
            
            // Initialize map system if needed
            if (mapSystem == null && Application.isPlaying)
            {
                mapSystem = miniRAID.Globals.backend?.GetMapSystem();
            }
            
            if (mapSystem == null && !Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Map Editor requires Play Mode to access the MapSystem.", MessageType.Warning);
                return;
            }
            
            EditorGUILayout.Space();
            
            // Player Position Controls
            EditorGUILayout.LabelField("Player Position", EditorStyles.boldLabel);
            Vector3 newPlayerPos = EditorGUILayout.Vector3Field("Position", playerPosition);
            if (newPlayerPos != playerPosition)
            {
                playerPosition = newPlayerPos;
                if (autoLoadChunks && mapSystem != null)
                {
                    mapSystem.UpdateChunkLoading(playerPosition);
                }
            }
            
            autoLoadChunks = EditorGUILayout.Toggle("Auto Load Chunks", autoLoadChunks);
            
            if (GUILayout.Button("Update Chunk Loading"))
            {
                mapSystem?.UpdateChunkLoading(playerPosition);
            }
            
            EditorGUILayout.Space();
            
            // Chunk Info
            EditorGUILayout.LabelField("Chunk Information", EditorStyles.boldLabel);
            Vector3Int playerChunk = MapSystem.WorldToChunkCoordinate(playerPosition);
            EditorGUILayout.LabelField($"Player Chunk: {playerChunk}");
            
            if (mapSystem != null)
            {
                var loadedChunks = mapSystem.GetLoadedChunkCoordinates().ToList();
                EditorGUILayout.LabelField($"Loaded Chunks: {loadedChunks.Count}");
                
                if (loadedChunks.Count > 0)
                {
                    EditorGUILayout.LabelField("Chunks:");
                    foreach (var chunk in loadedChunks.Take(10)) // Show first 10
                    {
                        EditorGUILayout.LabelField($"  {chunk}");
                    }
                    if (loadedChunks.Count > 10)
                    {
                        EditorGUILayout.LabelField($"  ... and {loadedChunks.Count - 10} more");
                    }
                }
            }
            
            EditorGUILayout.Space();
            
            // Painting Tools
            EditorGUILayout.LabelField("Painting Tools", EditorStyles.boldLabel);
            editMode = (TerrainEditMode)EditorGUILayout.EnumPopup("Edit Mode", editMode);
            brushSize = EditorGUILayout.IntSlider("Brush Size", brushSize, 1, 5);
            
            switch (editMode)
            {
                case TerrainEditMode.TerrainType:
                    selectedTerrainType = (miniRAID.GridData.TerrainType)EditorGUILayout.EnumPopup("Terrain Type", selectedTerrainType);
                    break;
                case TerrainEditMode.Solid:
                    paintSolid = EditorGUILayout.Toggle("Set Solid", paintSolid);
                    break;
                case TerrainEditMode.Standable:
                    paintStandable = EditorGUILayout.Toggle("Set Standable", paintStandable);
                    break;
                case TerrainEditMode.Passable:
                    paintPassable = EditorGUILayout.Toggle("Set Passable", paintPassable);
                    break;
            }
            
            EditorGUILayout.Space();
            
            // Selected Block Info
            EditorGUILayout.LabelField("Selected Block", EditorStyles.boldLabel);
            selectedChunk = EditorGUILayout.Vector3IntField("Chunk", selectedChunk);
            selectedBlock = EditorGUILayout.Vector3IntField("Local Block", selectedBlock);
            
            if (mapSystem != null)
            {
                var chunk = mapSystem.GetLoadedChunk(selectedChunk);
                if (chunk != null)
                {
                    var localPos = selectedBlock;
                    if (localPos.x >= 0 && localPos.x < MapChunk.SIZE && 
                        localPos.y >= 0 && localPos.y < MapChunk.SIZE && 
                        localPos.z >= 0 && localPos.z < MapChunk.SIZE)
                    {
                        EditorGUILayout.LabelField($"Terrain: {chunk.GetTerrainType(localPos.x, localPos.y, localPos.z)}");
                        EditorGUILayout.LabelField($"Solid: {chunk.GetIsSolid(localPos.x, localPos.y, localPos.z)}");
                        EditorGUILayout.LabelField($"Standable: {chunk.GetIsStandable(localPos.x, localPos.y, localPos.z)}");
                        EditorGUILayout.LabelField($"Passable: {chunk.GetIsPassable(localPos.x, localPos.y, localPos.z)}");
                    }
                }
                else
                {
                    EditorGUILayout.LabelField("Chunk not loaded");
                }
            }
            
            EditorGUILayout.Space();
            
            // Rendering Controls
            EditorGUILayout.LabelField("Rendering", EditorStyles.boldLabel);
            showChunkBoundaries = EditorGUILayout.Toggle("Show Chunk Boundaries", showChunkBoundaries);
            
            if (mapRenderer == null)
            {
                mapRenderer = FindObjectOfType<MapRenderer>();
            }
            
            if (mapRenderer != null)
            {
                mapRenderer.showChunkBoundaries = showChunkBoundaries;
            }
            
            if (GUILayout.Button("Find/Create Map Renderer"))
            {
                FindOrCreateMapRenderer();
            }
            
            EditorGUILayout.Space();
            
            // Utility Buttons
            EditorGUILayout.LabelField("Utilities", EditorStyles.boldLabel);
            
            if (GUILayout.Button("Generate Test Terrain"))
            {
                GenerateTestTerrain();
            }
            
            if (GUILayout.Button("Clear All Chunks"))
            {
                ClearAllChunks();
            }
        }
        
        void OnSceneGUI(SceneView sceneView)
        {
            if (mapSystem == null) return;
            
            HandleMouseInput();
            DrawGizmos();
        }
        
        void HandleMouseInput()
        {
            Event current = Event.current;
            
            if (current.type == EventType.MouseDown && current.button == 0 && current.control)
            {
                // Ctrl+Click to paint
                Ray ray = HandleUtility.GUIPointToWorldRay(current.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    Vector3 worldPos = hit.point;
                    PaintAtPosition(worldPos);
                    current.Use();
                }
            }
            else if (current.type == EventType.MouseMove)
            {
                // Update selection
                Ray ray = HandleUtility.GUIPointToWorldRay(current.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    Vector3 worldPos = hit.point;
                    selectedChunk = MapSystem.WorldToChunkCoordinate(worldPos);
                    selectedBlock = MapSystem.WorldToLocalChunkCoordinate(worldPos);
                    Repaint();
                }
            }
        }
        
        void DrawGizmos()
        {
            // Draw loaded chunks
            if (mapSystem != null)
            {
                Handles.color = Color.yellow;
                foreach (var chunkCoord in mapSystem.GetLoadedChunkCoordinates())
                {
                    Vector3 chunkWorldPos = new Vector3(
                        chunkCoord.x * MapChunk.SIZE,
                        chunkCoord.y * MapChunk.SIZE,
                        chunkCoord.z * MapChunk.SIZE
                    );
                    
                    Vector3 chunkSize = Vector3.one * MapChunk.SIZE;
                    Handles.DrawWireCube(chunkWorldPos + chunkSize * 0.5f, chunkSize);
                }
                
                // Draw selected block
                if (selectedChunk != Vector3Int.zero || selectedBlock != Vector3Int.zero)
                {
                    Vector3 worldPos = new Vector3(
                        selectedChunk.x * MapChunk.SIZE + selectedBlock.x,
                        selectedChunk.y * MapChunk.SIZE + selectedBlock.y,
                        selectedChunk.z * MapChunk.SIZE + selectedBlock.z
                    );
                    
                    Handles.color = Color.red;
                    Handles.DrawWireCube(worldPos + Vector3.one * 0.5f, Vector3.one);
                }
            }
        }
        
        void PaintAtPosition(Vector3 worldPos)
        {
            Vector3Int chunkCoord = MapSystem.WorldToChunkCoordinate(worldPos);
            Vector3Int localCoord = MapSystem.WorldToLocalChunkCoordinate(worldPos);
            
            var chunk = mapSystem.GetLoadedChunk(chunkCoord);
            if (chunk == null) return;
            
            // Apply brush
            for (int dx = -brushSize + 1; dx < brushSize; dx++)
            {
                for (int dy = -brushSize + 1; dy < brushSize; dy++)
                {
                    for (int dz = -brushSize + 1; dz < brushSize; dz++)
                    {
                        Vector3Int targetLocal = localCoord + new Vector3Int(dx, dy, dz);
                        
                        if (targetLocal.x >= 0 && targetLocal.x < MapChunk.SIZE &&
                            targetLocal.y >= 0 && targetLocal.y < MapChunk.SIZE &&
                            targetLocal.z >= 0 && targetLocal.z < MapChunk.SIZE)
                        {
                            ApplyPaint(chunk, targetLocal.x, targetLocal.y, targetLocal.z);
                        }
                    }
                }
            }
            
            mapSystem.MarkChunkDirty(chunkCoord);
        }
        
        void ApplyPaint(MapChunk chunk, int x, int y, int z)
        {
            switch (editMode)
            {
                case TerrainEditMode.TerrainType:
                    chunk.SetTerrainType(x, y, z, selectedTerrainType);
                    break;
                case TerrainEditMode.Solid:
                    chunk.SetIsSolid(x, y, z, paintSolid);
                    break;
                case TerrainEditMode.Standable:
                    chunk.SetIsStandable(x, y, z, paintStandable);
                    break;
                case TerrainEditMode.Passable:
                    chunk.SetIsPassable(x, y, z, paintPassable);
                    break;
            }
        }
        
        void FindOrCreateMapRenderer()
        {
            mapRenderer = FindObjectOfType<MapRenderer>();
            
            if (mapRenderer == null)
            {
                GameObject rendererGO = new GameObject("Map Renderer");
                mapRenderer = rendererGO.AddComponent<MapRenderer>();
                
                // Set up renderer
                mapRenderer.showChunkBoundaries = showChunkBoundaries;
                mapRenderer.enableRendering = true;
                
                Selection.activeGameObject = rendererGO;
            }
        }
        
        void GenerateTestTerrain()
        {
            if (mapSystem == null) return;
            
            Vector3Int playerChunk = MapSystem.WorldToChunkCoordinate(playerPosition);
            
            // Generate some test terrain in the player's chunk
            var chunk = mapSystem.GetLoadedChunk(playerChunk);
            if (chunk == null) return;
            
            // Create a simple hill
            for (int x = 0; x < MapChunk.SIZE; x++)
            {
                for (int z = 0; z < MapChunk.SIZE; z++)
                {
                    int height = Mathf.FloorToInt(8 + 4 * Mathf.Sin(x * 0.2f) * Mathf.Cos(z * 0.2f));
                    height = Mathf.Clamp(height, 0, MapChunk.SIZE - 1);
                    
                    for (int y = 0; y <= height; y++)
                    {
                        chunk.SetIsSolid(x, y, z, true);
                        chunk.SetIsPassable(x, y, z, false);
                        chunk.SetIsStandable(x, y, z, y == height);
                        
                        if (y == height)
                            chunk.SetTerrainType(x, y, z, miniRAID.GridData.TerrainType.Normal);
                        else
                            chunk.SetTerrainType(x, y, z, miniRAID.GridData.TerrainType.Mountain);
                    }
                    
                    // Air above
                    for (int y = height + 1; y < MapChunk.SIZE; y++)
                    {
                        chunk.SetIsSolid(x, y, z, false);
                        chunk.SetIsPassable(x, y, z, true);
                        chunk.SetIsStandable(x, y, z, false);
                        chunk.SetTerrainType(x, y, z, miniRAID.GridData.TerrainType.Normal);
                    }
                }
            }
            
            mapSystem.MarkChunkDirty(playerChunk);
        }
        
        void ClearAllChunks()
        {
            if (mapSystem == null) return;
            
            foreach (var chunkCoord in mapSystem.GetLoadedChunkCoordinates().ToList())
            {
                var chunk = mapSystem.GetLoadedChunk(chunkCoord);
                if (chunk == null) continue;
                
                // Reset to default terrain
                for (int i = 0; i < MapChunk.SIZE * MapChunk.SIZE * MapChunk.SIZE; i++)
                {
                    var coord = MapChunk.IndexToCoordinate(i);
                    
                    // Simple ground plane at Y=0
                    if (coord.y == 0)
                    {
                        chunk.SetIsSolid(coord.x, coord.y, coord.z, true);
                        chunk.SetIsPassable(coord.x, coord.y, coord.z, false);
                        chunk.SetIsStandable(coord.x, coord.y, coord.z, true);
                    }
                    else
                    {
                        chunk.SetIsSolid(coord.x, coord.y, coord.z, false);
                        chunk.SetIsPassable(coord.x, coord.y, coord.z, true);
                        chunk.SetIsStandable(coord.x, coord.y, coord.z, false);
                    }
                    
                    chunk.SetTerrainType(coord.x, coord.y, coord.z, miniRAID.GridData.TerrainType.Normal);
                }
                
                mapSystem.MarkChunkDirty(chunkCoord);
            }
        }
    }
}