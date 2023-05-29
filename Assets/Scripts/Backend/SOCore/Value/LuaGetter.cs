using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using XLua;
using Sirenix.OdinInspector;
using System.Linq;
using System;

using System.Text.RegularExpressions;

namespace miniRAID
{
    // Represents any Lua function : TIn -> TOut.
    [LuaCallCSharp]
    public class LuaGetter<TIn, TOut>
    {
        public enum LuaGetterType
        {
            STATIC,
            DYNAMIC,
            TEMPLATE,
        }

        public LuaGetter() { }
        public LuaGetter(TOut staticVal)
        {
            type = LuaGetterType.STATIC;
            staticOut = staticVal;
        }
        public LuaGetter(string expr, string desc)
        {
            type = LuaGetterType.DYNAMIC;
            LuaExpr = expr;
            description = desc;
        }

        public LuaGetterType type = LuaGetterType.STATIC;

        // TODO: change everything to private; Requires to get SerializedProperty in Editor.
        public TOut staticOut = default;

        [TextArea(0, 100)]
        public string LuaExpr;
        public string description;

        [NonSerialized]
        public System.Func<TIn, TOut> parsedDynamic;

        //[HideLabel]
        //[InlineEditor]
        [TypeFilter("GetTemplateFilterList")]
        //[Sirenix.Serialization.OdinSerialize]
        public LuaGetterTemplate<TIn, TOut> getterTemplate;

        //public object[] Unpack(TIn obj)
        //{
        //    if(IsValueTuple(obj))
        //    {
        //        return obj.GetType().GetFields().Select(f => f.GetValue(obj)).ToArray();
        //    }
        //    return new[] {(object)obj};
        //}

        public IEnumerable<Type> GetTemplateFilterList()
        {
            var q = typeof(LuaGetterTemplate<TIn, TOut>).Assembly.GetTypes()
                .Where(x => !x.IsAbstract)
                .Where(x => !x.IsGenericTypeDefinition)
                .Where(x => typeof(LuaGetterTemplate<TIn, TOut>).IsAssignableFrom(x));

            if(typeof(IEnumerator).IsAssignableFrom(typeof(TOut)))
            {
                q = q.Append(typeof(Sequence<TIn>));
            }

            return q;
        }

        public bool isNonEmpty()
        {
            return !(
                type == LuaGetterType.STATIC
            || (type == LuaGetterType.DYNAMIC && (LuaExpr == null || LuaExpr.Length == 0))
            || (type == LuaGetterType.TEMPLATE && getterTemplate == null));
        }

        public virtual TOut Eval(TIn param)
        {
            if (type == LuaGetterType.STATIC) { return staticOut; }

            if (isNonEmpty())
            {
                if (type == LuaGetterType.TEMPLATE) { return getterTemplate.Eval(param); }
                else /*if(type == LuaGetterType.DYNAMIC)*/
                {
                    // Ugly - set temporal global parameter
                    //Globals.xLuaInstance.Instance.luaEnv.Global.Set("this", self);

                    if (parsedDynamic == null)
                    {
                        //Regex regex_coroutine = new Regex(@"\@\{(?s:.*?)\}\@");
                        Regex regex_coroutine = new Regex(@"!\(((?>[^()]|(?<o>)\(|(?<-o>)\))*)\)(?(o)(?!))");
                        string replacedLuaExpr = regex_coroutine.Replace(LuaExpr, "coroutine.yield(csStartCoroutine($1))");
                        parsedDynamic = Globals.xLuaInstance.Instance.GetFunc<TIn, TOut>(replacedLuaExpr, new XLuaInstance.LuaDebugInfo { name = "LuaGetter" });
                    }

                    //return (TOut)parsedDynamic.DynamicInvoke(Unpack(param));
                    //Debug.Log(param);

                    //// Ugly - set temporal global parameter
                    //Globals.xLuaInstance.Instance.luaEnv.Global.Set("this", self);

                    // Eval function
                    var output = parsedDynamic(param);

                    // Set back to null
                    //Globals.xLuaInstance.Instance.luaEnv.Global.Set<string, object>("this", null);

                    return output;
                }
            }

            return default;
        }

        // TODO: To string

        public static implicit operator LuaGetter<TIn, TOut>(TOut val)
        {
            return new LuaGetter<TIn, TOut>() {
                staticOut = val,
                type = LuaGetterType.STATIC,
                LuaExpr = "",
                description = ""
            };
        }
    }

    public abstract class LuaGetterTemplate<TIn, TOut>
    {
        public string comment;

        public abstract TOut Eval(TIn param);
    }

    public abstract class LuaJumpInTemplate<TIn, TOut> : LuaGetterTemplate<(SerialCoroutineContext, TIn), TOut>
    {
        public TOut EvalJumpIn(SerialCoroutineContext c, TIn param)
        {
            return Eval((c, param));
        }
    }

    /*public class LuaFloatMultiplier : LuaGetter<float, float>
    {
        public LuaFloatMultiplier() : base(1.0f) {}

        public LuaFloatMultiplier(float staticOut) : base(staticOut){}

        public override float Eval(float param)
        {
            if (type == LuaGetterType.STATIC) { return param * staticOut; }
            return base.Eval(param);
        }
    }*/

    // Attribute used by HealthBarAttributeDrawer.
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class EventSlotAttribute : Attribute
    {
        public EventSlotAttribute()
        {
        }
    }

    public class LuaFunc<TIn, TOut> : LuaGetter<TIn, TOut>
    {
        public LuaFunc()
        {
            type = LuaGetter<TIn, TOut>.LuaGetterType.DYNAMIC;
        }
    }

    public class None { }

    [System.Serializable]
    public class LuaBoundedGetter<TIn, TBoundIn, TOut> : LuaGetter<TIn, TOut>
    {
        public LuaGetter<TBoundIn, TOut> lowerBound, upperBound;

        public (TOut, TOut) PrecalculatedBounds(TBoundIn args)
        {
            if (type == LuaGetterType.STATIC)
            {
                return (staticOut, staticOut);
            }
            else if (type == LuaGetterType.DYNAMIC)
            {
                return (lowerBound.Eval(args), upperBound.Eval(args));
            }
            else
            {
                Debug.LogError("Unsupported LuaGetter type.");
                return (default, default);
            }
        }

        // TODO: To string

        public static implicit operator LuaBoundedGetter<TIn, TBoundIn, TOut>(TOut val)
        {
            return new LuaBoundedGetter<TIn, TBoundIn, TOut>()
            {
                staticOut = val,
                type = LuaGetterType.STATIC,
                lowerBound = val,
                upperBound = val,
                LuaExpr = "",
                description = ""
            };
        }
    }
}
