using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace miniRAID
{
    public class StatModifierSO : MobListenerSO
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
        
        [TabGroup("Stat Mods")]
        [PropertyOrder(99)]
        public Dictionary<StatModTarget, dNumberModifier> modifiers = new();

        public override MobListener Wrap(MobData parent)
        {
            return new StatModifier(parent, this);
        }
    }

    public class StatModifier : MobListener
    {
        private StatModifierSO statData => (StatModifierSO)data;
        public int stacks = 1;
        
        public StatModifier(MobData parent, MobListenerSO data) : base(parent, data)
        {
            stacks = 1;
        }

        protected void ModifyBaseStats(MobData m)
        {
            statData.ModifyBaseStats(m, statData.modifiers, stacks);
        }
        
        protected void ModifyMoreStats(MobData m)
        {
            statData.ModifyMoreStats(m, statData.modifiers, stacks);
        }

        public override void OnAttach(MobData mob)
        {
            base.OnAttach(mob);
            
            mob.OnBaseStatCalculation += ModifyBaseStats;
            mob.OnStatCalculation += ModifyMoreStats;
        }

        public override void OnRemove(MobData mob)
        {
            base.OnRemove(mob);

            mob.OnBaseStatCalculation -= ModifyBaseStats;
            mob.OnStatCalculation -= ModifyMoreStats;
        }
    }
}