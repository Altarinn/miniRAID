using UnityEngine;
using UnityEngine.UIElements;

namespace miniRAID.UIElements
{
    public class UnitBarController
    {
        private VisualElement masterElem;

        public Label name, level, weaponName, weaponDesc, defense, spDefense, dodge;
        public Label currentHP, maxHP, currentAPInt, currentAPFract, maxAP;
        public Label VIT, STR, MAG, INT, TEC, AGI;
        public Label tempBuffList;

        public VisualElement unitIcon;

        public UnitBarController(VisualElement elem)
        {
            masterElem = elem;

            name = elem.Q<Label>("UnitName");
            level = elem.Q<Label>("Level");
            weaponName = elem.Q<Label>("CurrentWeapon");
            weaponDesc = elem.Q<Label>("WeaponStats");
            defense = elem.Q<Label>("Defense");
            spDefense = elem.Q<Label>("MDefense");
            dodge = elem.Q<Label>("Avoid");
            VIT = elem.Q<Label>("VIT");
            STR = elem.Q<Label>("STR");
            MAG = elem.Q<Label>("MAG");
            INT = elem.Q<Label>("INT");
            TEC = elem.Q<Label>("TEC");
            AGI = elem.Q<Label>("AGI");

            currentHP = elem.Q<Label>("currHP");
            maxHP = elem.Q<Label>("maxHP");
            currentAPInt = elem.Q<Label>("currAPInt");
            currentAPFract = elem.Q<Label>("currAPFract");
            maxAP = elem.Q<Label>("maxAPInt");

            tempBuffList = elem.Q<Label>("TempBuffInfo");

            unitIcon = elem.Q("UnitIcon");
        }

        public void RefreshWithContents(MobData mob)
        {
            name.text = mob.nickname;
            level.text = mob.level.ToString();
            
            weaponName.text = $"{mob.mainWeapon?.name} [E]";
            weaponDesc.text = mob.mainWeapon?.GetInformationString();

            defense.text = $"{100 - Mathf.RoundToInt(Consts.GetDefenseRate((float)mob.defense.Value, mob.level) * 100)}%";
            spDefense.text = $"{100 - Mathf.RoundToInt(Consts.GetDefenseRate((float)mob.spDefense.Value, mob.level) * 100)}%";
            dodge.text = $"{Mathf.RoundToInt((float)mob.dodge.Value)}";

            currentHP.text = $"{mob.health}";
            maxHP.text = $"{mob.maxHealth}";
            currentAPInt.text = $"{Mathf.FloorToInt(mob.actionPoints)}";
            currentAPFract.text = $".{(int)(100 * (mob.actionPoints - Mathf.FloorToInt(mob.actionPoints)))}";
            maxAP.text = $"{mob.apMax}";

            VIT.text = $"{mob.VIT}";
            STR.text = $"{mob.STR}";
            MAG.text = $"{mob.MAG}";
            INT.text = $"{mob.INT}";
            TEC.text = $"{mob.TEC}";
            AGI.text = $"{mob.DEX}";
            
            unitIcon.style.backgroundImage = new StyleBackground(mob.mobRenderer?.GetComponentInChildren<SpriteRenderer>().sprite);
            
            string effects = "";
            foreach (var fx in mob.listeners)
            {
                if (
                    fx.type is MobListenerSO.ListenerType.Buff or MobListenerSO.ListenerType.Passive)
                {
                    effects += "\n" + ((Buff.Buff)fx).detailedName;
                }
            }

            tempBuffList.text = effects.TrimStart('\n');
        }
    }
}