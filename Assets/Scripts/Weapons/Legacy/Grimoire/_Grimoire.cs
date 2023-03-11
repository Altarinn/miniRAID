//using miniRAID.Spells;
//using miniRAID.UI;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//namespace miniRAID.Weapon.Grimoire
//{
//    public class _Grimoire : OldWeapon
//    {
//        public List<Spells.Spell> contents;
//        public int currentIdx;

//        [SerializeField] bool nameModified = false;

//        string[] RomaianIdxs = new string[7]
//        {
//            "  I", " II", "III", " IV", "  V", " VI", "VII"
//        };

//        public _Grimoire()
//        {
//            contents = new List<Spells.Spell>();
//            wpType = WeaponType.Grimoire;
//        }

//        protected virtual string GrimoireSpellName(int idx)
//        {
//            return $"{RomaianIdxs[idx]}. {contents[idx].ActionName}";
//        }

//        //public override void OnShowMobMenu(Mob mob, UIState state, ref UIMenu_UIContainer menu)
//        //{
//        //    menu.AddActionEntry(contents[0], mob, state, GrimoireSpellName(0), false, OnPostAttack);
//        //    menu.AddActionEntry(GetRegularAttackSpell(), mob, state, GrimoireSpellName(currentIdx), false, OnPostAttack);
//        //}

//        public override void OnAttach(Mob mob)
//        {
//            base.OnAttach(mob);

//            if(!nameModified)
//            {
//                nameModified = true;
//                for (int i = 0; i < contents.Count; i++)
//                {
//                    contents[i].ActionName = GrimoireSpellName(i);
//                }
//            }

//            throw new System.NotImplementedException();
//            //mob.OnActionPostcast += Mob_OnActionPostcast;
//        }

//        /// <summary>
//        /// Check if spell matches current spell, and add currentIdx as Grimoire progress increased by 1.
//        /// </summary>
//        /// <param name="action"></param>
//        /// <param name="mob"></param>
//        private void Mob_OnActionPostcast(Mob mob, Action action, SpellTarget target)
//        {
//            if (action == contents[currentIdx])
//            {
//                IncreaseProgress();
//            }
//        }

//        protected override void OnQueryActions(Mob mob, ref HashSet<Action> actions)
//        {
//            throw new System.NotImplementedException();
//            //mob.AddAction(contents[0]);
//            //mob.AddAction(GetRegularAttackSpell());
//        }

//        public override Spell GetRegularAttackSpell()
//        {
//            if(contents.Count <= 0)
//            {
//                Debug.LogError($"Grimoire {this.name} has no contents for RegularAttack()");
//                return null;
//            }

//            return contents[currentIdx];
//        }

//        public virtual void IncreaseProgress()
//        {
//            currentIdx++;
//            currentIdx %= contents.Count;
//        }

//        public override MobListenerSO Clone()
//        {
//            var copied = base.Clone() as _Grimoire;

//            copied.contents = new List<Spells.Spell>(contents.Count);
//            contents.ForEach((item) =>
//            {
//                copied.contents.Add(Instantiate(item as Spells.Spell));
//            });

//            return copied;
//        }

//        public override string GetInformationString()
//        {
//            string info =
//                $"[Grimoire]\n"+
//                $"{name}\n";

//            for (int i = 0; i < contents.Count; i++)
//            {
//                info +=
//                    $"\n[{(currentIdx == i ? "X" : " ")}] {contents[i].ActionName}";
//            }

//            return info;
//        }
//    }
//}