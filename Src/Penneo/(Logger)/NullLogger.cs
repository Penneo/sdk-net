namespace Penneo
{
    [global::System.Obsolete(global::Penneo.SdkDeprecation.Message)]
    public class NullLogger : IPenneoLogger
    {
        public void Log(string message, LogSeverity severity)
        {
            // does nothing
        }
    }
}
