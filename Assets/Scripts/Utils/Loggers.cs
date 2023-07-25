using System;
using System.IO;
using UnityEngine;

namespace miniRAID
{
    public class Logger
    {
        public string debugLogPath;

        public Logger(string path)
        {
            this.debugLogPath = path;
            
            if(Application.isEditor)
                File.WriteAllText(debugLogPath, $"- Start of debug log -\n[{DateTime.Now}] AWAKE\n");
            
#if !UNITY_WEBGL
            File.WriteAllText(debugLogPath, $"- Start of debug log -\n[{DateTime.Now}] AWAKE\n");
#endif
        }

        public virtual void Log(string message)
        {
            if (Application.isEditor)
            {
                string minSec = string.Format("[回合{0}] {1:00}m {2:00}s: ", miniRAID.Globals.combatMgr.Instance.turn, (int)Time.realtimeSinceStartup / 60, (int)Time.realtimeSinceStartup % 60);
                
                File.AppendAllText(debugLogPath, $"{minSec} {message}\n");
            }

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