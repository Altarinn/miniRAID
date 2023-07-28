using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using System.Linq;

using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using System;
using miniRAID.Buff;
using Sirenix.Serialization;

namespace miniRAID.Editor
{
    public class DynamicValueDrawer : OdinValueDrawer<dNumber>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            dNumber value = ValueEntry.SmartValue;
            //SerializedProperty property = prop

            GenericMenu typeMenu = new GenericMenu();
            typeMenu.AddItem(
                new GUIContent("Static"),
                value.valueType == dNumber.ValueType.STATIC,
                OnVarTypeSelected, dNumber.ValueType.STATIC);
            //typeMenu.AddItem(
            //    new GUIContent("Dynamic (Lua)"),
            //    value.valueType == dNumber.ValueType.DYNAMIC,
            //    OnVarTypeSelected, dNumber.ValueType.DYNAMIC);
            typeMenu.AddItem(
                new GUIContent("Composition"),
                value.valueType == dNumber.ValueType.COMPOSITION,
                OnVarTypeSelected, dNumber.ValueType.COMPOSITION);

            //GUILayout.Label("Test");
            EditorGUILayout.BeginHorizontal();
            if (label != null)
            {
                EditorGUILayout.PrefixLabel(label);
            }

            string s = "?";
            if(value.valueType == dNumber.ValueType.STATIC) { s = "S"; }
            //if(value.valueType == dNumber.ValueType.DYNAMIC) { s = "D"; }
            if(value.valueType == dNumber.ValueType.COMPOSITION) { s = "C"; }

            if (EditorGUILayout.DropdownButton(new GUIContent(s), FocusType.Keyboard, GUILayout.Width(30)))
            {
                typeMenu.ShowAsContext();
            }
            //EditorGUILayout.EnumPopup(value.valueType);

            switch(value.valueType)
            {
                case dNumber.ValueType.STATIC:
                    value.staticValue = SirenixEditorFields.DoubleField((GUIContent)null, value.staticValue); 
                    break;
                //case dNumber.ValueType.DYNAMIC:
                //    EditorGUILayout.LabelField("Inputs: " + (value.luaParameters == "" ? "None" : value.luaParameters));
                //    break;
                case dNumber.ValueType.COMPOSITION:
                    GUI.enabled = false;
                    SirenixEditorFields.DoubleField((GUIContent)null, value.Value);
                    GUI.enabled = true;
                    break;
            }

            EditorGUILayout.EndHorizontal();

            //if(value.valueType == dNumber.ValueType.DYNAMIC)
            //{
            //    value.dynamicLuaExpr = EditorGUILayout.TextArea(value.dynamicLuaExpr);
            //}
        }

        void OnVarTypeSelected(object type)
        {
            var newV = ValueEntry.SmartValue;
            newV.valueType = (dNumber.ValueType)type;
            ValueEntry.SmartValue = newV;
        }
    }

    // A bit confused ... What is T? LuaGetter or LuaGetter<TIn, TOut>??
    [DrawerPriority(super: 0.1)]
    public class LuaGetterDrawer<T, TIn, TOut> : OdinValueDrawer<T> where T : LuaGetter<TIn, TOut>, new()
    {
        public static Vector3 LerpHSV(Color a, Color b, float x)
        {
            Vector3 ah;
            Color.RGBToHSV(a, out ah.x, out ah.y, out ah.z);
            Vector3 bh;
            Color.RGBToHSV(b, out bh.x, out bh.y, out bh.z);

            return new Vector3(
                Mathf.LerpAngle(ah.x, bh.x, x),
                Mathf.Lerp(ah.y, bh.y, x),
                Mathf.Lerp(ah.z, bh.z, x)
            );
        }

        private Color LerpColorHSV(Color a, Color b, float t)
        {
            var newHSV = LerpHSV(a, b, t);
            return Color.HSVToRGB(newHSV.x, newHSV.y, newHSV.z);
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            // Handle null values
            //LuaGetter<TIn, TOut> value = ValueEntry.Values[0];
            LuaGetter<TIn, TOut> value = ValueEntry.SmartValue;
            if (ValueEntry.ValueState == PropertyValueState.NullReference)
            {
                // Create automatically if null
                Debug.Log("Null, creating");
                ValueEntry.SmartValue = new T();
            }
            else
            {
                // Label
                bool isEvt = Property.Attributes.Any(attr => attr.GetType() == typeof(EventSlotAttribute));

                Color oldColor = Color.white;
                Color baseColor = Color.white;

                if (isEvt)
                {
                    int depth = 0;
                    var prop = Property;
                    while (prop != null)
                    {
                        prop = prop.Parent;
                        ++depth;
                    }

                    //EditorStyles
                    oldColor = GUI.backgroundColor;

                    baseColor = LerpColorHSV(
                        new Color(1.0f, 0.82f, 0.404f),
                        new Color(0.0f, 0.565f, 0.894f),
                        Mathf.Max(depth - 2, 0) / 10.0f
                    );

                    GUI.backgroundColor = Color.Lerp
                        (Color.white, baseColor, Mathf.Max(depth, 0) / 4.0f);

                    SirenixEditorGUI.BeginBox();
                }

                EditorGUILayout.BeginHorizontal();
                if (label != null)
                {
                    GUIContent content = label;
                    if (isEvt)
                    {
                        var labelStyle = new GUIStyle(GUI.skin.label);
                        labelStyle.normal.textColor = baseColor;
                        labelStyle.hover.textColor = Color.Lerp(Color.white, baseColor, 0.25f);
                        labelStyle.alignment = TextAnchor.UpperLeft;
                        labelStyle.fontStyle = FontStyle.Bold;

                        label.text = $"↯ {Property.Name}";

                        EditorGUILayout.PrefixLabel(label, new GUIStyle(GUI.skin.box), labelStyle);
                    }
                    else
                    {
                        EditorGUILayout.PrefixLabel(label);
                    }
                }

                GenericMenu typeMenu = new GenericMenu();
                if (!isEvt)
                {
                    typeMenu.AddItem(
                        new GUIContent("Static"),
                        value.type == LuaGetter<TIn, TOut>.LuaGetterType.STATIC,
                        OnVarTypeSelected, LuaGetter<TIn, TOut>.LuaGetterType.STATIC);
                }
                else
                {
                    typeMenu.AddItem(
                        new GUIContent("Disabled (static)"),
                        value.type == LuaGetter<TIn, TOut>.LuaGetterType.STATIC,
                        OnVarTypeSelected, LuaGetter<TIn, TOut>.LuaGetterType.STATIC);
                }

                typeMenu.AddItem(
                    new GUIContent("Dynamic (Lua)"),
                    value.type == LuaGetter<TIn, TOut>.LuaGetterType.DYNAMIC,
                    OnVarTypeSelected, LuaGetter<TIn, TOut>.LuaGetterType.DYNAMIC);
                typeMenu.AddItem(
                    new GUIContent("Template"),
                    value.type == LuaGetter<TIn, TOut>.LuaGetterType.TEMPLATE,
                    OnVarTypeSelected, LuaGetter<TIn, TOut>.LuaGetterType.TEMPLATE);

                System.Type typ = typeof(TIn);

                string s = "?";
                if (!isEvt && value.type == LuaGetter<TIn, TOut>.LuaGetterType.STATIC)
                {
                    s = "S";
                }

                if (isEvt && value.type == LuaGetter<TIn, TOut>.LuaGetterType.STATIC)
                {
                    s = "-";
                }

                if (value.type == LuaGetter<TIn, TOut>.LuaGetterType.DYNAMIC)
                {
                    s = "D";
                }

                if (value.type == LuaGetter<TIn, TOut>.LuaGetterType.TEMPLATE)
                {
                    s = "T";
                }

                if (EditorGUILayout.DropdownButton(new GUIContent(s), FocusType.Keyboard, GUILayout.Width(30)))
                {
                    typeMenu.ShowAsContext();
                }
                //EditorGUILayout.EnumPopup(value.valueType);

                int lvl = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;

                switch (value.type)
                {
                    case LuaGetter<TIn, TOut>.LuaGetterType.STATIC:
                        //if (typeof(TOut) == typeof(int))
                        //{
                        //    value.staticOut = SirenixEditorFields.IntField((dynamic)value.staticOut);
                        //}
                        //if (typeof(TOut) == typeof(float))
                        //{
                        //    value.staticOut = SirenixEditorFields.FloatField((dynamic)value.staticOut);
                        //}
                        //if (typeof(TOut) == typeof(double))
                        //{
                        //    value.staticOut = SirenixEditorFields.DoubleField((dynamic)value.staticOut);
                        //}
                        //if (typeof(TOut) == typeof(bool))
                        //{
                        //    //value.staticOut = EditorGUILayout.Toggle((dynamic)value.staticOut);
                        //    Property.FindChild(x => x.Name == "staticOut", false).Draw(null);
                        //}
                        //if (typeof(TOut).IsEnum)
                        //{
                        //    Property.FindChild(x => x.Name == "staticOut", false).Draw(null);
                        //}
                        if (!isEvt)
                        {
                            DrawStatic();
                        }

                        break;
                    case LuaGetter<TIn, TOut>.LuaGetterType.DYNAMIC:
                    case LuaGetter<TIn, TOut>.LuaGetterType.TEMPLATE:
                        string intype = "";
                        var typeArr = typeof(TIn).GetGenericArguments();
                        if (typeArr.Length == 0)
                        {
                            intype = XLuaInstance.GetDefaultParamName(typeof(TIn));
                        }
                        else
                        {
                            intype = string.Join<string>(
                                ", ",
                                typeArr.Select(x => XLuaInstance.GetDefaultParamName(x)
                                ));
                            intype = $"{intype}";
                        }

                        string outtype = typeof(TOut).ToString().Split('.').Last();

                        SirenixEditorGUI.Title($"{intype} → {outtype}", null, TextAlignment.Left, false, false);
                        //EditorGUILayout.LabelField($"{intype} → {outtype}");
                        break;
                }

                EditorGUI.indentLevel = lvl;
                EditorGUILayout.EndHorizontal();

                if (value.type == LuaGetter<TIn, TOut>.LuaGetterType.DYNAMIC)
                {
                    SirenixEditorGUI.BeginBox();

                    if (typeof(T) != typeof(LuaFunc<TIn, TOut>))
                    {
                        value.description = SirenixEditorFields.TextField("Description", value.description);
                    }

                    if (typeof(T).GetGenericTypeDefinition() == typeof(LuaBoundedGetter<,,>))
                    {
                        SirenixEditorGUI.BeginBox();
                        Property.FindChild(prop => prop.Name == "lowerBound", false).Draw();
                        Property.FindChild(prop => prop.Name == "upperBound", false).Draw();
                        SirenixEditorGUI.EndBox();
                    }

                    //value.LuaExpr = EditorGUILayout.TextArea(value.LuaExpr);
                    Property.FindChild(x => x.Name == "LuaExpr", false).Draw(null);

                    SirenixEditorGUI.EndBox();
                }

                if (value.type == LuaGetter<TIn, TOut>.LuaGetterType.TEMPLATE)
                {
                    var p = Property.FindChild(prop => prop.Name == "getterTemplate", false);
                    if (p != null)
                    {
                        //SirenixEditorGUI.BeginBoxHeader();
                        //EditorGUILayout.LabelField("TEST");
                        //SirenixEditorGUI.EndBoxHeader();

                        //GUILayout.Space(10 * lvl);
                        p.Draw(null);
                    }
                    //SirenixEditorGUI.draw
                }

                if (isEvt)
                {
                    SirenixEditorGUI.EndBox();
                    GUI.backgroundColor = oldColor;
                }

                //Sirenix.OdinInspector.Editor.Drawers.NullableReferenceDrawer

                //Property.GetActiveDrawerChain().MoveNext();
                //CallNextDrawer(null);
            }
        }

        void DrawStatic()
        {
            Property.FindChild(x => x.Name == "staticOut", false).Draw(null);
        }
        
        void OnVarTypeSelected(object type)
        {
            ValueEntry.SmartValue.type = (LuaGetter<TIn, TOut>.LuaGetterType)type;
        }
    }

    [DrawerPriority(super:0.1)]
    public class PowerGetterDrawer : OdinValueDrawer<PowerGetter>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            // Handle null values
            PowerGetter value = ValueEntry.SmartValue;
            if (ValueEntry.ValueState == PropertyValueState.NullReference)
            {
                // Create automatically if null
                Debug.Log("Null, creating");
                ValueEntry.SmartValue = new PowerGetter(1);
            }
            else
            {
                // Label
                EditorGUILayout.BeginHorizontal();
                if (label != null)
                {
                    GUIContent content = label;
                    EditorGUILayout.PrefixLabel(label);
                }

                GenericMenu typeMenu = new GenericMenu();
                typeMenu.AddItem(
                    new GUIContent("AttackPower"),
                    value.powerType == PowerGetter.PowerGetterType.AttackPower,
                    OnVarTypeSelected, PowerGetter.PowerGetterType.AttackPower);
                typeMenu.AddItem(
                    new GUIContent("SpellPower"),
                    value.powerType == PowerGetter.PowerGetterType.SpellPower,
                    OnVarTypeSelected, PowerGetter.PowerGetterType.SpellPower);
                typeMenu.AddItem(
                    new GUIContent("HealPower"),
                    value.powerType == PowerGetter.PowerGetterType.HealPower,
                    OnVarTypeSelected, PowerGetter.PowerGetterType.HealPower);
                typeMenu.AddItem(
                    new GUIContent("BuffPower"),
                    value.powerType == PowerGetter.PowerGetterType.BuffPower,
                    OnVarTypeSelected, PowerGetter.PowerGetterType.BuffPower);
                typeMenu.AddItem(
                    new GUIContent("Static"),
                    value.powerType == PowerGetter.PowerGetterType.STATIC,
                    OnVarTypeSelected, PowerGetter.PowerGetterType.STATIC);
                typeMenu.AddItem(
                    new GUIContent("Dynamic"),
                    value.powerType == PowerGetter.PowerGetterType.DYNAMIC,
                    OnVarTypeSelected, PowerGetter.PowerGetterType.DYNAMIC);

                string s = "?";
                if (value.powerType == PowerGetter.PowerGetterType.AttackPower) { s = "AttackP %"; }
                if (value.powerType == PowerGetter.PowerGetterType.SpellPower) { s = "SpellP %"; }
                if (value.powerType == PowerGetter.PowerGetterType.HealPower) { s = "HealP %"; }
                if (value.powerType == PowerGetter.PowerGetterType.BuffPower) { s = "BuffP %"; }
                if (value.powerType == PowerGetter.PowerGetterType.STATIC) { s = "STATIC"; }
                if (value.powerType == PowerGetter.PowerGetterType.DYNAMIC) { s = "DYNAMIC"; }

                if (EditorGUILayout.DropdownButton(new GUIContent(s), FocusType.Keyboard, GUILayout.Width(80)))
                {
                    typeMenu.ShowAsContext();
                }

                int lvl = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;

                switch (value.powerType)
                {
                    case PowerGetter.PowerGetterType.STATIC:
                        Property.FindChild(x => x.Name == "powerFactor", false).Draw(null);
                        break;
                    case PowerGetter.PowerGetterType.DYNAMIC:
                        EditorGUILayout.LabelField("C# Implementation");
                        break;
                    case PowerGetter.PowerGetterType.AttackPower:
                    case PowerGetter.PowerGetterType.BuffPower:
                    case PowerGetter.PowerGetterType.HealPower:
                    case PowerGetter.PowerGetterType.SpellPower:
                        Property.FindChild(x => x.Name == "powerFactor", false).Draw(null);
                        break;
                }

                EditorGUI.indentLevel = lvl;
                EditorGUILayout.EndHorizontal();

                //Sirenix.OdinInspector.Editor.Drawers.NullableReferenceDrawer

                //Property.GetActiveDrawerChain().MoveNext();
                //CallNextDrawer(null);
            }
        }

        void OnVarTypeSelected(object type)
        {
            Property.RecordForUndo();
            ValueEntry.SmartValue.powerType = (PowerGetter.PowerGetterType)type;
        }
    }
    
    public class LeveledStatsDrawer<T> : OdinValueDrawer<LeveledStats<T>>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            // Handle null values
            LeveledStats<T> value = ValueEntry.SmartValue;
            if (value.values == null)
            {
                Property.RecordForUndo();
                value.values = new T[Consts.MaxListenerLevels];
                if (typeof(T) == typeof(float))
                {
                    for(int i = 0; i < Consts.MaxListenerLevels; i++)
                    {
                        value.values[i] = (dynamic)1.0f;
                    }
                }
                ValueEntry.SmartValue = value;
            }
            else
            {
                var p = Property.FindChild(x => x.Name == "values", false);
                if (value.isLeveled)
                {
                    for (int i = 0; i < value.values.Length; i++)
                    {
                        p.Children[i].Draw(null);
                    }
                }
                else
                {
                    p.Children[0].Draw(null);
                }
                
                value.isLeveled = SirenixEditorGUI.ToolbarToggle(value.isLeveled, "L");
                if (value.isLeveled != ValueEntry.SmartValue.isLeveled)
                {
                    Property.RecordForUndo();
                    ValueEntry.SmartValue = value;
                }
            }
        }
    }
    
    public class ActionSOEntryDrawer : OdinValueDrawer<ActionSOEntry>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            EditorGUILayout.BeginHorizontal();
            
            // Handle null values
            ActionSOEntry value = ValueEntry.SmartValue;
            var pD = Property.FindChild(x => x.Name == "data", false);
            var pL = Property.FindChild(x => x.Name == "level", false);

            pD.Draw(label);
            value.level = SirenixEditorFields.IntField((string)null, value.level + 1, GUILayoutOptions.MaxWidth(35)) - 1;
            
            if(value.level != ValueEntry.SmartValue.level)
            {
                Property.RecordForUndo();
                ValueEntry.SmartValue = value;
            }
            
            EditorGUILayout.EndHorizontal();
        }
    }
    
    public class ActionSOEntryDrawer<T> : OdinValueDrawer<ActionSOEntry<T>> where T : ActionDataSO
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            EditorGUILayout.BeginHorizontal();
            
            // Handle null values
            ActionSOEntry<T> value = ValueEntry.SmartValue;
            var pD = Property.FindChild(x => x.Name == "data", false);
            var pL = Property.FindChild(x => x.Name == "level", false);

            pD.Draw(label);
            value.level = SirenixEditorFields.IntField((string)null, value.level + 1, GUILayoutOptions.MaxWidth(35)) - 1;
            
            if(value.level != ValueEntry.SmartValue.level)
            {
                Property.RecordForUndo();
                ValueEntry.SmartValue = value;
            }
            
            EditorGUILayout.EndHorizontal();
        }
    }
    
    public class BuffSOEntryDrawer : OdinValueDrawer<BuffSOEntry>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            EditorGUILayout.BeginHorizontal();
            
            // Handle null values
            BuffSOEntry value = ValueEntry.SmartValue;
            var pD = Property.FindChild(x => x.Name == "data", false);
            var pL = Property.FindChild(x => x.Name == "level", false);

            pD.Draw(label);
            value.level = SirenixEditorFields.IntField((string)null, value.level + 1, GUILayoutOptions.MaxWidth(35)) - 1;
            
            if(value.level != ValueEntry.SmartValue.level)
            {
                Property.RecordForUndo();
                ValueEntry.SmartValue = value;
            }
            
            EditorGUILayout.EndHorizontal();
        }
    }
    
    public class MobListenerSOEntryDrawer : OdinValueDrawer<MobListenerSOEntry>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            EditorGUILayout.BeginHorizontal();
            
            // Handle null values
            MobListenerSOEntry value = ValueEntry.SmartValue;
            var pD = Property.FindChild(x => x.Name == "data", false);
            var pL = Property.FindChild(x => x.Name == "level", false);

            pD.Draw(label);
            value.level = SirenixEditorFields.IntField((string)null, value.level + 1, GUILayoutOptions.MaxWidth(35)) - 1;
            
            if(value.level != ValueEntry.SmartValue.level)
            {
                Property.RecordForUndo();
                ValueEntry.SmartValue = value;
            }
            
            EditorGUILayout.EndHorizontal();
        }
    }

    [DrawerPriority(super: 0.1)]
    public class NoneAsSpacerDrawer : OdinValueDrawer<None>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            //if(label != null)
            //{
            //    EditorGUILayout.LabelField(label);
            //}
        }
    }

    //// Place the drawer script file in an Editor folder or wrap it in a #if UNITY_EDITOR condition.
    //[DrawerPriority(super:0.1)]
    //public class EventSlotAttributeDrawer<T, TIn, TOut> : OdinAttributeDrawer<EventSlotAttribute, T> where T : LuaGetter<TIn, TOut>
    //{
    //    protected override void DrawPropertyLayout(GUIContent label)
    //    {
    //        //this.DrawProperty(null);

    //        //if (label != null)
    //        //{
    //        //    EditorGUILayout.PrefixLabel(label);
    //        //}

    //        // Skip next drawer
    //        Property.GetActiveDrawerChain().MoveNext();
    //        if(ValueEntry.Values[0] != null)
    //        {
    //            // Call the next drawer, which will draw the LuaFunc field.
    //            this.CallNextDrawer(label);
    //        }
    //    }
    //}

    //public class LuaFuncDrawer<TIn, TOut> : OdinValueDrawer<LuaFunc<TIn, TOut>>
    //public class LuaFuncDrawer<T, TIn, TOut> : OdinValueDrawer<T> where T : LuaFunc<TIn, TOut>
    //{
    //    protected override void DrawPropertyLayout(GUIContent label)
    //    {
    //        LuaGetter<TIn, TOut> value = ValueEntry.Values[0];
    //        //SerializedProperty property = prop

    //        Debug.Log(value.GetType().GetCustomAttributes(true));

    //        GenericMenu typeMenu = new GenericMenu();
    //        typeMenu.AddItem(
    //            new GUIContent("Static"),
    //            value.type == LuaGetter<TIn, TOut>.LuaGetterType.STATIC,
    //            OnVarTypeSelected, LuaGetter<TIn, TOut>.LuaGetterType.STATIC);
    //        typeMenu.AddItem(
    //            new GUIContent("Dynamic (Lua)"),
    //            value.type == LuaGetter<TIn, TOut>.LuaGetterType.DYNAMIC,
    //            OnVarTypeSelected, LuaGetter<TIn, TOut>.LuaGetterType.DYNAMIC);

    //        //GUILayout.Label("Test");
    //        EditorGUILayout.BeginHorizontal();
    //        if (label != null)
    //        {
    //            EditorGUILayout.PrefixLabel(label);
    //        }

    //        System.Type typ = typeof(TIn);

    //        string s = "?";
    //        if (value.type == LuaGetter<TIn, TOut>.LuaGetterType.STATIC) { s = "S"; }
    //        if (value.type == LuaGetter<TIn, TOut>.LuaGetterType.DYNAMIC) { s = "D"; }

    //        if (EditorGUILayout.DropdownButton(new GUIContent(s), FocusType.Keyboard, GUILayout.Width(30)))
    //        {
    //            typeMenu.ShowAsContext();
    //        }
    //        //EditorGUILayout.EnumPopup(value.valueType);

    //        switch (value.type)
    //        {
    //            case LuaGetter<TIn, TOut>.LuaGetterType.STATIC:
    //                if (typeof(TOut) == typeof(int))
    //                {
    //                    value.staticOut = EditorGUILayout.IntField((dynamic)value.staticOut);
    //                }
    //                if (typeof(TOut) == typeof(double))
    //                {
    //                    value.staticOut = EditorGUILayout.DoubleField((dynamic)value.staticOut);
    //                }
    //                if (typeof(TOut) == typeof(bool))
    //                {
    //                    value.staticOut = EditorGUILayout.Toggle((dynamic)value.staticOut);
    //                }
    //                break;
    //            case LuaGetter<TIn, TOut>.LuaGetterType.DYNAMIC:

    //                string intype = "";
    //                var typeArr = typeof(TIn).GetGenericArguments();
    //                if (typeArr.Length == 0)
    //                {
    //                    intype = typeof(TIn).ToString().Split('.').Last();
    //                }
    //                else
    //                {
    //                    intype = string.Join<string>(
    //                        ", ",
    //                        typeArr.Select(x => x.ToString().Split('.').Last()
    //                    ));
    //                    intype = $"{intype}";
    //                }

    //                string outtype = typeof(TOut).ToString().Split('.').Last();

    //                EditorGUILayout.LabelField($"{intype} → {outtype}");
    //                break;
    //        }

    //        EditorGUILayout.EndHorizontal();

    //        if (value.type == LuaGetter<TIn, TOut>.LuaGetterType.DYNAMIC)
    //        {
    //            value.description = EditorGUILayout.TextField("Description", value.description);
    //            value.LuaExpr = EditorGUILayout.TextArea(value.LuaExpr);
    //        }
    //    }

    //    void OnVarTypeSelected(object type)
    //    {
    //        ValueEntry.SmartValue.type = (LuaGetter<TIn, TOut>.LuaGetterType)type;
    //    }
    //}
}
