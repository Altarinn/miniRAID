using System.Collections;
using miniRAID.Spells;
using UnityEngine;

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
        
        public override void OnAttach(MobData mob)
        {
            base.OnAttach(mob);
            mob.AddAction(heavyData.chargedAttack.ToBase());
        }

        public override string GetInformationString()
        {
            return $"{heavyData.chargedAttack.data.ActionName}";
        }
    }
}