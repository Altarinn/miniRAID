using UnityEngine;
using System.Collections;

namespace miniRAID
{
    public static class Globals
    {
        public static MonoSingleton<UI.GridUI> ui = new MonoSingleton<UI.GridUI>(false);
        public static MonoSingleton<Scheduler> scheduler = new MonoSingleton<Scheduler>(false);
        public static MonoSingleton<CombatSchedulerCoroutine> combatMgr = new MonoSingleton<CombatSchedulerCoroutine>(false);
        public static MonoSingleton<SerialCoroutine> serialCoroutine = new MonoSingleton<SerialCoroutine>(false);
        public static MonoSingleton<XLuaInstance> xLuaInstance = new MonoSingleton<XLuaInstance>(false);
        public static MonoSingleton<DebugMessagePool> debugMessage = new MonoSingleton<DebugMessagePool>(false);
        public static MonoSingleton<PopupManager> popupMgr = new MonoSingleton<PopupManager>(false);
        public static MonoSingleton<UI.GridOverlayManager> overlayMgr = new MonoSingleton<UI.GridOverlayManager>(false);

        public static MonoSingleton<ActionHost> actionHost = new MonoSingleton<ActionHost>(true);

        public static Databackend backend = Databackend.GetSingleton();
        public static CombatStatistics combatStats = new CombatStatistics();
    }

    public static class Settings
    {
        // Automatically passes unit when AP = 0 after action
        public static bool autoPass = true;

        // Time delay to delete fx (particle systems etc.) after they have stopped.
        public static float fxTimeout = 10.0f;
    }
}
