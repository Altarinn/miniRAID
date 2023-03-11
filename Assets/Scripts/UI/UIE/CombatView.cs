using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace miniRAID.UIElements
{
    public class CombatView : MonoBehaviour
    {
        public UnitMenuController menu;
        public BossStatsController bossStats;
        public CombatStatsController combatStats;

        private void OnEnable()
        {
            // The UXML is already instantiated by the UIDocument component
            var uiDocument = GetComponent<UIDocument>();

            VisualElement unitMenu = uiDocument.rootVisualElement.Q("UnitMenuContainer");
            menu = new UnitMenuController(unitMenu);
            
            // TODO: Multiple boss stats panels
            bossStats = new BossStatsController(
                uiDocument.rootVisualElement.Q("BossStats")
            );

            combatStats = new CombatStatsController(
                uiDocument.rootVisualElement.Q("CombatStats-global")
            );
        }

        public void BindAsBoss(Mob mob)
        {
            bossStats.Register(mob);
        }

        public void Update()
        {
            bossStats.Update();
        }
    }
}