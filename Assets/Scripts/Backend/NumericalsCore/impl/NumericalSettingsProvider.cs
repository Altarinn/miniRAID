using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace miniRAID.Backend.Numericals.Impl
{
    public class NumericalSettingsProvider : SettingsProvider
    {
        // 第一階層をProjectにします
        private const string SettingPath = "Project/Numericals Settings";
        
        private Editor _editor;

        [SettingsProvider]
        public static SettingsProvider CreateProvider()
        {
            // SettingsScopeをProjectにします
            return new NumericalSettingsProvider(SettingPath, SettingsScope.Project, null);
        }

        public NumericalSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords) : base(path, scopes, keywords)
        {
        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            var fundamentalNumericalsSO = FundamentalNumericalsSO.instance;
            fundamentalNumericalsSO.hideFlags = HideFlags.HideAndDontSave & ~HideFlags.NotEditable; // ScriptableSingletonを編集可能にする（本文で補足）
            
            // 設定ファイルの標準のインスペクターのエディタを生成
            Editor.CreateCachedEditor(fundamentalNumericalsSO, null, ref _editor);
        }

        public override void OnGUI(string searchContext)
        {
            // EditorGUILayout.LabelField("Test");
            EditorGUI.BeginChangeCheck();
            // 設定ファイルの標準のインスペクターを表示
            _editor.OnInspectorGUI();
            if (EditorGUI.EndChangeCheck())
            {
                // 差分があったら保存
                FundamentalNumericalsSO.instance.Save();
            }
        }
    }
}