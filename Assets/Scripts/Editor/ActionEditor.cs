using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Reflection;
using miniRAID;
using miniRAID.Buff;
using NuclearBand;

namespace miniRAID.Editor
{
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
                    if (!File.Exists(path))
                    {
                        AssetDatabase.CreateAsset(so, path);
                        AssetDatabase.SaveAssets();
                    }
                }
            }
        }

        [MenuItem("Assets/Create Action", priority = 0)]
        static void CreateActionSO()
        {
            Type projectWindowUtilType = typeof(ProjectWindowUtil);
            MethodInfo getActiveFolderPath =
                projectWindowUtilType.GetMethod("GetActiveFolderPath", BindingFlags.Static | BindingFlags.NonPublic);
            object obj = getActiveFolderPath.Invoke(null, new object[0]);
            string pathToCurrentFolder = obj.ToString();
            Debug.Log(pathToCurrentFolder);

            SOWizard window =
                ScriptableObject.CreateInstance(typeof(SOWizard)) as SOWizard;
            window.Setup(typeof(ActionDataSO), (type, str) =>
            {
                Debug.Log($"The chosen type is: {type}");

                ActionDataSO so = ScriptableObject.CreateInstance(type) as ActionDataSO;
                if (so != null)
                {
                    so.name = str;

                    string path = Path.Join(pathToCurrentFolder, $"{str}.asset");
                    if (!File.Exists(path))
                    {
                        AssetDatabase.CreateAsset(so, path);
                        AssetDatabase.SaveAssets();

                        Selection.activeObject = so;
                    }
                }
            }, "ActionData");

            window.ShowModalUtility();
        }

        [MenuItem("Assets/Create Buff", priority = 0)]
        static void CreateBuffSO()
        {
            Type projectWindowUtilType = typeof(ProjectWindowUtil);
            MethodInfo getActiveFolderPath =
                projectWindowUtilType.GetMethod("GetActiveFolderPath", BindingFlags.Static | BindingFlags.NonPublic);
            object obj = getActiveFolderPath.Invoke(null, new object[0]);
            string pathToCurrentFolder = obj.ToString();
            Debug.Log(pathToCurrentFolder);

            SOWizard window =
                ScriptableObject.CreateInstance(typeof(SOWizard)) as SOWizard;
            window.Setup(typeof(BuffSO), (type, str) =>
            {
                Debug.Log($"The chosen type is: {type}");

                BuffSO so = ScriptableObject.CreateInstance(type) as BuffSO;
                if (so != null)
                {
                    so.name = str;

                    string path = Path.Join(pathToCurrentFolder, $"{str}.asset");
                    if (!File.Exists(path))
                    {
                        AssetDatabase.CreateAsset(so, path);
                        AssetDatabase.SaveAssets();

                        Selection.activeObject = so;
                    }
                }
            }, "Buff");

            window.ShowModalUtility();
        }
    }
}
