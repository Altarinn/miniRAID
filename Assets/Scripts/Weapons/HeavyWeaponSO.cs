using System.Collections;
using miniRAID.Spells;
using miniRAID.UIElements;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UIElements;

namespace miniRAID.Weapon
{
    [CreateAssetMenu(menuName = "Weapon/HeavyWeapon")]
    public class HeavyWeaponSO : WeaponSO
    {
        public HeavyWeaponSO() : base()
        {
            wpType = WeaponType.HeavyWeapon;
        }

        public ActionSOEntry<ChargedActionSO> chargedAttack;

        public override MobListener Wrap(MobData parent)
        {
            return new HeavyWeapon(parent, this);
        }
    }

    public class HeavyWeapon : Weapon
    {
        public HeavyWeaponSO heavyData => (HeavyWeaponSO)data;
        
        public HeavyWeapon(MobData parent, HeavyWeaponSO data) : base(parent, data) { }

        private bool isCharging = false;
        int chargeTimer = 0;

        public ChargedAction RchargedAttack;
        
        public override void OnAttach(MobData mob)
        {
            base.OnAttach(mob);
            RchargedAttack = mob.AddAction(heavyData.chargedAttack.ToBase()) as ChargedAction;
        }

        public override string GetInformationString()
        {
            return $"{heavyData.chargedAttack.data.ActionName}";
        }
        
        public override void ShowInUI(EquipmentController ui)
        {
            base.ShowInUI(ui);

            if (RchargedAttack != null)
            {
                var skillInfo = ui.specialAttackTemplate.CloneTree();
                RchargedAttack.ShowInUI(skillInfo.Q("skillContainer"));
                
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