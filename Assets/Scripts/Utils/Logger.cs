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
            File.WriteAllText(debugLogPath, $"- Start of debug log -\n[{DateTime.Now}] AWAKE\n");
        }

        public void Log(string message)
        {
            File.AppendAllText(debugLogPath, $"[{DateTime.Now}] {message}\n");
        }
    }
}