using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using XLua;
using Sirenix.OdinInspector;

namespace miniRAID
{
    // RIP Generic DynamicValue<T>
    [System.Serializable]
    [LuaCallCSharp]
    public struct dNumber
    {
        [System.Serializable]
        public struct DynamicValueSource
        {
            public string source;
        }

        public enum ValueType
        {
            STATIC,
            //DYNAMIC,
            //DYNAMIC_CSHARP,
            COMPOSITION
        }
        public ValueType valueType;

        // TODO: change everything to private; Requires to get SerializedProperty in Editor.
        [SerializeField]
        public double staticValue;
        //public string dynamicLuaExpr, luaParameters;

        public DynamicValueSource source;

        // Dynamic value
        // TODO: Some kind of recording function
        public double baseValue;
        public double addedValue;
        public double addMultiplier, mulMultiplier;

        public double Value
        {
            get
            {
                switch(valueType)
                {
                    case ValueType.STATIC:
                        return staticValue;
                        break;
                    //case ValueType.DYNAMIC:
                    //    return default;
                    //    break;
                    case ValueType.COMPOSITION:
                        return ((baseValue * addMultiplier) + addedValue) * mulMultiplier;
                    //case ValueType.DYNAMIC_CSHARP:
                    default:
                        Debug.LogError("Unsupported value type!");
                        return default;
                        break;
                }
            }
        }

        public static dNumber CreateStatic(double v, string source = "Unknown")
        {
            dNumber o;
            o.valueType = ValueType.STATIC;
            o.staticValue = v;
            o.source = new DynamicValueSource { source = source };
            o.baseValue = 0;
            o.addedValue = 0;
            o.addMultiplier = 1.0;
            o.mulMultiplier = 1.0;

            return o;
        }

        public static dNumber CreateComposite(double baseValue, string source = "Unknown")
        {
            dNumber o;
            o.valueType = ValueType.COMPOSITION;
            o.staticValue = 0;
            o.source = new DynamicValueSource { source = source };
            o.baseValue = baseValue;
            o.addedValue = 0;
            o.addMultiplier = 1.0;
            o.mulMultiplier = 1.0;

            return o;
        }

        //public dNumber() { luaParameters = ""; }
        //public dNumber(string luaParams) { luaParameters = luaParams; }

        public static implicit operator double(dNumber d) => d.Value;
        public static implicit operator float(dNumber d) => (float)d.Value;
        public static implicit operator int(dNumber d) => (int)d.Value;
        public static explicit operator dNumber(double d)
        {
            return CreateStatic(d);
        }

        public static explicit operator dNumber((double, string) param)
        {
            return CreateStatic(param.Item1, param.Item2);
        }

        public static dNumber operator +(dNumber a, dNumber b)
        {
            dNumber o;
            if (a.valueType != ValueType.COMPOSITION)
            {
                //o = CreateInstance<dNumber>();
                o = CreateComposite(a.Value, "unknown");
            }
            else
            {
                o = a;
            }

            o.Add(b);

            return o;
        }

        public static dNumber operator *(dNumber a, dNumber b)
        {
            dNumber o;
            if (a.valueType != ValueType.COMPOSITION)
            {
                //o = CreateInstance<dNumber>();
                o = CreateComposite(a.Value, "unknown");
            }
            else
            {
                o = a;
            }

            o.Mul(b);

            return o;
        }

        public void Add(dNumber v)
        {
            addedValue += v;
        }

        public void Substract(dNumber v)
        {
            addedValue -= v;
        }

        public void Mul(dNumber f)
        {
            addMultiplier += f - 1;
            if(addMultiplier < 0)
            {
                addMultiplier = 0;
            }
        }

        public void MulMul(dNumber multiplier)
        {
            if(valueType != ValueType.COMPOSITION)
            {
                Debug.LogError("Cannot MulMul a non-COMPOSITION typed DynamicValue!");
                return;
            }

            mulMultiplier *= multiplier.Value;
        }

        public static dNumber NewWithBase(dNumber baseVal)
        {
            //dNumber o = CreateInstance<dNumber>();
            dNumber o = CreateComposite(baseVal.Value, "unknown");

            return o;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    //[System.Serializable]
    //public class dNumberMob : dNumber
    //{
    //    public dNumberMob()
    //    {
    //        luaParameters = "mob";
    //    }
    //}

    //[System.Serializable]
    //public class dNumberMobTarget : dNumber
    //{
    //    public dNumberMobTarget()
    //    {
    //        luaParameters = "mob, target";
    //    }
    //}
}
