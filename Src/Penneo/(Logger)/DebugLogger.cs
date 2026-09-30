using System;

namespace Penneo
{
    [global::System.Obsolete(global::Penneo.SdkDeprecation.Message)]
    public class DebugLogger : IPenneoLogger
    {
        public void Log(string message, LogSeverity severity)
        {
            Console.WriteLine("Penneo: " + severity + ": " + message);
        }
    }
}
