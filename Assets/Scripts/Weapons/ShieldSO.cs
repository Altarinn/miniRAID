using miniRAID.UIElements;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UIElements;

namespace miniRAID.Weapon
{
    [CreateAssetMenu(menuName = "Weapon/Shield")]
    public class ShieldSO : WeaponSO
    {
        public ShieldSO() : base()
        {
            wpType = WeaponType.Shield;
        }

        public ActionSOEntry defenseAction;

        public override MobListener Wrap(MobData parent)
        {
            return new Shield(parent, this);
        }
    }

    public class Shield : Weapon
    {
        public ShieldSO shieldData => (ShieldSO)data;
        
        public Shield(MobData parent, ShieldSO data) : base(parent, data) { }

        private RuntimeAction RDefenseAction;
        
        public override void OnAttach(MobData mob)
        {
            base.OnAttach(mob);
            RDefenseAction = mob.AddAction(shieldData.defenseAction);
        }

        public override string GetInformationString()
        {
            return $"{shieldData.defenseAction.data.ActionName}";
        }

        public override void ShowInUI(EquipmentController ui)
        {
            base.ShowInUI(ui);

            if (RDefenseAction != null)
            {
                var skillInfo = ui.specialAttackTemplate.CloneTree();
                RDefenseAction.ShowInUI(skillInfo.Q("skillContainer"));

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