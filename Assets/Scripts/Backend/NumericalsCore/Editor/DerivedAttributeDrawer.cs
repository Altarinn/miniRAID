using System;
using miniRAID;
using miniRAID.Backend.Numericals;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Backend.NumericalsCore.Editor
{
    public class DerivedAttributeDrawer : OdinAttributeDrawer<DerivedAttributeAttribute>
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
            
            object parent = GetRootParent(Property);
            if (parent is UnityEngine.Object)
            {
                EditorGUILayout.LabelField((parent as UnityEngine.Object).name);
            }
            else
            {
                EditorGUILayout.LabelField(parent.ToString());
            }
            EditorGUILayout.LabelField(Property.Path);

            if (Globals.numericals.GetStat(parent, Property.Path, out object o))
            {
                EditorGUILayout.LabelField(o.ToString());
            }
            else
            {
                CallNextDrawer(null);
            }
        }
    }
}