//using miniRAID.Spells;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//namespace miniRAID.Weapon.HeavyWeapon
//{
//    public class _HeavyWeapon : OldWeapon
//    {
//        int chargeTimer = 0;
//        public int chargeTime = 1;

//        bool isCharging = false;

//        public Action ChargeAction;
//        public Spell ChargedAttack;

//        public override void OnAttach(Mob mob)
//        {
//            base.OnAttach(mob);
//            mob.OnWakeup += Mob_OnWakeup;
//            throw new System.NotImplementedException();
//            //mob.OnActionPostcast += Mob_OnActionPostcast;
//        }

//        private void Mob_OnActionPostcast(Mob mob, Action action, SpellTarget target)
//        {
//            if(action.Tags.Contains("charge"))
//            {
//                Charge(mob);
//            }

//            if(action == ChargedAttack)
//            {
//                Uncharge(mob);
//            }
//        }

//        private IEnumerator Mob_OnWakeup(Mob mob)
//        {
//            if(this.isCharging)
//            {
//                this.chargeTimer--;
//                if (chargeTimer < 0)
//                {
//                    isCharging = false;
//                    chargeTimer = 0;
//                }
//            }

//            yield break;
//        }

//        public void Charge(Mob mob)
//        {
//            isCharging = true;
//            chargeTimer = chargeTime;
//        }

//        public bool Uncharge(Mob mob)
//        {
//            isCharging = false;
//            return true;
//        }

//        protected override void OnQueryActions(Mob mob, ref HashSet<Action> actions)
//        {
//            base.OnQueryActions(mob, ref actions);

//            if(isCharging)
//            {
//                actions.RemoveWhere(action => action.Tags.Contains("Move"));
//                if(chargeTimer <= 0)
//                {
//                    actions.Add(ChargedAttack);
//                }
//            }
//            else
//            {
//                actions.Add(ChargeAction);
//            }
//        }

//        public override MobListenerSO Clone()
//        {
//            var copied = base.Clone() as _HeavyWeapon;

//            copied.ChargeAction = Instantiate(ChargeAction);
//            copied.ChargedAttack = Instantiate(ChargedAttack);

//            return copied;
//        }

//        public override string GetInformationString()
//        {
//            string info =
//                $"[Claymore]\n" +
//                $"{name}\n\n" +
//                $"Regular | {regularAttack.ActionName}\n" +
//                $"Charged | {ChargedAttack.ActionName}";

//            return info;
//        }
//    }
//}