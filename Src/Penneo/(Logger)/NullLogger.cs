namespace Penneo
{
    public class NullLogger : IPenneoLogger
    {
        public void Log(string message, LogSeverity severity)
        {
            // does nothing
        }
    }
}
