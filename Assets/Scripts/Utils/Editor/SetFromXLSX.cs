using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using System.Linq;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;

using ClosedXML;
using ClosedXML.Excel;

namespace miniRAID.Editor
{
    public class SetFromXLSX : OdinEditorWindow
    {
        [MenuItem("miniRAID/Set from XLSX")]
        private static void OpenWindow()
        {
            GetWindow<SetFromXLSX>().Show();
        }

        public async void UpdateSheet()
        {
            // TODO
        }

        [Button(ButtonSizes.Large)]
        public void ReadFromSheet()
        {
            Numericals.DataSheet sheet = new Numericals.DataSheet("data.xlsx");
            try
            {
                foreach (var row in sheet)
                {
                    var obj = Addressables.LoadAssetAsync<UnityEngine.Object>(row.address).WaitForCompletion();
                    if (obj == null)
                    {
                        continue;
                    }

                    var pt = new PropertyTree<UnityEngine.Object>(new[] { obj });
                    Undo.RecordObject(obj, $"Data copied from datasheet: {obj.name}");

                    foreach (var kv in row.parameters)
                    {
                        var ip = pt.GetPropertyAtUnityPath(kv.Key);
                        if (ip == null)
                        {
                            continue;
                        }

                        if (kv.Value.IsBlank)
                        {
                            continue;
                        }

                        if (ip.ValueEntry.TypeOfValue == typeof(int))
                        {
                            ip.ValueEntry.WeakValues[0] = (int)Math.Round(kv.Value.GetNumber());
                        }
                        else if (ip.ValueEntry.TypeOfValue == typeof(float))
                        {
                            ip.ValueEntry.WeakValues[0] = (float)kv.Value.GetNumber();
                        }
                        else if (ip.ValueEntry.TypeOfValue == typeof(double))
                        {
                            ip.ValueEntry.WeakValues[0] = kv.Value.GetNumber();
                        }
                        else if (ip.ValueEntry.TypeOfValue == typeof(string))
                        {
                            ip.ValueEntry.WeakValues[0] = kv.Value.GetText();
                        }
                        else
                        {
                            Debug.LogError($"Format not supported at {row.address} - {kv.Key} !!");
                        }

                        ip.MarkSerializationRootDirty();
                    }

                    pt.UpdateTree();
                }
            }
            finally
            {
                sheet.Close();
            }
        }

        // [Button(ButtonSizes.Large)]
        public async void TestAddressables()
        {
            var allLocations = Addressables.LoadResourceLocationsAsync("AllyNumericals", typeof(ActionDataSO)).WaitForCompletion();

            var columnDict = new Dictionary<string, string>
            {
                {"Power Lv1", "power.powerFactor.values.Array.data[0]"},
                {"Power Lv2", "power.powerFactor.values.Array.data[1]"},
                {"Power Lv3", "power.powerFactor.values.Array.data[2]"},
                {"Power Lv4", "power.powerFactor.values.Array.data[3]"},
                {"Power Lv5", "power.powerFactor.values.Array.data[4]"},
                {"Effective range", "Requester.range"},
                {"AP cost", "costs.{0es}.staticOut"},
                {"Mana cost", "costs.{1es}.staticOut"},
                {"Cooldown", "costs.{2es}.staticOut"},
                {"Direct %", "damageOrHeal.power.value"},
                {"Buff %", "buff.power.value"},
            }.ToList();

            var wb = new XLWorkbook();
            IXLWorksheet worksheet;
            if (!wb.Worksheets.TryGetWorksheet("test", out worksheet))
            {
                worksheet = wb.Worksheets.Add("test");
            }
            
            // Write directly
            // Column A, B = addressable path, name : string, string
            worksheet.Cell(1, 1).Value = "Addressable Path";
            worksheet.Cell(1, 2).Value = "Object Name";
            var columnEnumerated = columnDict.Select((v, i) => new { v, i });
            foreach (var it in columnEnumerated)
            {
                worksheet.Cell(1, it.i + 3).Value = it.v.Key;
                worksheet.Cell(2, it.i + 3).Value = it.v.Value;
            }

            var loader = Addressables.LoadAssetsAsync<ActionDataSO>(allLocations, null).Task;
            await loader;

            int objId = 0;
            foreach (var x in loader.Result)
            {
                worksheet.Cell(objId + 3, 1).Value = allLocations[objId].PrimaryKey;
                worksheet.Cell(objId + 3, 2).Value = x.name;

                var pt = new PropertyTree<UnityEngine.Object>(new [] {loader.Result[objId] });
                foreach (var it in columnEnumerated)
                {
                    var ip = pt.GetPropertyAtUnityPath(it.v.Value);
                    if(ip == null){ continue; }

                    if (ip.ValueEntry.TypeOfValue == typeof(float))
                    {
                        worksheet.Cell(objId + 3, it.i + 3).Value = (float)(ip.ValueEntry.WeakValues[0]);
                    }
                    else if(ip.ValueEntry.TypeOfValue == typeof(double))
                    {
                        worksheet.Cell(objId + 3, it.i + 3).Value = (double)(ip.ValueEntry.WeakValues[0]);
                    }
                    else if(ip.ValueEntry.TypeOfValue == typeof(int))
                    {
                        worksheet.Cell(objId + 3, it.i + 3).Value = (int)(ip.ValueEntry.WeakValues[0]);
                    }
                    else
                    {
                        worksheet.Cell(objId + 3, it.i + 3).Value = ip.ValueEntry.WeakValues[0].ToString();
                    }
                }

                objId++;
            }
            
            wb.SaveAs("data.xlsx");
        }

        // [SerializeField] private ScriptableObject targetObj;
        [SerializeField] private string addressablePath;
        [SerializeField] private string path;

        // [Button(ButtonSizes.Large)]
        public void TestRun()
        {
            var targetObj = (Addressables.LoadAssetAsync<ScriptableObject>(addressablePath)).WaitForCompletion();
            Undo.RecordObject(targetObj, "test");

            var pt = new PropertyTree<ScriptableObject>(new [] {targetObj});
            Debug.Log(pt.GetPropertyAtUnityPath(path).ValueEntry.WeakValues[0]);

            var prop = pt.GetPropertyAtUnityPath(path);
            prop.ValueEntry.WeakValues[0] = 10.0;
            prop.MarkSerializationRootDirty();
            pt.ApplyChanges();
        }
    }
}