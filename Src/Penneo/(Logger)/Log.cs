namespace Penneo
{
    /// <summary>
    /// Internal log helper
    /// </summary>
    internal class Log
    {
        private ILogger _logger;

        /// <summary>
        /// Sets the logger to output messages to
        /// </summary>        
        public void SetLogger(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Write an entry to the log
        /// </summary>
        internal void Write(string message, LogSeverity severity)
        {
            if (_logger != null)
            {
                _logger.Log(message, severity);
            }
        }
    }
}