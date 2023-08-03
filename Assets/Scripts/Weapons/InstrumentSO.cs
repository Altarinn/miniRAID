using miniRAID.Buff;
using miniRAID.UIElements;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UIElements;

namespace miniRAID.Weapon
{
    [CreateAssetMenu(menuName = "Weapon/Instrument")]
    public class InstrumentSO : WeaponSO
    {
        public InstrumentSO() : base()
        {
            wpType = WeaponType.Instrument;
        }

        public BuffSOEntry buff;

        public override MobListener Wrap(MobData parent)
        {
            return new Instrument(parent, this);
        }
    }

    public class Instrument : Weapon
    {
        public InstrumentSO instrumentData => (InstrumentSO)data;
        
        public Instrument(MobData parent, InstrumentSO data) : base(parent, data) { }

        private Buff.Buff RBuff;

        public override void OnAttach(MobData mob)
        {
            base.OnAttach(mob);
            RBuff = mob.AddBuff(instrumentData.buff.data, instrumentData.buff.level, mob);
        }

        public override void OnRemove(MobData mob)
        {
            base.OnRemove(mob);
            mob.RemoveListener(instrumentData.buff.data);
            RBuff = null;
        }

        public override string GetInformationString()
        {
            return instrumentData.buff.data.name;
        }
        
        public override void ShowInUI(EquipmentController ui)
        {
            base.ShowInUI(ui);

            if (instrumentData.buff.data != null)
            {
                var skillInfo = ui.specialAttackTemplate.CloneTree();
                skillInfo.Q<Label>("name").text = instrumentData.buff.data.name;
                skillInfo.Q<Label>("costs").text = "";
                skillInfo.Q<Label>("tooltip").text = RBuff.Tooltip;
                
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