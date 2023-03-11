using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;

[CustomEditor(typeof(miniRAID.CustomIconScriptableObject), editorForChildClasses: true)]
public class CustomIconScriptableObjectEditor : OdinEditor
{
    public override Texture2D RenderStaticPreview(string assetPath, UnityEngine.Object[] subAssets, int width, int height)
    {
        var o = (target as miniRAID.CustomIconScriptableObject);
        if(o != null && o.icon != null)
        {
            var preview = AssetPreview.GetAssetPreview(o.icon);
            Texture2D tex = new Texture2D(width, height);

            if(preview == null || tex == null) { return null; }

            EditorUtility.CopySerialized(preview, tex);

            return tex;
        }

        return null;
    }
}

[CustomPropertyDrawer(typeof(ScriptableObjectIdAttribute))]
public class ScriptableObjectIdDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUILayout.BeginHorizontal();

        //if (label != null)
        //{
        //    EditorGUILayout.PrefixLabel(label);
        //}

        //if (GUILayout.Button("Reset"))
        //{
        //    property.stringValue = null;
        //}

        GUI.enabled = false;
        if (string.IsNullOrEmpty(property.stringValue))
        {
            property.stringValue = Guid.NewGuid().ToString().Replace("-", string.Empty);
        }
        EditorGUI.PropertyField(position, property, label, true);
        //SirenixEditorFields.TextField(property.stringValue);
        GUI.enabled = true;

        EditorGUILayout.EndHorizontal();
    }
}

[CustomPropertyDrawer(typeof(EffectIdAttribute))]
public class EffectIdDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUILayout.BeginHorizontal();

        //if (label != null)
        //{
        //    EditorGUILayout.PrefixLabel(label);
        //}

        //if (GUILayout.Button("Reset"))
        //{
        //    property.intValue = -1;
        //}

        GUI.enabled = false;
        if (property.intValue <= 0)
        {
            StreamReader sr = new StreamReader("EffectIDRecord.txt");
            int currentId = int.Parse(sr.ReadLine());
            sr.Close();

            property.intValue = currentId + 1;

            StreamWriter sw = new StreamWriter("EffectIDRecord.txt");
            sw.WriteLine($"{currentId + 1}");
            sw.Close();
        }

        //SirenixEditorFields.IntField(property.intValue);
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;

        EditorGUILayout.EndHorizontal();
    }
}
