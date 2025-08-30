using UnityEngine;
using UnityEngine.UIElements;

namespace miniRAID.UIElements
{
    public class UnitBarController
    {
        private VisualElement masterElem;

        public Label name, unitClass, level, weaponName, subweaponName, weaponDesc, defense, spDefense, dodge;
        public Label currentHP, maxHP, currentMP, maxMP, currentAPInt, currentAPFract, maxAP;
        public Label VIT, STR, MAG, INT, TEC, AGI;
        public Label tempBuffList;

        public VisualElement unitIcon, HPBar, MPBar;
        public VisualElement[] apStars;
        public VisualElement[] apStarFills;

        public UnitBarController(VisualElement elem)
        {
            masterElem = elem;

            name = elem.Q<Label>("UnitName");
            unitClass = elem.Q<Label>("UnitClass");
            level = elem.Q<Label>("Level");
            weaponName = elem.Q<Label>("CurrentWeapon");
            subweaponName = elem.Q<Label>("SubWeapon");
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
            HPBar = elem.Q<VisualElement>("HPBarContent");

            currentMP = elem.Q<Label>("currMP");
            maxMP = elem.Q<Label>("maxMP");
            MPBar = elem.Q<VisualElement>("MPBarContent");
            
            currentAPInt = elem.Q<Label>("currAPInt");
            currentAPFract = elem.Q<Label>("currAPFract");
            maxAP = elem.Q<Label>("maxAPInt");

            // Initialize AP stars and fills
            apStars = new VisualElement[6];
            apStarFills = new VisualElement[6];
            for (int i = 0; i < 6; i++)
            {
                apStars[i] = elem.Q<VisualElement>($"APStar{i + 1}");
                apStarFills[i] = elem.Q<VisualElement>($"APStar{i + 1}Fill");
            }

            tempBuffList = elem.Q<Label>("TempBuffInfo");

            unitIcon = elem.Q("UnitIcon");
        }

        public void RefreshWithContents(MobData mob)
        {
            name.text = mob.nickname;
            unitClass.text = $"{mob.baseDescriptor.race?.raceName} / {mob.baseDescriptor.job?.className}";
            level.text = mob.level.ToString();
            
            weaponName.text = $"{mob.mainWeapon?.name} [E]";
            subweaponName.text = $"{mob.subWeapon?.name ?? "-"}";
            weaponDesc.text = mob.mainWeapon?.GetInformationString();

            defense.text = $"{100 - Mathf.RoundToInt(Consts.GetDefenseRate((float)mob.defense.Value, mob.level) * 100)}%";
            spDefense.text = $"{100 - Mathf.RoundToInt(Consts.GetDefenseRate((float)mob.spDefense.Value, mob.level) * 100)}%";
            dodge.text = $"{Mathf.RoundToInt((float)mob.dodge.Value)}";

            currentHP.text = $"{mob.health}";
            maxHP.text = $"{mob.maxHealth}";
            HPBar.style.width = Length.Percent(100.0f * mob.health / (float)(mob.maxHealth == 0 ? 1 : mob.maxHealth));
            
            currentAPInt.text = $"{Mathf.FloorToInt(mob.actionPoints)}";
            currentAPFract.text = $".{(int)(100 * (mob.actionPoints - Mathf.FloorToInt(mob.actionPoints))):#00}";
            maxAP.text = $"{mob.apNonFreeMax + Consts.freeAP}";

            // Update AP stars visual representation
            UpdateAPStars(mob);

            VIT.text = $"{mob.VIT}";
            STR.text = $"{mob.STR}";
            MAG.text = $"{mob.MAG}";
            INT.text = $"{mob.INT}";
            TEC.text = $"{mob.TEC}";
            AGI.text = $"{mob.AGI}";
            
            unitIcon.style.backgroundImage = new StyleBackground(mob.mobRenderer?.GetComponentInChildren<BillboardSpriteRenderer>().Sprite);
            
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
            
            // General Resources
            // Mana
            var manaListener = mob.FindListener<GeneralManaListener>();
            if (manaListener == null)
            {
                currentMP.text = "0";
                maxMP.text = "0";
                MPBar.style.width = Length.Percent(0);
            }
            else
            {
                currentMP.text = $"{manaListener.current}";
                maxMP.text = $"{manaListener.max}";
                MPBar.style.width = Length.Percent(100.0f * manaListener.current / (float)(manaListener.max == 0 ? 1 : manaListener.max));
            }
        }

        private void UpdateAPStars(MobData mob)
        {
            if (apStars == null || apStarFills == null) return;

            float currentAP = mob.actionPoints;
            float currentFreeAP = mob.freeActionPoints;
            int maxNonFreeAP = mob.apNonFreeMax;
            int maxFreeAP = Consts.freeAP;
            
            // Calculate non-free AP (total AP minus free AP)
            float currentNonFreeAP = currentAP - currentFreeAP;
            
            // Update non-free AP stars (stars 1-5, indices 0-4)
            for (int i = 0; i < maxNonFreeAP && i < apStars.Length - 1; i++)
            {
                if (apStars[i] != null && apStarFills[i] != null)
                {
                    // Show the star
                    apStars[i].style.visibility = Visibility.Visible;
                    
                    float starProgress = Mathf.Clamp01(currentNonFreeAP - i);
                    if (starProgress > 0)
                    {
                        // Fill from bottom to top based on progress
                        apStarFills[i].style.height = Length.Percent(starProgress * 100);
                    }
                    else
                    {
                        // Empty star
                        apStarFills[i].style.height = Length.Percent(0);
                    }
                }
            }
            
            // Update free AP star (star 6, index 5) - reflect actual free AP status
            if (apStars.Length > 5 && apStars[5] != null && apStarFills.Length > 5 && apStarFills[5] != null)
            {
                apStars[5].style.visibility = Visibility.Visible;
                
                // Free AP star shows current free AP vs max free AP
                float freeAPProgress = currentFreeAP / maxFreeAP;
                apStarFills[5].style.height = Length.Percent(freeAPProgress * 100);
            }
            
            // Hide unused non-free AP stars if maxNonFreeAP < 5
            for (int i = maxNonFreeAP; i < apStars.Length - 1; i++)
            {
                if (apStars[i] != null)
                {
                    apStars[i].style.visibility = Visibility.Hidden;
                }
            }
        }
    }
}