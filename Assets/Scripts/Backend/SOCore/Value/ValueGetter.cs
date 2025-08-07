using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;
using System.Linq;
using System;

using System.Text.RegularExpressions;
using miniRAID.Backend.Numericals;

namespace miniRAID
{
    // Represents any Lua function : TIn -> TOut.
    public class ValueGetter<TIn, TOut>
    {
        public enum LuaGetterType
        {
            STATIC,
            // DYNAMIC,
            TEMPLATE,
        }

        public ValueGetter() { }
        public ValueGetter(TOut staticVal)
        {
            type = LuaGetterType.STATIC;
            staticOut = staticVal;
        }

        public LuaGetterType type = LuaGetterType.STATIC;

        // TODO: change everything to private; Requires to get SerializedProperty in Editor.
        [PathBell]
        public TOut staticOut = default;

        [TypeFilter("GetTemplateFilterList")]
        public LuaGetterTemplate<TIn, TOut> getterTemplate;

        public IEnumerable<Type> GetTemplateFilterList()
        {
            var q = typeof(LuaGetterTemplate<TIn, TOut>).Assembly.GetTypes()
                .Where(x => !x.IsAbstract)
                .Where(x => !x.IsGenericTypeDefinition)
                .Where(x => typeof(LuaGetterTemplate<TIn, TOut>).IsAssignableFrom(x));

            return q;
        }

        public bool isNonEmpty()
        {
            return !(
                type == LuaGetterType.STATIC
            || (type == LuaGetterType.TEMPLATE && getterTemplate == null));
        }

        public virtual TOut Eval(TIn param)
        {
            if (type == LuaGetterType.STATIC) { return staticOut; }

            if (isNonEmpty())
            {
                if (type == LuaGetterType.TEMPLATE) { return getterTemplate.Eval(param); }
            }

            return default;
        }

        // TODO: To string

        public static implicit operator ValueGetter<TIn, TOut>(TOut val)
        {
            return new ValueGetter<TIn, TOut>() {
                staticOut = val,
                type = LuaGetterType.STATIC,
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

    // Attribute used by HealthBarAttributeDrawer.
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class EventSlotAttribute : Attribute
    {
        public EventSlotAttribute()
        {
        }
    }

    public class None { }

    [System.Serializable]
    public class ValueBoundedGetter<TIn, TBoundIn, TOut> : ValueGetter<TIn, TOut>
    {
        public ValueGetter<TBoundIn, TOut> lowerBound, upperBound;

        public (TOut, TOut) PrecalculatedBounds(TBoundIn args)
        {
            if (type == LuaGetterType.STATIC)
            {
                return (staticOut, staticOut);
            }
            else
            {
                Debug.LogError("Unsupported LuaGetter type.");
                return (default, default);
            }
        }

        // TODO: To string

        public static implicit operator ValueBoundedGetter<TIn, TBoundIn, TOut>(TOut val)
        {
            return new ValueBoundedGetter<TIn, TBoundIn, TOut>()
            {
                staticOut = val,
                type = LuaGetterType.STATIC,
                lowerBound = val,
                upperBound = val,
            };
        }
    }
}
