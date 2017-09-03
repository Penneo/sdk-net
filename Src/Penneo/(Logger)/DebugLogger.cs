using System.Diagnostics;

namespace Penneo
{
    internal class DebugLogger : ILogger
    {
        public void Log(string message, LogSeverity severity)
        {
            Debug.WriteLine(severity + ": " + message);
        }
    }
}
