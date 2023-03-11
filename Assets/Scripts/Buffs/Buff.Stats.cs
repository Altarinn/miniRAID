using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace miniRAID.Buff
{
    public partial class BuffSO : MobListenerSO
    {
        public enum StatModTarget
        {
            // Main stats
            VIT,
            STR,
            MAG,
            INT,
            DEX,
            TEC,

            // Sub stats
            // TODO

            // Battle stats

            AttackPower,
            SpellPower,
            HealPower,
            BuffPower,

            Defense,
            SpDefense,
            AggroMul,

            Hit,
            Dodge,
            Crit,
            CritRes,
            ExRange,

            APRegen,
        }

        public enum dNumberModType
        {
            Add,
            Mul,
            MulMul
        }

        [InlineProperty(LabelWidth = 40)]
        public struct dNumberModifier
        {
            [HorizontalGroup]
            [LabelWidth(32)]
            public dNumberModType type;

            [HorizontalGroup]
            public LuaGetter<Mob, float> value;
        }

        public void ModifydNumber(ref dNumber num, Mob mob, dNumberModifier mod)
        {
            switch(mod.type)
            {
                case dNumberModType.Add:
                    num.Add(dNumber.CreateStatic(mod.value.Eval(mob), "unknown"));
                    break;
                case dNumberModType.Mul:
                    num.Mul(dNumber.CreateStatic(mod.value.Eval(mob), "unknown"));
                    break;
                case dNumberModType.MulMul:
                    num.MulMul(dNumber.CreateStatic(mod.value.Eval(mob), "unknown"));
                    break;
            }
        }

        public void ModifyBaseStats(Mob mob, Dictionary<StatModTarget, dNumberModifier> modifiers)
        {
            foreach (var kv in modifiers)
            {
                switch(kv.Key)
                {
                    case StatModTarget.VIT: 
                        ModifydNumber(ref mob.data.baseStats.VIT, mob, kv.Value); break;
                    case StatModTarget.STR:
                        ModifydNumber(ref mob.data.baseStats.STR, mob, kv.Value); break;
                    case StatModTarget.MAG:
                        ModifydNumber(ref mob.data.baseStats.MAG, mob, kv.Value); break;
                    case StatModTarget.INT:
                        ModifydNumber(ref mob.data.baseStats.INT, mob, kv.Value); break;
                    case StatModTarget.DEX:
                        ModifydNumber(ref mob.data.baseStats.DEX, mob, kv.Value); break;
                    case StatModTarget.TEC:
                        ModifydNumber(ref mob.data.baseStats.TEC, mob, kv.Value); break;
                }
            }
        }

        public void ModifyMoreStats(Mob mob, Dictionary<StatModTarget, dNumberModifier> modifiers)
        {
            foreach (var kv in modifiers)
            {
                switch (kv.Key)
                {
                    case StatModTarget.Defense:
                        ModifydNumber(ref mob.data.defense, mob, kv.Value); break;
                    case StatModTarget.SpDefense:
                        ModifydNumber(ref mob.data.spDefense, mob, kv.Value); break;

                    case StatModTarget.AggroMul:
                        ModifydNumber(ref mob.data.aggroMul, mob, kv.Value); break;

                    case StatModTarget.AttackPower:
                        ModifydNumber(ref mob.data.attackPower, mob, kv.Value); break;
                }
            }
        }
    }
}