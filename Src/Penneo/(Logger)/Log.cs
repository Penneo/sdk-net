namespace Penneo
{
    /// <summary>
    /// Internal log helper
    /// </summary>
    internal static class Log
    {
        private static ILogger _logger;

        /// <summary>
        /// Sets the logger to output messages to
        /// </summary>        
        public static void SetLogger(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Write an entry to the log
        /// </summary>
        internal static void Write(string message, LogSeverity severity)
        {
            if (_logger != null)
            {
                _logger.Log(message, severity);
            }
        }
    }
}