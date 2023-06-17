using System;
using System.Collections.Generic;
using System.Linq;
using miniRAID.UIElements;
using Sirenix.OdinInspector;
using UnityEngine;

namespace miniRAID.Weapon
{
    public class EquipmentSO : StatModifierSO
    {
        public enum EquipmentType
        {
            Weapon,
            Armor,
            Accessory,
        }

        [Title("Standard Equipments", "MobListener", TitleAlignments.Centered)]
        [InfoBox("Power and auxPower should be ignored as they are not being calculated from itemLevel.")]
        [InfoBox("In Stat Mods, try keep every Power% = 1 to reflect itemLevel.\nDouble-handed weapons should have Power% = 2 etc.")]
        public int itemLevel;
        
        [Space(10.0f)]
        [InfoBox("Base stats requirement should be a growthRate.\nReal stats requirement will be computed via levelRequirement.")]
        public int levelRequirement;
        public Consts.BaseStatsGrowth baseStatsRequirements;

        public override MobListener Wrap(MobData parent)
        {
            return new Equipment(parent, this);
        }

        public virtual string GetType()
        {
            return "装备";
        }
    }

    public class Equipment : StatModifier
    {
        private EquipmentSO eqData => (EquipmentSO)data;

        public dNumber itemLevel;
        private int cachedItemLevel;

        private Dictionary<StatModifierSO.StatModTarget, StatModifierSO.dNumberModifier> derivedModifiers;
        private Consts.BaseStats derivedBaseStatsRequirements;
        
        public Equipment(MobData parent, MobListenerSO data) : base(parent, data) { }

        public override void OnAttach(MobData mob)
        {
            RefreshDerivedStats(mob);
            base.OnAttach(mob);

            power = dNumber.CreateStatic(1);
            auxPower = dNumber.CreateStatic(1);
            stacks = 1;
        }

        private void RefreshDerivedStats(MobData mob)
        {
            derivedBaseStatsRequirements = new Consts.BaseStats();
            derivedBaseStatsRequirements.VIT = dNumber.CreateStatic(
                eqData.baseStatsRequirements.VIT > 0 ? Consts.BaseStatsFromLevel(mob.level, eqData.baseStatsRequirements.VIT) : 0);
            derivedBaseStatsRequirements.STR = dNumber.CreateStatic(
                eqData.baseStatsRequirements.STR > 0 ? Consts.BaseStatsFromLevel(mob.level, eqData.baseStatsRequirements.STR) : 0);
            derivedBaseStatsRequirements.MAG = dNumber.CreateStatic(
                eqData.baseStatsRequirements.MAG > 0 ? Consts.BaseStatsFromLevel(mob.level, eqData.baseStatsRequirements.MAG) : 0);
            derivedBaseStatsRequirements.INT = dNumber.CreateStatic(
                eqData.baseStatsRequirements.INT > 0 ? Consts.BaseStatsFromLevel(mob.level, eqData.baseStatsRequirements.INT) : 0);
            derivedBaseStatsRequirements.TEC = dNumber.CreateStatic(
                eqData.baseStatsRequirements.TEC > 0 ? Consts.BaseStatsFromLevel(mob.level, eqData.baseStatsRequirements.TEC) : 0);
            derivedBaseStatsRequirements.DEX = dNumber.CreateStatic(
                eqData.baseStatsRequirements.DEX > 0 ? Consts.BaseStatsFromLevel(mob.level, eqData.baseStatsRequirements.DEX) : 0);

            // TODO: Handle the case when itemLevel changes during combat
            itemLevel = dNumber.CreateComposite(eqData.itemLevel, "eqbase");
            cachedItemLevel = itemLevel;
            
            derivedModifiers = eqData.modifiers.ToDictionary(
                entry => entry.Key,
                entry => CopyModifierEntry(mob, entry)
            );
        }

        private StatModifierSO.dNumberModifier CopyModifierEntry(
            MobData mob, 
            KeyValuePair<StatModifierSO.StatModTarget, StatModifierSO.dNumberModifier> entry)
        {
            switch (entry.Value.type)
            {
                case StatModifierSO.dNumberModType.Add:
                    return new StatModifierSO.dNumberModifier()
                    {
                        aux = false,
                        type = StatModifierSO.dNumberModType.Add,
                        value = new LuaGetter<MobData, float>(
                            Consts.ValueFromItemLevel(cachedItemLevel, entry.Key, entry.Value.value.Eval(mob)))
                    };
                    break;
                case StatModifierSO.dNumberModType.Mul:
                case StatModifierSO.dNumberModType.MulMul:
                    throw new NotImplementedException();
                    break;
            }
            
            throw new NotImplementedException();
        }

        protected override void RecalculateStats(MobData mob)
        {
            // TODO: Handle the case when itemLevel changes during combat
            RefreshDerivedStats(mob);
        }

        protected override void ModifyBaseStats(MobData m)
        {
            // TODO: Handle the case when itemLevel changes during combat
            RecalculateStats(m);
            eqData.ModifyBaseStats(this, m, derivedModifiers, stacks);
        }

        protected override void ModifyMoreStats(MobData m)
        {
            // TODO: Handle the case when itemLevel changes during combat
            RecalculateStats(m);
            eqData.ModifyMoreStats(this, m, derivedModifiers, stacks);
        }

        public virtual void ShowInUI(EquipmentController ui)
        {
            ui.name.text = name;
            ui.type.text = eqData.GetType();
            ui.ilvl.text = $"{cachedItemLevel}";
            ui.rarity.text = "-";

            string req = "";
            if (eqData.levelRequirement > 0)
                req += $"等级 {eqData.levelRequirement}\n";
            if (derivedBaseStatsRequirements.VIT > 0)
                req += $"体格 {derivedBaseStatsRequirements.VIT}\n";
            if (derivedBaseStatsRequirements.STR > 0)
                req += $"力量 {derivedBaseStatsRequirements.STR}\n";
            if (derivedBaseStatsRequirements.MAG > 0)
                req += $"魔力 {derivedBaseStatsRequirements.MAG}\n";
            if (derivedBaseStatsRequirements.INT > 0)
                req += $"智力 {derivedBaseStatsRequirements.INT}\n";
            if (derivedBaseStatsRequirements.TEC > 0)
                req += $"技巧 {derivedBaseStatsRequirements.TEC}\n";
            if (derivedBaseStatsRequirements.DEX > 0)
                req += $"敏捷 {derivedBaseStatsRequirements.DEX}\n";

            ui.requirements.text = req.TrimEnd('\n');

            string stats = "";
            if (derivedModifiers.ContainsKey(StatModifierSO.StatModTarget.AttackPower))
                stats += $"+{Mathf.CeilToInt(derivedModifiers[StatModifierSO.StatModTarget.AttackPower].value.staticOut)} 攻击强度\n";

            if (derivedModifiers.ContainsKey(StatModifierSO.StatModTarget.SpellPower))
                stats += $"+{Mathf.CeilToInt(derivedModifiers[StatModifierSO.StatModTarget.SpellPower].value.staticOut)} 法术强度\n";

            if (derivedModifiers.ContainsKey(StatModifierSO.StatModTarget.HealPower))
                stats += $"+{Mathf.CeilToInt(derivedModifiers[StatModifierSO.StatModTarget.HealPower].value.staticOut)} 治疗强度\n";

            if (derivedModifiers.ContainsKey(StatModifierSO.StatModTarget.BuffPower))
                stats += $"+{Mathf.CeilToInt(derivedModifiers[StatModifierSO.StatModTarget.BuffPower].value.staticOut)} 增益强度\n";

            if (derivedModifiers.ContainsKey(StatModifierSO.StatModTarget.Defense))
                stats += $"+{Mathf.CeilToInt(derivedModifiers[StatModifierSO.StatModTarget.Defense].value.staticOut)} 护甲\n";

            if (derivedModifiers.ContainsKey(StatModifierSO.StatModTarget.SpDefense))
                stats += $"+{Mathf.CeilToInt(derivedModifiers[StatModifierSO.StatModTarget.SpDefense].value.staticOut)} 魔法抗性\n";

            ui.stats.text = stats.TrimEnd('\n');

        }
    }
}