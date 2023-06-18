using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace miniRAID.Editor
{
    public class SyncExcelWindow : OdinEditorWindow
    {
        public struct ObjectFilter
        {
            
        }
        
        [MenuItem("miniRAID/Sync XLSX")]
        private static void OpenWindow()
        {
            GetWindow<SyncExcelWindow>().Show();
        }
        
        [PropertyOrder(-10)]
        public XLSXDefinitionSO xlsxDefinition;

        [ReadOnly]
        [PropertyOrder(-10)]
        [HideLabel]
        public string xlsxFilePath;

        [PropertyOrder(-10)]
        [Button(ButtonSizes.Large)]
        public void LoadXLSXDefinition()
        {
            if (xlsxDefinition == null)
            {
                return;
            }
            
            objectsInXLSX = xlsxDefinition.objects;
            xlsxFilePath = xlsxDefinition.path;
        }

        [PropertyOrder(-9)]
        [Title("Objects finder", "")]
        public List<ObjectFilter> filters = new List<ObjectFilter>();
        
        [HorizontalGroup]
        [PropertyOrder(-8)]
        [Button(ButtonSizes.Large)]
        public void ApplyFilters()
        {
            
        }
        
        [HorizontalGroup]
        [PropertyOrder(-8)]
        [Button(ButtonSizes.Large)]
        public void ClearCaptured()
        {
            capturedObjects.Clear();
        }

        [PropertyOrder(-5)]
        public List<CustomIconScriptableObject> capturedObjects = new List<CustomIconScriptableObject>();
        
        [PropertyOrder(-4)]
        [Button(ButtonSizes.Large)]
        public void AddToXLSX()
        {
            Undo.RecordObject(xlsxDefinition, "Modified xlsx definition");
            capturedObjects.ForEach(so => objectsInXLSX.Add(so));
        }

        [PropertyOrder(0)]
        [Title("Objects in XLSX", "")]
        public List<CustomIconScriptableObject> objectsInXLSX = new List<CustomIconScriptableObject>();

        [Button(ButtonSizes.Large)]
        public void ExportToXLSX()
        {
            var obj = objectsInXLSX[0];
            ListProperties("root", obj, 0);
        }

        private System.Type[] terminationTypes = new[]
        {
            typeof(String),
            typeof(Int32),
            typeof(Int64),
            typeof(float),
            typeof(Double),
            typeof(Boolean),
            typeof(Enum)
        };
        
        private System.Type[] blacklistedTypes = new[]
        {
            typeof(Delegate),
            typeof(UnityEngine.HideFlags),
        };

        private bool IsTerminatingType(System.Type type)
            => terminationTypes.Any(t => t.IsAssignableFrom(type));
        
        private bool IsEnumerableType(System.Type type)
            => typeof(IEnumerable).IsAssignableFrom(type);
        
        private bool IsBlacklistedType(System.Type type)
            => blacklistedTypes.Any(t => t.IsAssignableFrom(type));

        private int maxDepth = 5;
        
        private void ListProperties(string prefix, object obj, int depth)
        {
            if (depth > maxDepth)
            {
                return;
            }
            
            Debug.Log($"Listing properties of {obj.GetType()}");
            
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            FieldInfo[] fields = obj.GetType().GetFields(flags);
            
            foreach (FieldInfo field in fields)
            {
                if (IsBlacklistedType(field.FieldType))
                {
                    continue;
                }
                
                string fieldPath = $"{prefix}.{field.Name}";
                Debug.Log($"{fieldPath} := {field}");

                if (IsTerminatingType(field.FieldType))
                {
                    continue;
                }

                if (IsEnumerableType(field.FieldType))
                {
                    continue;
                }
                
                var child = field.GetValue(obj);
                if (child != null)
                {
                    ListProperties(fieldPath, child, depth+1);
                }
            }
        }
    }
}