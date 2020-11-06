using System;
using System.Net.Mail;

namespace Logging
{
    public static class Log
    {
        static ILog _logger = new DebugLogger();
        static readonly object _lock = new object();

        public static void Use(ILog logger, LogLevels level)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));

            lock (_lock)
            {
                _logger = new LogLeveler(logger, level);
            }
        }

        public static void LogError(string message) => _logger.LogError(message);

        public static void LogError(Exception ex, string message) => _logger.LogError(ex, message);

        public static void LogInfo(string message) => _logger.LogInfo(message);

        public static void LogDebug(string message) => _logger.LogDebug(message);

        public static void LogSuccess(string message) => _logger.LogSuccess(message);
    }
}
