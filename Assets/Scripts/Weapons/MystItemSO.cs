using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace miniRAID.Weapon
{
    [CreateAssetMenu(menuName = "Weapon/MystItem")]
    public class MystItemSO : WeaponSO
    {
        public MystItemSO() : base()
        {
            wpType = WeaponType.MystItem;
        }

        public ActionDataSO specialAttack;
        public int energyCount, energyMax;

        public override MobListener Wrap(MobData parent)
        {
            return new MystItem(parent, this);
        }
    }

    public class MystItem : Weapon
    {
        public new MystItemSO data;

        bool moved = false;
        int currentEnergy = 0;

        public MystItem(MobData parent, MystItemSO data) : base(parent, data) { this.data = data; }
        RuntimeAction RspecialAttack;

        public override void OnAttach(MobData mob)
        {
            base.OnAttach(mob);

            RspecialAttack = mob.AddAction(data.specialAttack);

            mob.OnActionPostcast += OnActionPostCast;

            currentEnergy = 0;
        }

        private void OnActionPostCast(MobData mob, RuntimeAction ract, Spells.SpellTarget target)
        {
            if (ract.data == RregularAttack.data) // Movement
            {
                currentEnergy += 1;
                currentEnergy = Mathf.Min(data.energyMax, currentEnergy);
            }
            else if(ract.data == RspecialAttack.data)
            {
                currentEnergy -= data.energyCount;
                currentEnergy = Mathf.Max(0, currentEnergy);
            }
        }

        protected override void OnQueryActions(MobData mob, HashSet<RuntimeAction> actions)
        {
            base.OnQueryActions(mob, actions);
            if (currentEnergy < data.energyCount)
            {
                actions.RemoveWhere(x => x.data == data.specialAttack);
            }
        }

        public override string GetInformationString()
        {
            return $"{base.GetInformationString()}    能量：{currentEnergy} / {data.energyCount}";
        }
    }
}
