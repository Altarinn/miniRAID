using miniRAID.Buff;
using UnityEngine;

namespace miniRAID.Weapon
{
    [CreateAssetMenu(menuName = "Weapon/Instrument")]
    public class InstrumentSO : WeaponSO
    {
        public InstrumentSO() : base()
        {
            wpType = WeaponType.Instrument;
        }

        public BuffSO buff;

        public override MobListener Wrap(MobData parent)
        {
            return new Instrument(parent, this);
        }
    }

    public class Instrument : Weapon
    {
        public InstrumentSO instrumentData => (InstrumentSO)data;
        
        public Instrument(MobData parent, InstrumentSO data) : base(parent, data) { }

        public override void OnAttach(MobData mob)
        {
            base.OnAttach(mob);
            mob.AddBuff(instrumentData.buff, mob);
        }

        public override void OnRemove(MobData mob)
        {
            base.OnRemove(mob);
            mob.RemoveListener(instrumentData.buff);
        }

        public override string GetInformationString()
        {
            return instrumentData.buff.name;
        }
    }
}