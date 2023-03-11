using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using NuclearBand;

public class ActionEditor
{
    [MenuItem("Assets/CreateSOFromScript", priority = 0)]
    static void CreateTest()
    {
        Debug.Log(Selection.assetGUIDs.Length);
        if (Selection.activeObject is UnityEditor.MonoScript)
        {
            MonoScript script = Selection.activeObject as UnityEditor.MonoScript;
            Debug.Log(Selection.activeObject.GetType());
            Debug.Log(script.GetClass());
            if (typeof(ScriptableObject).IsAssignableFrom(script.GetClass()))
            {
                Debug.Log("Is SO!");

                var path = AssetDatabase.GetAssetPath(script);
                path = Path.ChangeExtension(path, ".asset");

                var so = ScriptableObject.CreateInstance(script.GetClass());
                if(!File.Exists(path))
                {
                    AssetDatabase.CreateAsset(so, path);
                    AssetDatabase.SaveAssets();
                }
            }
        }
    }
}
