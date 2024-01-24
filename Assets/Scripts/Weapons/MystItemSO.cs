using System.Collections;
using System.Collections.Generic;
using miniRAID.UIElements;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UIElements;

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

        private IEnumerator OnActionPostCast(MobData mob, RuntimeAction ract, Spells.SpellTarget target)
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
            yield break;
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
        
        public override void ShowInUI(EquipmentController ui)
        {
            base.ShowInUI(ui);

            if (RspecialAttack != null)
            {
                var skillInfo = ui.specialAttackTemplate.CloneTree();
                RspecialAttack.ShowInUI(skillInfo.Q("skillContainer"));
                
                skillInfo.Q<Label>("specialTitle").text = GetWeaponSpecialAttackTitle();
                
                string specialAttackTooltip = GetWeaponSpecialAttackTooltip();
                if (specialAttackTooltip != null)
                {
                    var specialLabel = skillInfo.Query("specialDescription").Children<Label>().First();
                    specialLabel.text = specialAttackTooltip;
                }
                
                ui.container.Add(skillInfo);
            }
        }
    }
}
