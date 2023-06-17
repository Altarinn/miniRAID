using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace miniRAID.UIElements
{
    [RequireComponent(typeof(UIDocument))]
    public class CombatView : MonoBehaviour
    {
        public UnitMenuController menu;
        public BossStatsController bossStats;
        public CombatStatsController combatStats;
        public MessagePoolController messagePool;
        public MobDetailsController mobDetails;

        public Label battlePreview;

        private UIDocument uiDocument;
        [SerializeField] private Vector2Int referenceResolution;
        [SerializeField] private int minimumScale;

        private void OnEnable()
        {
            // The UXML is already instantiated by the UIDocument component
            uiDocument = GetComponent<UIDocument>();

            mobDetails = new MobDetailsController(
                uiDocument.rootVisualElement.Q("MobDetails")
            );

            VisualElement unitMenu = uiDocument.rootVisualElement.Q("UnitMenuContainer");
            menu = new UnitMenuController(unitMenu, mobDetails);
            
            // TODO: Multiple boss stats panels
            bossStats = new BossStatsController(
                uiDocument.rootVisualElement.Q("BossStats")
            );

            combatStats = new CombatStatsController(
                uiDocument.rootVisualElement.Q("CombatStats-global")
            );

            battlePreview = uiDocument.rootVisualElement.Q<Label>("BattlePreview");

            messagePool = new MessagePoolController(
                uiDocument.rootVisualElement.Q("MessagePanel")
            );
        }

        public void ShowBattlePreview(string text)
        {
            battlePreview.text = $"{text}";
            battlePreview.style.visibility = Visibility.Visible;
        }

        public void HideBattlePreview()
        {
            battlePreview.style.visibility = Visibility.Hidden;
        }

        public void BindAsBoss(MobRenderer mobRenderer)
        {
            bossStats.Register(mobRenderer);
        }

        public void Update()
        {
            bossStats.Update();
            UpdateSize();
        }

        private void UpdateSize()
        {
            int widthRatio = Mathf.FloorToInt(Screen.width / referenceResolution.x);
            int heightRatio = Mathf.FloorToInt(Screen.height / referenceResolution.y);

            uiDocument.panelSettings.scale = Mathf.Max(minimumScale, Mathf.Min(widthRatio, heightRatio));
        }
    }
}