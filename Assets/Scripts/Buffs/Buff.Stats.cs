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
            public LuaGetter<MobData, float> value;
        }

        public void ModifydNumber(ref dNumber num, MobData mob, dNumberModifier mod, int stacks)
        {
            for (int i = 0; i < stacks; i++)
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
        }

        public void ModifyBaseStats(MobData mob, Dictionary<StatModTarget, dNumberModifier> modifiers, int stacks)
        {
            foreach (var kv in modifiers)
            {
                switch(kv.Key)
                {
                    case StatModTarget.VIT: 
                        ModifydNumber(ref mob.baseStats.VIT, mob, kv.Value, stacks); break;
                    case StatModTarget.STR:
                        ModifydNumber(ref mob.baseStats.STR, mob, kv.Value, stacks); break;
                    case StatModTarget.MAG:
                        ModifydNumber(ref mob.baseStats.MAG, mob, kv.Value, stacks); break;
                    case StatModTarget.INT:
                        ModifydNumber(ref mob.baseStats.INT, mob, kv.Value, stacks); break;
                    case StatModTarget.DEX:
                        ModifydNumber(ref mob.baseStats.DEX, mob, kv.Value, stacks); break;
                    case StatModTarget.TEC:
                        ModifydNumber(ref mob.baseStats.TEC, mob, kv.Value, stacks); break;
                }
            }
        }

        public void ModifyMoreStats(MobData mob, Dictionary<StatModTarget, dNumberModifier> modifiers, int stacks)
        {
            foreach (var kv in modifiers)
            {
                switch (kv.Key)
                {
                    case StatModTarget.Defense:
                        ModifydNumber(ref mob.defense, mob, kv.Value, stacks); break;
                    case StatModTarget.SpDefense:
                        ModifydNumber(ref mob.spDefense, mob, kv.Value, stacks); break;

                    case StatModTarget.AggroMul:
                        ModifydNumber(ref mob.aggroMul, mob, kv.Value, stacks); break;

                    case StatModTarget.AttackPower:
                        ModifydNumber(ref mob.attackPower, mob, kv.Value, stacks); break;
                }
            }
        }
    }
}