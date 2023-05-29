using System;
using System.IO;

namespace miniRAID
{
    public class Logger
    {
        private static Logger _Instance;
        public static Logger Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new Logger("miniRAID.log");
                }
                return _Instance;
            }
        }
        
        public string debugLogPath;

        private Logger(string path)
        {
            this.debugLogPath = path;
            File.WriteAllText(debugLogPath, $"- Start of debug log -\n[{DateTime.Now}] AWAKE\n");
        }

        public void Log(string message)
        {
            File.AppendAllText(debugLogPath, $"[{DateTime.Now}] {message}\n");
        }
    }
}