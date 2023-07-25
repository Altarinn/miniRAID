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

        public ActionSOEntry specialAttack;
        public int energyCount, energyMax;

        public override MobListener Wrap(MobData parent)
        {
            return new MystItem(parent, this);
        }
    }

    public class MystItem : Weapon
    {
        public MystItemSO mystData => (MystItemSO)data;

        bool moved = false;
        int currentEnergy = 0;

        public MystItem(MobData parent, MystItemSO data) : base(parent, data) { this.data = data; }
        RuntimeAction RspecialAttack;

        public override void OnAttach(MobData mob)
        {
            base.OnAttach(mob);

            RspecialAttack = mob.AddAction(mystData.specialAttack);

            mob.OnActionPostcast += OnActionPostCast;

            currentEnergy = 0;
        }

        public override void OnRemove(MobData mob)
        {
            mob.OnActionPostcast -= OnActionPostCast;
            
            base.OnRemove(mob);
        }

        private void OnActionPostCast(MobData mob, RuntimeAction ract, Spells.SpellTarget target)
        {
            if (ract.data == RregularAttack.data) // Movement
            {
                currentEnergy += 1;
                currentEnergy = Mathf.Min(mystData.energyMax, currentEnergy);
            }
            else if(ract.data == RspecialAttack.data)
            {
                currentEnergy -= mystData.energyCount;
                currentEnergy = Mathf.Max(0, currentEnergy);
            }
        }

        protected override void OnQueryActions(MobData mob, HashSet<RuntimeAction> actions)
        {
            base.OnQueryActions(mob, actions);
            if (currentEnergy < mystData.energyCount)
            {
                actions.RemoveWhere(x => x.data == mystData.specialAttack.data);
            }
        }

        public override string GetInformationString()
        {
            return $"能量：{currentEnergy} / {mystData.energyCount}";
        }
    }
}
