using System.Collections.Generic;
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

            EditorGUILayout.BeginHorizontal();

            value.canvasSize = Mathf.Clamp(SirenixEditorFields.IntField("Canvas Size", value.canvasSize), 2, 9);
            if (value.shape == null || GUILayout.Button("Reset shape"))
            {
                value.shape = new HashSet<Vector3Int>();
                value.shape.Add(Vector3Int.zero);
            }
            
            EditorGUILayout.EndHorizontal();

            int size = value.canvasSize * 2 + 1;
            
            Rect rect = EditorGUILayout.GetControlRect(true, TileSize * size);
            rect = rect.AlignCenter(TileSize * size);
            
            SirenixEditorGUI.DrawSolidRect(rect, Color.gray);

            for (int i = 0; i < size * size; i++)
            {
                int x = i % size - value.canvasSize;
                int y = (value.canvasSize - (int)(i / size));
                
                Rect tileRect = rect.SplitGrid(TileSize, TileSize, i);
                Vector3Int tileGridPos = new Vector3Int(x, 0, y);
                bool shapeHasTile = value.shape.Contains(tileGridPos);

                if (x == 0 && y == 0)
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
                        if (shapeHasTile)
                        {
                            Property.RecordForUndo("Remove Grid");
                            value.RemoveGrid(tileGridPos);
                        }
                        else
                        {
                            Property.RecordForUndo("Add Grid");
                            value.AddGrid(tileGridPos);
                        }
                    }
                }

                // GUIHelper.PushColor(Color.black);
                // GUI.Label(tileRect, $"{x}");
                // GUIHelper.PopColor();
            }

            // Top-layer
            for (int i = 0; i < size * size; i++)
            {
                Rect tileRect = rect.SplitGrid(TileSize, TileSize, i);
                SirenixEditorGUI.DrawBorders(tileRect.SetWidth(tileRect.width + 1).SetHeight(tileRect.height + 1), 1);
            }
        }
    }
}