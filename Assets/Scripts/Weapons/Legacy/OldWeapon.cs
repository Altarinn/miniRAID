//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//namespace miniRAID.Weapon
//{
//    public class OldWeapon : MobListenerSO
//    {
//        public enum WeaponType
//        {
//            Sword,
//            Spear,
//            HeavyWeapon,
//            Bow,
//            Shield,

//            Staff,
//            Instrument,
//            Grimoire,
//            MagicItem,
//            MystItem
//        }

//        public Spells.Spell regularAttack;

//        public WeaponType wpType;

//        public Consts.Elements mainElement;

//        public OldWeapon()
//        {
//            base.type = ListenerType.Weapon;
//        }

//        public override void OnAttach(Mob mob)
//        {
//            base.OnAttach(mob);

//            throw new System.NotImplementedException();
//            //mob.OnShowMobMenu += OnShowMobMenu;
//            //mob.OnQueryActions += OnQueryActions;
//        }

//        protected virtual void OnQueryActions(Mob mob, ref HashSet<Action> actions)
//        {
//            throw new System.NotImplementedException();
//            //mob.AddAction(GetRegularAttackSpell());
//        }

//        //public virtual void OnShowMobMenu(Mob mob, UI.UIState state, ref UI.UIMenu_UIContainer menu)
//        //{
//        //    // TODO: Modify UI based on weapon status
//        //    menu.AddActionEntry(GetRegularAttackSpell(), mob, state, "Attack", false, OnPostAttack);
//        //}

//        public virtual Spells.Spell GetRegularAttackSpell()
//        {
//            return regularAttack;
//        }

//        //public virtual void OnPostAttack(Action spell, Mob mob) { }

//        public override MobListenerSO Clone()
//        {
//            var copied = base.Clone();

//            if(regularAttack != null)
//            {
//                (copied as OldWeapon).regularAttack = Instantiate(regularAttack);
//            }

//            return copied;
//        }
//    }
//}
