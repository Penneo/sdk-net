using System.Diagnostics;

namespace Penneo
{
    public class DebugLogger : IPenneoLogger
    {
        public void Log(string message, LogSeverity severity)
        {
            Debug.WriteLine(severity + ": " + message);
        }
    }
}
