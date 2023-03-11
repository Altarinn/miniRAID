using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine.UIElements;

namespace miniRAID.UIElements
{
    public class CombatStatsController
    {
        VisualElement masterElem;
        Label damageText, healText;

        public CombatStatsController(VisualElement e)
        {
            masterElem = e;

            damageText = e.Q<VisualElement>("Damage").Q<Label>("Content");
            healText = e.Q<VisualElement>("Healing").Q<Label>("Content");

            Globals.combatStats.SetUI(this);
        }

        public void UpdateInfo(string dmg, string heal)
        {
            damageText.text = dmg;
            healText.text = heal;
        }
    }
}
