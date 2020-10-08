using System;

namespace Penneo
{
    public class DebugLogger : IPenneoLogger
    {
        public void Log(string message, LogSeverity severity)
        {
            Console.WriteLine("Penneo: " + severity + ": " + message);
        }
    }
}
