using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

using Sirenix.OdinInspector;
using XLua;

using miniRAID;
using miniRAID.Spells;
using System.Runtime.CompilerServices;
using UnityEngine.Serialization;

namespace miniRAID
{
    [System.AttributeUsage(System.AttributeTargets.Class |
                       System.AttributeTargets.Struct,
                       AllowMultiple = false)  // multiuse attribute
    ]
    public class ParameterDefaultNameAttribute : System.Attribute
    {
        public string name;

        public ParameterDefaultNameAttribute(string name)
        {
            this.name = name;
        }
    }
}

public class XLuaInstance : MonoBehaviour
{
    public LuaEnv luaEnv;

    int funcID = 0;

    [Title("Debug only")]
    public ActionDataSO actionData;
    [FormerlySerializedAs("mob")] public MobRenderer mobRenderer;

    void Awake()
    {
        luaEnv = new LuaEnv();
        Coroutine InvokeStartCoroutine(IEnumerator routine) => StartCoroutine(routine);
        void InvokeStopCoroutine(Coroutine coroutine) => StopCoroutine(coroutine);
        void ShowMessage(object msg) => Globals.debugMessage.AddMessage(msg.ToString());

        luaEnv.Global.Set("csStartCoroutine", (Func<IEnumerator, Coroutine>)InvokeStartCoroutine);
        luaEnv.Global.Set("JumpIn", (Func<IEnumerator, JumpIn>)CreateJumpIn);
        luaEnv.Global.Set("csStopCoroutine", (Action<Coroutine>)InvokeStopCoroutine);
        luaEnv.Global.Set("msg", (Action<object>)ShowMessage);
        luaEnv.Global.Set("debugLog", (Action<object>)Debug.Log);
        luaEnv.Global.Set("backend", Globals.backend);

        luaEnv.Global.Set("dNum", (Func<double, string, dNumber>)dNumber.CreateStatic);

        // Pass variables to Lua
        luaEnv.Global.Set("example", this);

        luaEnv.DoString(@"
            util = require 'xlua.util'
            local unpack = unpack or table.unpack

            function startCoroutine(routine, ...)
                local params = {...}
                return csStartCoroutine(util.cs_generator(routine, unpack(params)))
            end

            function stopCoroutine(coroutine)
                csStopCoroutine(coroutine)
            end"
        );

        luaEnv.DoString("require 'miniRAIDlib'");
    }

    public JumpIn CreateJumpIn(IEnumerator em)
    {
        return new JumpIn(em);
    }

    public struct LuaDebugInfo
    {
        public string name;
    }

    public static string GetDefaultParamName(System.Type type)
    {
        string paramName = type.ToString().Split('.').Last();

        var definedName = (ParameterDefaultNameAttribute)Attribute.GetCustomAttribute(type, typeof(ParameterDefaultNameAttribute));
        if (definedName != null)
        {
            paramName = definedName.name;
        }

        return paramName;
    }

    public System.Func<TIn, TOut> GetFunc<TIn, TOut>(string LuaExpr, LuaDebugInfo info)
    {
        string intype = "";

        int typeCount = IsValueTuple(typeof(TIn));
        if (typeCount == 0)
        {
            intype = GetDefaultParamName(typeof(TIn));
        }
        else
        {
            /* Approaches for custom parameter names:
             * https://stackoverflow.com/questions/43488888/i-cant-get-parameter-names-from-valuetuple-via-reflection-in-c-sharp-7-0 - Seems impossible via ValueTuple's custom member name.
             */

            var typeArr = typeof(TIn).GetGenericArguments();
            //intype = string.Join<string>(
            //    ", ",
            //    typeArr.Select(x => GetDefaultParamName(x))
            //);

            Dictionary<Type, int> types = new();
            for(int i = 0; i < typeArr.Length; i++)
            {
                Type t = typeArr[i];
                if (!types.ContainsKey(t)) { types.Add(t, 0); }
                if(types[t] == 0) { intype += GetDefaultParamName(t); }
                else { intype += GetDefaultParamName(t) + $"{types[t]}"; }

                types[t]++;

                if(i < (typeArr.Length - 1)) { intype += ", "; }
            }

            intype = $"{intype}";
        }

        return GetFunc<TIn, TOut>(LuaExpr, info, intype);
    }

    private static readonly Dictionary<Type, int> ValTupleTypes = new Dictionary<Type, int>(){
            { typeof(ValueTuple<>), 1},
            { typeof(ValueTuple<,>), 2},
            { typeof(ValueTuple<,,>), 3},
            { typeof(ValueTuple<,,,>), 4},
            { typeof(ValueTuple<,,,,>), 5},
            { typeof(ValueTuple<,,,,,>), 6},
            { typeof(ValueTuple<,,,,,,>), 7},
        };

    static int IsValueTuple(Type type)
    {
        if(type.IsGenericType && ValTupleTypes.TryGetValue(type.GetGenericTypeDefinition(), out int count))
        {
            return count;
        }
        return 0;
    }

    public System.Func<TIn, TOut> GetFunc<TIn, TOut>(string LuaExpr, LuaDebugInfo info, string paramNames)
    {
        string postfix = $"_{funcID}", paddedLuaExpr = "";
        System.Func<TIn, TOut> result;

        string unpackTuple;
        int tupleElemCount = IsValueTuple(typeof(TIn));
        if (tupleElemCount > 0)
        {
            unpackTuple = "";
            for(int i = 0; i < tupleElemCount; i++)
            {
                unpackTuple += $"t.Item{i + 1}";
                if(i < tupleElemCount - 1)
                {
                    unpackTuple += ", ";
                }
            }
        }
        else
        {
            unpackTuple = "t";
        }

        // Coroutines
        if (typeof(IEnumerator).IsAssignableFrom(typeof(TOut)))
        {
            paddedLuaExpr = @$"function routine{postfix}({paramNames})
{LuaExpr}
end

function f{postfix}(t)
    return util.cs_generator(routine{postfix}, {unpackTuple})
end";
        }
        // Regular functions
        else
        {
            string prefix = "";
            int lineCount = LuaExpr.Split('\n').Length;
            if (lineCount == 1 && !LuaExpr.StartsWith("return ")) { prefix = "return "; }
            paddedLuaExpr = @$"function ff{postfix}({paramNames})
{prefix}{LuaExpr}
end

function f{postfix}(t)
    return ff{postfix}({unpackTuple})
end";
        }

        Debug.Log(paddedLuaExpr);
        luaEnv.DoString(paddedLuaExpr, $"{info.name} | {paramNames} -> {typeof(TOut)}");
        result = luaEnv.Global.Get<System.Func<TIn, TOut>>($"f{postfix}");

        funcID++;

        return result;
    }

    // Use this for initialization
    void Start()
    {
        //Test();
    }

    //[ContextMenu("xLua Coroutine Test")]
    //public void Test()
    //{
    //    RuntimeAction action = new RuntimeAction(actionData);
    //    StartCoroutine(action.Do(mob, new SpellTarget(Vector3Int.zero)));
    //}

    // Update is called once per frame
    void Update()
    {
        if (luaEnv != null)
        {
            luaEnv.Tick();
        }
    }
}
