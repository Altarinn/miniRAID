using System;
using miniRAID;
using miniRAID.Backend.Numericals;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Backend.NumericalsCore.Editor
{
    public class DerivedAttributeDrawer : OdinAttributeDrawer<PathBellAttribute>
    {
        protected object GetRootParent(InspectorProperty p)
        {
            while (p.Parent != null)
            {
                p = p.Parent;
            }

            return p.ValueEntry.WeakValues[0];
        }
        
        protected override void DrawPropertyLayout(GUIContent label)
        {
            // TODO: Mark the label of a derived attribute with a bold-italic style
            // var tmp = EditorStyles.label.fontStyle;
            // EditorStyles.label.fontStyle = FontStyle.BoldAndItalic;
            //
            // GUIContent wrappedLabel = new GUIContent($"[D] {label.text}");
            // EditorGUILayout.PrefixLabel(wrappedLabel);
            //
            // EditorStyles.label.fontStyle = tmp;
            
            EditorGUILayout.BeginHorizontal();
            
            // object parent = GetRootParent(Property);
            // if (parent is UnityEngine.Object)
            // {
            //     tooltip += $"{(parent as UnityEngine.Object).name}: ";
            // }
            // else
            // {
            //     tooltip += $"{(parent.ToString())}: ";
            // }
            
            // if (Globals.numericals.GetStat(parent, Property.Path, out object o))
            // {
            //     tooltip += $"{o.ToString()}";
            // }
            // else
            // {
            //     tooltip += "NOT FOUND";
            // }
            
            // GUILayout.Label(new GUIContent("#", tooltip));
            if (Event.current.shift && Event.current.control)
            {
                if (SirenixEditorGUI.IconButton(EditorIcons.Bell))
                {
                    string tooltip = $"{(Property.UnityPropertyPath)}";
                    GUIUtility.systemCopyBuffer = Property.UnityPropertyPath;
                    Debug.Log(tooltip);
                }
            }

            CallNextDrawer(label);
            
            // https://discussions.unity.com/t/remove-space-between-fields-in-beginhorizontal/217300
            GUILayout.FlexibleSpace();
            
            EditorGUILayout.EndHorizontal();
        }
    }
}