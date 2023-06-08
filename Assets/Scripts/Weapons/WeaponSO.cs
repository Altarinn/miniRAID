using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace miniRAID.Weapon
{
    [CreateAssetMenu(menuName = "Weapon/BaseWeaponData")]
    public class WeaponSO : StatModifierSO
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

        public WeaponType wpType;

        public ActionDataSO regularAttack;
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

            if (regularAttack != null)
            {
                (copied as WeaponSO).regularAttack = Instantiate(regularAttack);
            }

            return copied;
        }

        public override MobListener Wrap(MobData parent)
        {
            return new Weapon(parent, this);
        }
    }

    public class Weapon : StatModifier
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
    }
}
