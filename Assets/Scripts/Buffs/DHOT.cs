//using System.Collections;
//using UnityEngine;

//namespace miniRAID.Buff
//{
//    [CreateAssetMenu(menuName = "Buffs/DHOT")]
//    public class DHOT : BuffSO
//    {
//        public Consts.Elements effectType = Consts.Elements.Fire;
//        public int effectValue = 10;

//        public float critChance = 0.0f;

//        public override void OnNextTurn(Mob mob)
//        {
//            base.OnNextTurn(mob);

//            mob.ReceiveDamage(new Consts.DamageHeal_FrontEndInput()
//            {
//                //source = source.parentMob,
//                source = mob,
//                //sourceSpell = Spells.Spell.Dummy(name),
//                //sourceSpellEnt = null,

//                value = effectValue,
//                type = effectType,

//                crit = critChance,
//                hit = 500.0f,

//                popup = true
//            });
//        }
//    }
//}