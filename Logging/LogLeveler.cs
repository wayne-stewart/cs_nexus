using System;

namespace Logging
{
    public sealed class LogLeveler : ILog
    {
        readonly LogLevels _level;
        readonly ILog _logger;

        public LogLeveler(ILog logger, LogLevels level)
        {
            _level = level;
            _logger = logger;
        }

        void Run(LogTypes type, Action<string> action, string message)
        {
            if (((uint)_level & (uint)type) > 0)
            {
                action(message);
            }
        }

        void ILog.LogDebug(string message) => Run(LogTypes.Debug, _logger.LogDebug, message);

        void ILog.LogError(string message) => Run(LogTypes.Error, _logger.LogError, message);

        void ILog.LogInfo(string message) => Run(LogTypes.Info, _logger.LogInfo, message);

        void ILog.LogSuccess(string message) => Run(LogTypes.Success, _logger.LogSuccess, message);
    }
}
