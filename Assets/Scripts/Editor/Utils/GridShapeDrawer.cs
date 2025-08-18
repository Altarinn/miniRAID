using System.Collections.Generic;
using System.Linq;
using miniRAID;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace miniRAID.Editor
{
    public class GridShapeDrawer : OdinValueDrawer<GridShape>
    {
        private const float TileSize = 20;
        
        protected override void DrawPropertyLayout(GUIContent label)
        {
            GridShape value = ValueEntry.SmartValue;

            // SirenixEditorFields.Vector3Field("Bounds Min", value.Bounds.min);
            // SirenixEditorFields.Vector3Field("Bounds Max", value.Bounds.max);
            EditorGUILayout.LabelField(
                $"m: {value.Bounds.min.x}, {value.Bounds.min.y}, {value.Bounds.min.z} | M: {value.Bounds.max.x}, {value.Bounds.max.y}, {value.Bounds.max.z}");
            
            // Y-Axis Edit Mode Selection
            EditorGUILayout.BeginHorizontal();
            // EditorGUILayout.LabelField("Y-Axis Mode", GUILayout.Width(80));
            YAxisEditMode newMode = (YAxisEditMode)EditorGUILayout.EnumPopup(value.editorMode);
            if (newMode != value.editorMode)
            {
                if (TryChangeEditMode(value, newMode, out string error))
                {
                    Property.RecordForUndo("Change Y-Axis Edit Mode");
                    value.editorMode = newMode;
                }
                else
                {
                    EditorUtility.DisplayDialog("Cannot Switch Mode", error, "OK");
                }
            }

            // Mode-specific controls
            if (value.editorMode == YAxisEditMode.PlanarWithHeight)
            {
                DrawPlanarHeightControls(value);
            }
            else
            {
                DrawLevelPerLevelControls(value);
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();

            value.canvasSize = Mathf.Clamp(SirenixEditorFields.IntField("Canvas Size", value.canvasSize), 2, 9);
            if (value.shape == null || GUILayout.Button("Reset shape"))
            {
                Property.RecordForUndo("Reset shape");
                value.shape = new HashSet<Vector3Int>();
                value.shape.Add(Vector3Int.zero);
                value.editorMinY = 0;
                value.editorMaxY = 0;
                value.editorCurrentLevel = 0;
            }
            
            EditorGUILayout.EndHorizontal();

            // Draw the grid editor
            DrawGridEditor(value);
        }

        private void DrawPlanarHeightControls(GridShape value)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Y Range", GUILayout.Width(70));
            
            int newMinY = EditorGUILayout.IntField(value.editorMinY, GUILayout.Width(50));
            EditorGUILayout.LabelField("to", GUILayout.Width(30));
            int newMaxY = EditorGUILayout.IntField(value.editorMaxY, GUILayout.Width(50));
            
            if (newMinY != value.editorMinY || newMaxY != value.editorMaxY)
            {
                if (newMinY <= newMaxY)
                {
                    Property.RecordForUndo("Change Y Range");
                    value.editorMinY = newMinY;
                    value.editorMaxY = newMaxY;
                    ApplyPlanarHeightToShape(value);
                }
            }
            
            EditorGUILayout.EndHorizontal();
        }

        private void DrawLevelPerLevelControls(GridShape value)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Y: ", GUILayout.Width(70));
            
            if (GUILayout.Button("-", GUILayout.Width(20)))
            {
                value.editorCurrentLevel--;
            }
            
            int newLevel = EditorGUILayout.IntField(value.editorCurrentLevel, GUILayout.Width(50));
            if (newLevel != value.editorCurrentLevel)
            {
                value.editorCurrentLevel = newLevel;
            }
            
            if (GUILayout.Button("+", GUILayout.Width(20)))
            {
                value.editorCurrentLevel++;
            }
            
            EditorGUILayout.EndHorizontal();
        }

        private void DrawGridEditor(GridShape value)
        {
            int size = value.canvasSize * 2 + 1;
            
            Rect rect = EditorGUILayout.GetControlRect(true, TileSize * size);
            rect = rect.AlignCenter(TileSize * size);
            
            SirenixEditorGUI.DrawSolidRect(rect, Color.gray);

            // Get current editing Y level
            int editingY = value.editorMode == YAxisEditMode.PlanarWithHeight ? 0 : value.editorCurrentLevel;

            for (int i = 0; i < size * size; i++)
            {
                int x = i % size - value.canvasSize;
                int z = (value.canvasSize - (int)(i / size));
                
                Rect tileRect = rect.SplitGrid(TileSize, TileSize, i);
                Vector3Int tileGridPos = new Vector3Int(x, editingY, z);
                
                bool shapeHasTile = GetTileStateForMode(value, x, z, editingY);

                if (x == 0 && z == 0)
                {
                    if (shapeHasTile)
                    {
                        SirenixEditorGUI.DrawSolidRect(
                            new Rect(tileRect.x + 1, tileRect.y + 1, tileRect.width - 1, tileRect.height - 1),
                            new Color(.8f, 1f, .1f, 1f));
                    }
                    else
                    {
                        SirenixEditorGUI.DrawSolidRect(
                            new Rect(tileRect.x + 1, tileRect.y + 1, tileRect.width - 1, tileRect.height - 1),
                            new Color(.6f, .6f, .6f, 1f));
                    }
                }
                else
                {
                    if (shapeHasTile)
                    {
                        SirenixEditorGUI.DrawSolidRect(
                            new Rect(tileRect.x + 1, tileRect.y + 1, tileRect.width - 1, tileRect.height - 1),
                            new Color(.2f, .4f, 1f, 0.7f));
                    }
                }

                if (tileRect.Contains(Event.current.mousePosition))
                {
                    SirenixEditorGUI.DrawSolidRect(
                        new Rect(tileRect.x + 1, tileRect.y + 1, tileRect.width - 1, tileRect.height - 1),
                        new Color(0f, 1f, 0f, 0.3f));

                    if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                    {
                        HandleTileClick(value, x, z, editingY, shapeHasTile);
                    }
                }
            }

            // Top-layer borders
            for (int i = 0; i < size * size; i++)
            {
                Rect tileRect = rect.SplitGrid(TileSize, TileSize, i);
                SirenixEditorGUI.DrawBorders(tileRect.SetWidth(tileRect.width + 1).SetHeight(tileRect.height + 1), 1);
            }
        }

        private bool GetTileStateForMode(GridShape value, int x, int z, int editingY)
        {
            if (value.editorMode == YAxisEditMode.PlanarWithHeight)
            {
                // In planar mode, check if any Y level in the range has this XZ position
                for (int y = value.editorMinY; y <= value.editorMaxY; y++)
                {
                    if (value.shape.Contains(new Vector3Int(x, y, z)))
                        return true;
                }
                return false;
            }
            else
            {
                // In level-per-level mode, check only the current editing level
                return value.shape.Contains(new Vector3Int(x, editingY, z));
            }
        }

        private void HandleTileClick(GridShape value, int x, int z, int editingY, bool currentlyHasTile)
        {
            if (value.editorMode == YAxisEditMode.PlanarWithHeight)
            {
                HandlePlanarTileClick(value, x, z, currentlyHasTile);
            }
            else
            {
                HandleLevelTileClick(value, x, z, editingY, currentlyHasTile);
            }
        }

        private void HandlePlanarTileClick(GridShape value, int x, int z, bool currentlyHasTile)
        {
            if (currentlyHasTile)
            {
                Property.RecordForUndo("Remove Planar Grid");
                // Remove from all Y levels in range
                for (int y = value.editorMinY; y <= value.editorMaxY; y++)
                {
                    value.RemoveGrid(new Vector3Int(x, y, z));
                }
            }
            else
            {
                Property.RecordForUndo("Add Planar Grid");
                // Add to all Y levels in range
                for (int y = value.editorMinY; y <= value.editorMaxY; y++)
                {
                    value.AddGrid(new Vector3Int(x, y, z));
                }
            }
        }

        private void HandleLevelTileClick(GridShape value, int x, int z, int editingY, bool currentlyHasTile)
        {
            Vector3Int tilePos = new Vector3Int(x, editingY, z);
            
            if (currentlyHasTile)
            {
                Property.RecordForUndo("Remove Grid");
                value.RemoveGrid(tilePos);
            }
            else
            {
                Property.RecordForUndo("Add Grid");
                value.AddGrid(tilePos);
            }
        }

        private void ApplyPlanarHeightToShape(GridShape value)
        {
            // Get the current XZ pattern (from Y=0 for simplicity)
            HashSet<Vector3Int> xzPattern = new HashSet<Vector3Int>();
            foreach (var pos in value.shape)
            {
                if (pos.y == 0)
                {
                    xzPattern.Add(new Vector3Int(pos.x, 0, pos.z));
                }
            }

            // Clear the shape and rebuild with the new Y range
            value.shape.Clear();
            foreach (var xzPos in xzPattern)
            {
                for (int y = value.editorMinY; y <= value.editorMaxY; y++)
                {
                    value.AddGrid(new Vector3Int(xzPos.x, y, xzPos.z));
                }
            }
        }

        private bool TryChangeEditMode(GridShape value, YAxisEditMode newMode, out string error)
        {
            error = "";
            
            if (newMode == YAxisEditMode.PlanarWithHeight)
            {
                // Check if shape can be represented as planar + height
                if (!CanRepresentAsPlanar(value, out int detectedMinY, out int detectedMaxY))
                {
                    error = "Shape is too complex for Planar + Height mode. The XZ pattern must be identical across all Y levels.";
                    return false;
                }
                
                // Auto-detect and set the Y range
                value.editorMinY = detectedMinY;
                value.editorMaxY = detectedMaxY;
            }
            
            return true;
        }

        private bool CanRepresentAsPlanar(GridShape value, out int minY, out int maxY)
        {
            minY = int.MaxValue;
            maxY = int.MinValue;
            
            if (value.shape.Count == 0)
            {
                minY = 0;
                maxY = 0;
                return true;
            }

            // Group positions by Y level
            var levelGroups = value.shape.GroupBy(pos => pos.y).ToList();
            
            // Find Y range
            foreach (var group in levelGroups)
            {
                minY = Mathf.Min(minY, group.Key);
                maxY = Mathf.Max(maxY, group.Key);
            }

            // Get the XZ pattern from the first level
            var firstLevelXZ = levelGroups.First().Select(pos => new Vector3Int(pos.x, 0, pos.z)).ToHashSet();
            
            // Check if all levels have the same XZ pattern
            foreach (var group in levelGroups)
            {
                var currentLevelXZ = group.Select(pos => new Vector3Int(pos.x, 0, pos.z)).ToHashSet();
                if (!firstLevelXZ.SetEquals(currentLevelXZ))
                {
                    return false;
                }
            }
            
            return true;
        }
    }
}