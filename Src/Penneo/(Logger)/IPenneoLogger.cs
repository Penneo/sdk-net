namespace Penneo
{
    /// <summary>
    /// Logging interface. Inject into Penneo using PenneoConnector to retrieve log information
    /// </summary>
    public interface IPenneoLogger
    {
        void Log(string message, LogSeverity severity);
    }

    /// <summary>
    /// Log entry severities
    /// </summary>
    public enum LogSeverity
    {
        Trace,
        Information,
        Debug,
        Warning,
        Error,
        Fatal
    }
}