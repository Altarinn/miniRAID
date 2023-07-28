using System;
using XLua;
using Sirenix.OdinInspector;

namespace miniRAID
{
    [LuaCallCSharp]
    [ParameterDefaultName("cost")]
    public class Cost
    {
        [LuaCallCSharp]
        public enum Type
        {
            AP,
            Mana,
            CD,
            
            // Paladin
            OracleGrid, // Oracle grids
            OracleBuff, // Oracle buffs
        }

        public dNumber value;
        public Type type;

        public Cost(dNumber v, Type t)
        {
            value = v;
            type = t;
        }

        public void Add(dNumber v)
        {
            value.Add(v);
        }

        public void Substract(dNumber v)
        {
            value.Substract(v);
        }

        public void Mul(dNumber v)
        {
            value.Mul(v);
        }

        public void MulMul(dNumber v)
        {
            value.MulMul(v);
        }

        public static implicit operator dNumber(Cost c)
        {
            return c.value;
        }
    }
}

