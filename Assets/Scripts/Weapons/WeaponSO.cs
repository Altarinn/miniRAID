using System.Collections;
using System.Collections.Generic;
using miniRAID.Spells;
using miniRAID.UI.TargetRequester;
using Sirenix.OdinInspector;
using UnityEngine;

namespace miniRAID.Weapon
{
    [CreateAssetMenu(menuName = "Weapon/BaseWeaponData")]
    public class WeaponSO : EquipmentSO
    {
        public enum WeaponType
        {
            Sword,
            Spear,
            HeavyWeapon,
            Bow,
            Shield,

            Staff,
            Instrument,
            Grimoire,
            MagicItem,
            MystItem
        }

        [Title("Weapon", "", TitleAlignments.Centered)]
        public WeaponType wpType;

        public ActionSOEntry regularAttack;
        public Consts.Elements mainElement;

        public WeaponSO()
        {
            base.type = ListenerType.Weapon;
        }

        //public virtual void OnShowMobMenu(Mob mob, UI.UIState state, ref UI.UIMenu_UIContainer menu)
        //{
        //    // TODO: Modify UI based on weapon status
        //    menu.AddActionEntry(GetRegularAttackSpell(), mob, state, "Attack", false, OnPostAttack);
        //}

        //public virtual void OnPostAttack(Action spell, Mob mob) { }

        public override MobListenerSO Clone()
        {
            var copied = base.Clone();

            if (regularAttack.data != null)
            {
                (copied as WeaponSO).regularAttack = new ActionSOEntry() { data = Instantiate(regularAttack.data), level = regularAttack.level };
            }

            return copied;
        }

        public override MobListener Wrap(MobData parent)
        {
            return new Weapon(parent, this);
        }

        public override string GetType()
        {
            switch (wpType)
            {
                case WeaponType.Sword:
                    return "剑";
                case WeaponType.Spear:
                    return "矛";
                case WeaponType.HeavyWeapon:
                    return "重武器";
                case WeaponType.Bow:
                    return "弓";
                case WeaponType.Shield:
                    return "盾";
                case WeaponType.Staff:
                    return "法杖";
                case WeaponType.Instrument:
                    return "乐器";
                case WeaponType.Grimoire:
                    return "魔导书";
                case WeaponType.MagicItem:
                    return "魔法道具";
                case WeaponType.MystItem:
                    return "神秘道具";
                default:
                    return string.Empty;
            }
        }
    }

    public class Weapon : Equipment
    {
        public WeaponSO weaponData => (WeaponSO)data;
        public Weapon(MobData parent, WeaponSO data) : base(parent, data) { this.data = data; }
        protected RuntimeAction RregularAttack;

        public override void OnAttach(MobData mob)
        {
            base.OnAttach(mob);

            RregularAttack = mob.AddAction(weaponData.regularAttack);

            mob.OnQueryActions += OnQueryActions;
        }

        protected virtual void OnQueryActions(MobData mob, HashSet<RuntimeAction> actions)
        {
            //actions.RemoveWhere(x => x.data == data.hiddenAttack);
        }

        public virtual RuntimeAction GetRegularAttackSpell()
        {
            return RregularAttack;
        }

        public SpellTarget QueryTarget(MobData source)
        {
            RuntimeAction action = GetRegularAttackSpell();
            ActionTargetPickerBase picker = action?.data?.targetPicker;
            
            if (picker == null)
            {
                Debug.LogError($"{weaponData.name} : {action?.data.ActionName} has no target picker!");
                return null;
            }
            
            return picker.Pick(source, action);
        }
    }
}
