using UnityEngine;

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
        
        public override void OnAttach(MobData mob)
        {
            base.OnAttach(mob);
            mob.AddAction(shieldData.defenseAction);
        }

        public override string GetInformationString()
        {
            return $"{shieldData.defenseAction.data.ActionName}";
        }
    }
}