using C5;
using miniRAID.Buff;
using miniRAID.Weapon;
using UnityEngine;
using UnityEngine.UIElements;

namespace miniRAID.UIElements
{
    public class EquipmentController
    {
        private VisualElement masterElem;
        public VisualTreeAsset regularAttackTemplate, specialAttackTemplate;

        public Label name, type, ilvl, rarity, requirements, stats, description;
        public VisualElement container, descriptionContainer;
        
        public EquipmentController(VisualElement elem)
        {
            masterElem = elem;
            
            name = elem.Q<Label>("name");
            type = elem.Q<Label>("type");
            ilvl = elem.Q<Label>("itemLevelText");
            rarity = elem.Q<Label>("rarity");
            requirements = elem.Q<Label>("requirementsText");
            stats = elem.Q<Label>("statsText");

            container = elem.Q("skillContainer");
            descriptionContainer = elem.Q("descriptionContainer");
            description = elem.Q<Label>("descriptionText");
            
            regularAttackTemplate = Resources.Load<VisualTreeAsset>("UI/EquipmentComponents/regularAttack");
            specialAttackTemplate = Resources.Load<VisualTreeAsset>("UI/EquipmentComponents/specialAttack");
        }

        public void RefreshWithContents(Equipment equip)
        {
            container.Clear();
            descriptionContainer.style.display = DisplayStyle.None;
            equip.ShowInUI(this);
        }

        public void Hide()
        {
            masterElem.visible = false;
        }
    }
    
    public class MobDetailsController
    {
        private VisualElement masterElem;
        private EquipmentController mainWeapon, subWeapon;
        private Label skills;
        
        public MobDetailsController(VisualElement elem)
        {
            masterElem = elem;
            
            mainWeapon = new EquipmentController(elem.Q("mainWeaponInfo"));
            subWeapon = new EquipmentController(elem.Q("subWeaponInfo"));
            
            // TODO: For test only.
            skills = elem.Q<Label>("Skills");
        }

        public void Show(MobData mob)
        {
            mainWeapon.RefreshWithContents(mob.mainWeapon);
            subWeapon.Hide();

            masterElem.visible = true;
        }

        public void Hide()
        {
            masterElem.visible = false;
        }
    }
}