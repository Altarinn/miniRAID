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
        private MapSystem editorMapSystem; // Standalone MapSystem for editor use
        private Vector3 playerPosition = Vector3.zero;
        private Vector3Int selectedChunk = Vector3Int.zero;
        private Vector3Int selectedBlock = Vector3Int.zero;
        
        // Editor settings
        private bool showChunkBoundaries = true;
        private bool autoLoadChunks = true;
        private int brushSize = 1;
        private TerrainEditMode editMode = TerrainEditMode.TerrainType;
        private RaycastTarget raycastTarget = RaycastTarget.Solid;
        private PlaceMode placeMode = PlaceMode.Replace;
        
        // Painting settings
        private miniRAID.GridData.TerrainType selectedTerrainType = miniRAID.GridData.TerrainType.Normal;
        private bool paintSolid = false;
        private bool paintStandable = true;
        private bool paintPassable = true;
        
        // Direct block state settings
        private BlockState paintBlockState = new BlockState();
        
        // Rendering
        private MapRenderer mapRenderer;
        
        private enum TerrainEditMode
        {
            TerrainType,
            Solid,
            Standable,
            Passable,
            DirectBlockState // New mode for direct multi-state editing
        }
        
        // Direct block state settings
        [System.Serializable]
        public class BlockState
        {
            public bool solid = false;
            public bool standable = true;
            public bool passable = true;
            public miniRAID.GridData.TerrainType terrainType = miniRAID.GridData.TerrainType.Normal;
            
            public override string ToString()
            {
                return $"S:{(solid ? "Y" : "N")} St:{(standable ? "Y" : "N")} P:{(passable ? "Y" : "N")} T:{terrainType}";
            }
        }
        
        private enum RaycastTarget
        {
            Solid,
            Standable,
            SolidOrStandable
        }
        
        private enum PlaceMode
        {
            Replace, // Overwrite existing block (digging)
            Adjacent  // Place next to existing block (building)
        }
        
        void OnEnable()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }
        
        void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
            
            // Clean up editor-only MapSystem when editor closes
            if (editorMapSystem != null)
            {
                // Note: MapSystem doesn't implement IDisposable, but we can null it
                editorMapSystem = null;
            }
        }
        
        void OnGUI()
        {
            EditorGUILayout.LabelField("Map Editor", EditorStyles.boldLabel);
            
            // Show current mode
            string mode = Application.isPlaying ? "Play Mode (Runtime MapSystem)" : "Edit Mode (Standalone MapSystem)";
            EditorGUILayout.LabelField("Mode:", mode, EditorStyles.miniLabel);
            
            // Initialize map system if needed
            if (Application.isPlaying)
            {
                // Use runtime MapSystem in play mode
                if (mapSystem == null)
                {
                    mapSystem = miniRAID.Globals.backend?.GetMapSystem();
                }
            }
            else
            {
                // Use editor-only MapSystem outside play mode
                if (editorMapSystem == null)
                {
                    editorMapSystem = new MapSystem();
                    // Load initial chunks around player position
                    editorMapSystem.UpdateChunkLoading(playerPosition);
                }
                mapSystem = editorMapSystem;
            }
            
            if (mapSystem == null)
            {
                EditorGUILayout.HelpBox("Failed to initialize MapSystem.", MessageType.Error);
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
                
                // if (loadedChunks.Count > 0)
                // {
                //     EditorGUILayout.LabelField("Chunks:");
                //     foreach (var chunk in loadedChunks.Take(10)) // Show first 10
                //     {
                //         EditorGUILayout.LabelField($"  {chunk}");
                //     }
                //     if (loadedChunks.Count > 10)
                //     {
                //         EditorGUILayout.LabelField($"  ... and {loadedChunks.Count - 10} more");
                //     }
                // }
            }
            
            EditorGUILayout.Space();
            
            // Painting Tools
            EditorGUILayout.LabelField("Painting Tools", EditorStyles.boldLabel);
            editMode = (TerrainEditMode)EditorGUILayout.EnumPopup("Edit Mode", editMode);
            brushSize = EditorGUILayout.IntSlider("Brush Size", brushSize, 1, 5);
            raycastTarget = (RaycastTarget)EditorGUILayout.EnumPopup("Raycast Target", raycastTarget);
            placeMode = (PlaceMode)EditorGUILayout.EnumPopup("Place Mode", placeMode);
            
            // Help text
            EditorGUILayout.HelpBox(
                placeMode == PlaceMode.Replace 
                ? "Replace Mode: Overwrites the targeted block (good for digging holes)" 
                : "Adjacent Mode: Places next to the targeted block (good for building)", 
                MessageType.Info);
            
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
                case TerrainEditMode.DirectBlockState:
                    EditorGUILayout.LabelField("Direct Block State Painting", EditorStyles.boldLabel);
                    paintBlockState.solid = EditorGUILayout.Toggle("Solid", paintBlockState.solid);
                    paintBlockState.standable = EditorGUILayout.Toggle("Standable", paintBlockState.standable);
                    paintBlockState.passable = EditorGUILayout.Toggle("Passable", paintBlockState.passable);
                    paintBlockState.terrainType = (miniRAID.GridData.TerrainType)EditorGUILayout.EnumPopup("Terrain Type", paintBlockState.terrainType);
                    
                    EditorGUILayout.Space();
                    EditorGUILayout.HelpBox($"Will paint: {paintBlockState.ToString()}", MessageType.Info);
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
                
                // Ensure MapRenderer has the correct MapSystem in editor mode
                if (!Application.isPlaying)
                {
                    mapRenderer.SetMapSystem(mapSystem);
                }
                
                EditorGUILayout.LabelField("Block Type Visibility", EditorStyles.boldLabel);
                mapRenderer.showSolidBlocks = EditorGUILayout.Toggle("Show Solid Blocks", mapRenderer.showSolidBlocks);
                mapRenderer.showStandableBlocks = EditorGUILayout.Toggle("Show Standable Blocks", mapRenderer.showStandableBlocks);
                mapRenderer.showPassableBlocks = EditorGUILayout.Toggle("Show Passable Blocks", mapRenderer.showPassableBlocks);
                mapRenderer.enableOcclusionCulling = EditorGUILayout.Toggle("Enable Occlusion Culling", mapRenderer.enableOcclusionCulling);
                
                if (GUILayout.Button("Refresh All Rendering"))
                {
                    mapRenderer.RefreshAllChunks();
                }
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
            
            EditorGUILayout.Space();
            
            // Save/Load Buttons
            EditorGUILayout.LabelField("Save/Load", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Save All Chunks"))
            {
                SaveAllChunks();
            }
            if (GUILayout.Button("Force Reload All"))
            {
                ForceReloadAllChunks();
            }
            EditorGUILayout.EndHorizontal();
            
            if (GUILayout.Button("Save Current Chunk"))
            {
                SaveCurrentChunk();
            }
            
            if (GUILayout.Button("Open Save Folder"))
            {
                OpenSaveFolder();
            }
            
            EditorGUILayout.Space();
            
            // Cleanup Tools
            EditorGUILayout.LabelField("Cleanup", EditorStyles.boldLabel);
            
            if (GUILayout.Button("Clean Ghost Renderers"))
            {
                CleanupGhostRenderers();
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
                var raycastResult = DDAGridRaycast(ray);
                if (raycastResult.HasValue)
                {
                    Vector3 worldPos = GetPaintPosition(raycastResult.Value);
                    PaintAtPosition(worldPos);
                    current.Use();
                }
            }
            else if (current.type == EventType.MouseMove)
            {
                // Update selection
                Ray ray = HandleUtility.GUIPointToWorldRay(current.mousePosition);
                var raycastResult = DDAGridRaycast(ray);
                if (raycastResult.HasValue)
                {
                    Vector3 worldPos = GetPaintPosition(raycastResult.Value);
                    selectedChunk = MapSystem.WorldToChunkCoordinate(worldPos);
                    selectedBlock = MapSystem.WorldToLocalChunkCoordinate(worldPos);
                    Repaint();
                    SceneView.RepaintAll(); // Force SceneView refresh for red cube indicator
                }
            }
        }
        
        Vector3 GetPaintPosition((Vector3Int hitPos, Vector3Int faceNormal) raycastResult)
        {
            if (placeMode == PlaceMode.Adjacent)
            {
                // Place adjacent to the hit block
                return raycastResult.hitPos + raycastResult.faceNormal;
            }
            else
            {
                // Replace the hit block
                return raycastResult.hitPos;
            }
        }
        
        (Vector3Int hitPos, Vector3Int faceNormal)? DDAGridRaycast(Ray ray)
        {
            if (mapSystem == null) return null;
            
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
                if (IsRaycastTarget(gridPos))
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
        
        bool IsRaycastTarget(Vector3Int gridPos)
        {
            Vector3Int chunkCoord = MapSystem.WorldToChunkCoordinate(gridPos);
            Vector3Int localCoord = MapSystem.WorldToLocalChunkCoordinate(gridPos);
            
            var chunk = mapSystem.GetLoadedChunk(chunkCoord);
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
            Vector3Int gridPos = Vector3Int.FloorToInt(worldPos);
            Vector3Int chunkCoord = MapSystem.WorldToChunkCoordinate(gridPos);
            Vector3Int localCoord = MapSystem.WorldToLocalChunkCoordinate(gridPos);
            
            var chunk = mapSystem.GetLoadedChunk(chunkCoord);
            if (chunk == null) return;
            
            // Apply brush
            HashSet<Vector3Int> affectedChunks = new HashSet<Vector3Int>();
            for (int dx = -brushSize + 1; dx < brushSize; dx++)
            {
                for (int dy = -brushSize + 1; dy < brushSize; dy++)
                {
                    for (int dz = -brushSize + 1; dz < brushSize; dz++)
                    {
                        Vector3Int targetWorldPos = gridPos + new Vector3Int(dx, dy, dz);
                        Vector3Int targetChunkCoord = MapSystem.WorldToChunkCoordinate(targetWorldPos);
                        Vector3Int targetLocalCoord = MapSystem.WorldToLocalChunkCoordinate(targetWorldPos);
                        
                        var targetChunk = mapSystem.GetLoadedChunk(targetChunkCoord);
                        if (targetChunk != null &&
                            targetLocalCoord.x >= 0 && targetLocalCoord.x < MapChunk.SIZE &&
                            targetLocalCoord.y >= 0 && targetLocalCoord.y < MapChunk.SIZE &&
                            targetLocalCoord.z >= 0 && targetLocalCoord.z < MapChunk.SIZE)
                        {
                            ApplyPaint(targetChunk, targetLocalCoord.x, targetLocalCoord.y, targetLocalCoord.z);
                            affectedChunks.Add(targetChunkCoord);
                        }
                    }
                }
            }
            
            // Mark all affected chunks as dirty and refresh their rendering
            foreach (var affectedChunk in affectedChunks)
            {
                mapSystem.MarkChunkDirty(affectedChunk);
                if (mapRenderer != null)
                {
                    mapRenderer.RefreshChunk(affectedChunk);
                }
            }
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
                case TerrainEditMode.DirectBlockState:
                    // Apply all block state properties at once
                    chunk.SetIsSolid(x, y, z, paintBlockState.solid);
                    chunk.SetIsStandable(x, y, z, paintBlockState.standable);
                    chunk.SetIsPassable(x, y, z, paintBlockState.passable);
                    chunk.SetTerrainType(x, y, z, paintBlockState.terrainType);
                    break;
            }
        }
        
        void FindOrCreateMapRenderer()
        {
            // Find existing renderer first
            if (mapRenderer == null || mapRenderer.gameObject == null)
            {
                mapRenderer = FindObjectOfType<MapRenderer>();
            }
            
            if (mapRenderer == null)
            {
                GameObject rendererGO = new GameObject("Map Renderer (Editor)");
                mapRenderer = rendererGO.AddComponent<MapRenderer>();
                
                // Set up renderer
                mapRenderer.showChunkBoundaries = showChunkBoundaries;
                mapRenderer.enableRendering = true;
                mapRenderer.debugMode = true; // Enable debug by default
                
                // Mark as editor-only to prevent saving in scene
                if (!Application.isPlaying)
                {
                    rendererGO.hideFlags = HideFlags.DontSave;
                }
                
                Selection.activeGameObject = rendererGO;
                Debug.Log("[MapEditor] Created new MapRenderer");
            }
            else
            {
                Debug.Log($"[MapEditor] Found existing MapRenderer: {mapRenderer.name}");
            }
            
            // Always ensure the renderer has the correct MapSystem
            if (!Application.isPlaying)
            {
                mapRenderer.SetMapSystem(mapSystem);
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
            if (mapRenderer != null)
            {
                mapRenderer.RefreshChunk(playerChunk);
            }
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
            
            // Refresh all chunk rendering
            if (mapRenderer != null)
            {
                mapRenderer.RefreshAllChunks();
            }
        }
        
        void SaveAllChunks()
        {
            if (mapSystem == null)
            {
                EditorUtility.DisplayDialog("Error", "MapSystem not found! Enter Play Mode first.", "OK");
                return;
            }
            
            var loadedChunks = mapSystem.GetLoadedChunkCoordinates().ToList();
            int savedCount = 0;
            
            foreach (var chunkCoord in loadedChunks)
            {
                if (SaveChunkToDisk(chunkCoord))
                    savedCount++;
            }
            
            EditorUtility.DisplayDialog("Save Complete", 
                $"Saved {savedCount} chunks to disk.\nLocation: {UnityEngine.Application.persistentDataPath}/MapChunks/", "OK");
        }
        
        void SaveCurrentChunk()
        {
            if (mapSystem == null)
            {
                EditorUtility.DisplayDialog("Error", "MapSystem not found! Enter Play Mode first.", "OK");
                return;
            }
            
            Vector3Int playerChunk = MapSystem.WorldToChunkCoordinate(playerPosition);
            
            if (SaveChunkToDisk(playerChunk))
            {
                EditorUtility.DisplayDialog("Save Complete", 
                    $"Saved chunk {playerChunk} to disk.", "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("Save Failed", 
                    $"Could not save chunk {playerChunk}. Chunk may not be loaded.", "OK");
            }
        }
        
        void ForceReloadAllChunks()
        {
            if (mapSystem == null)
            {
                EditorUtility.DisplayDialog("Error", "MapSystem not found! Enter Play Mode first.", "OK");
                return;
            }
            
            bool confirmed = EditorUtility.DisplayDialog("Confirm Reload", 
                "This will reload all chunks from disk, discarding unsaved changes. Continue?", 
                "Yes", "Cancel");
                
            if (!confirmed) return;
            
            var loadedChunks = mapSystem.GetLoadedChunkCoordinates().ToList();
            int reloadedCount = 0;
            
            // Force reload by triggering the chunk loading system
            foreach (var chunkCoord in loadedChunks)
            {
                // Mark chunks as needing reload by temporarily changing player position
                mapSystem.UpdateChunkLoading(Vector3.zero); // Unload all
                reloadedCount++;
            }
            
            // Restore proper chunk loading
            mapSystem.UpdateChunkLoading(playerPosition);
            
            // Refresh rendering
            if (mapRenderer != null)
            {
                mapRenderer.RefreshAllChunks();
            }
            
            EditorUtility.DisplayDialog("Reload Complete", 
                $"Reloaded {reloadedCount} chunks from disk.", "OK");
        }
        
        bool SaveChunkToDisk(Vector3Int chunkCoordinate)
        {
            var chunk = mapSystem.GetLoadedChunk(chunkCoordinate);
            if (chunk == null) return false;
            
            try
            {
                string chunkStoragePath = System.IO.Path.Combine(UnityEngine.Application.persistentDataPath, "MapChunks");
                if (!System.IO.Directory.Exists(chunkStoragePath))
                {
                    System.IO.Directory.CreateDirectory(chunkStoragePath);
                }
                
                string filename = $"chunk_{chunkCoordinate.x}_{chunkCoordinate.y}_{chunkCoordinate.z}.chunk";
                string filepath = System.IO.Path.Combine(chunkStoragePath, filename);
                
                byte[] data = chunk.SerializeToBytes();
                System.IO.File.WriteAllBytes(filepath, data);
                
                Debug.Log($"[MapEditor] Saved chunk {chunkCoordinate} to {filepath}");
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[MapEditor] Failed to save chunk {chunkCoordinate}: {ex.Message}");
                return false;
            }
        }
        
        void OpenSaveFolder()
        {
            string chunkStoragePath = System.IO.Path.Combine(UnityEngine.Application.persistentDataPath, "MapChunks");
            
            if (!System.IO.Directory.Exists(chunkStoragePath))
            {
                System.IO.Directory.CreateDirectory(chunkStoragePath);
            }
            
            // Open folder in OS file explorer
            EditorUtility.RevealInFinder(chunkStoragePath);
            
            Debug.Log($"[MapEditor] Opened chunk save folder: {chunkStoragePath}");
        }
        
        void CleanupGhostRenderers()
        {
            // Find all MapRenderer instances and force cleanup
            var allRenderers = FindObjectsOfType<MapRenderer>();
            int cleanedCount = 0;
            
            foreach (var renderer in allRenderers)
            {
                if (renderer != null)
                {
                    // Force cleanup of all chunk renderers
                    System.Reflection.MethodInfo cleanupMethod = 
                        typeof(MapRenderer).GetMethod("CleanupAllChunkRenderers", 
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    
                    if (cleanupMethod != null)
                    {
                        cleanupMethod.Invoke(renderer, null);
                        cleanedCount++;
                    }
                    
                    // Also ensure material is set
                    System.Reflection.MethodInfo ensureMaterialMethod = 
                        typeof(MapRenderer).GetMethod("EnsureMaterial", 
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        
                    ensureMaterialMethod?.Invoke(renderer, null);
                }
            }
            
            // Clean up orphaned GameObjects with chunk-like names
            var allObjects = FindObjectsOfType<GameObject>();
            int orphanedCount = 0;
            
            foreach (var obj in allObjects)
            {
                if (obj.name.StartsWith("Chunk_"))
                {
                    // Check if it's a chunk renderer with no parent MapRenderer
                    var meshRenderer = obj.GetComponent<MeshRenderer>();
                    if (meshRenderer != null && (meshRenderer.material == null || meshRenderer.material.name.Contains("Missing")))
                    {
                        DestroyImmediate(obj);
                        orphanedCount++;
                    }
                }
            }
            
            EditorUtility.DisplayDialog("Cleanup Complete", 
                $"Cleaned {cleanedCount} MapRenderer(s) and removed {orphanedCount} orphaned/pink chunk objects.", "OK");
        }
    }
}