using UnityEngine;
using System.Collections;
using miniRAID.UI;
using miniRAID.UIElements;

namespace miniRAID
{
    public static class Globals
    {
        public static MonoSingleton<UI.GridUI> ui = new MonoSingleton<UI.GridUI>(false);
        public static MonoSingleton<Scheduler> scheduler = new MonoSingleton<Scheduler>(false);
        public static MonoSingleton<CombatSchedulerCoroutine> combatMgr = new MonoSingleton<CombatSchedulerCoroutine>(false);
        public static MonoSingleton<SerialCoroutine> serialCoroutine = new MonoSingleton<SerialCoroutine>(false);
        public static MonoSingleton<XLuaInstance> xLuaInstance = new MonoSingleton<XLuaInstance>(false);
        // public static MonoSingleton<DebugMessagePool> debugMessage = new MonoSingleton<DebugMessagePool>(false);
        public static MessagePoolController debugMessage => ui.Instance.combatView.messagePool;
        public static MonoSingleton<PopupManager> popupMgr = new MonoSingleton<PopupManager>(false);
        public static MonoSingleton<UI.GridOverlayManager> overlayMgr = new MonoSingleton<UI.GridOverlayManager>(false);
        public static MonoSingleton<SerialCoroutine> combatCoroutine = new MonoSingleton<SerialCoroutine>(false);
        public static MonoSingleton<PrefabPool> prefabs = new MonoSingleton<PrefabPool>(false);

        public static SerialCoroutineContext combatContext => combatCoroutine.Instance.currentContext;
        public static void CombatCoroutineNewContext(SerialCoroutineContext ctx) => combatCoroutine.Instance.SwitchContext(ctx);

        // Short-hand for combatContext
        public static SerialCoroutineContext cc => combatContext;
        public static void ccNewContext(SerialCoroutineContext ctx) => CombatCoroutineNewContext(ctx); // Short-hand for CombatCoroutineNewContext

        public static MonoSingleton<ActionHost> actionHost = new MonoSingleton<ActionHost>(true);

        public static Databackend backend = Databackend.GetSingleton();
        public static LocalizationManager localizer = LocalizationManager.GetSingleton();
        public static CombatTracker combatTracker = new CombatTracker();

        public static Logger logger = new Logger("miniRAID.log");
    }

    public static class Settings
    {
        // Automatically passes unit when AP = 0 after action
        public static bool autoPass = true;
        
        // Aggro threshold for "High Aggro!"
        public static float highAggroThreshold = 0.8f;

        // Time delay to delete fx (particle systems etc.) after they have stopped.
        public static float fxTimeout = 10.0f;
    }
}
