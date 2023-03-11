using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace miniRAID.Weapon
{
    [CreateAssetMenu(menuName = "Weapon/Bow")]
    public class BowSO : WeaponSO
    {
        public BowSO() : base()
        {
            wpType = WeaponType.Bow;
        }

        public ActionDataSO aimedAttack;

        public override MobListener Wrap(MobData parent)
        {
            return new Bow(parent, this);
        }
    }

    public class Bow : Weapon
    {
        public new BowSO data;

        bool moved = false;

        public Bow(MobData parent, BowSO data) : base(parent, data) { this.data = data; }
        RuntimeAction RaimedAttack;

        public override void OnAttach(Mob mob)
        {
            base.OnAttach(mob);

            RaimedAttack = mob.AddAction(data.aimedAttack);

            mob.OnNextTurn += OnNextTurn;
            mob.OnActionPostcast += OnActionPostCast;
        }

        private void OnNextTurn(Mob mob)
        {
            moved = false;
        }

        private void OnActionPostCast(Mob mob, RuntimeAction ract, Spells.SpellTarget target)
        {
            if(ract.data.Id == 1) // Movement
            {
                moved = true;
            }
        }

        protected override void OnQueryActions(Mob mob, HashSet<RuntimeAction> actions)
        {
            base.OnQueryActions(mob, actions);
            if(moved == true)
            {
                actions.RemoveWhere(x => x.data == data.aimedAttack);
            }
        }

        public override string GetInformationString()
        {
            if(moved == true)
            {
                return $"{base.GetInformationString()}    瞄准：否";
            }
            return $"{base.GetInformationString()}    瞄准：是";
        }
    }
}
