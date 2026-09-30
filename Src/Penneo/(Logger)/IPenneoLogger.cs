namespace Penneo
{
    /// <summary>
    /// Logging interface. Inject into Penneo using PenneoConnector to retrieve log information
    /// </summary>
    [global::System.Obsolete(global::Penneo.SdkDeprecation.Message)]
    public interface IPenneoLogger
    {
        void Log(string message, LogSeverity severity);
    }

    /// <summary>
    /// Log entry severities
    /// </summary>
    [global::System.Obsolete(global::Penneo.SdkDeprecation.Message)]
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