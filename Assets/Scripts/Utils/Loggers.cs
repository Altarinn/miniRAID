using System;
using System.IO;

namespace miniRAID
{
    public class Logger
    {
        public string debugLogPath;

        public Logger(string path)
        {
            this.debugLogPath = path;
#if !UNITY_WEBGL
            File.WriteAllText(debugLogPath, $"- Start of debug log -\n[{DateTime.Now}] AWAKE\n");
#endif
        }

        public virtual void Log(string message)
        {
#if !UNITY_WEBGL
            File.AppendAllText(debugLogPath, $"[{DateTime.Now}] {message}\n");
#endif
        }
    }
    
    public class LoggerWithUI : Logger
    {
        public LoggerWithUI(string path) : base(path) { }

        public override void Log(string message)
        {
            base.Log(message);
            Globals.ui.Instance.combatView.messagePool.AddMessage(message);
        }
    }

    public class CombatJSONLogger
    {
        
    }
}