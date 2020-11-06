using System;
using System.Linq;

namespace Logging
{
    public sealed class ConsoleLogger : ILog
    {
        static readonly object _lock = new object();

        readonly ConsoleColor
            ErrorColor,
            InfoColor,
            DebugColor,
            SuccessColor;

        public ConsoleLogger()
        {
            ErrorColor = ConsoleColor.Red;
            InfoColor = Console.ForegroundColor;
            DebugColor = Console.ForegroundColor;
            SuccessColor = ConsoleColor.Green;
        }

        static void WriteLine(string message, ConsoleColor color)
        {
            lock (_lock)
            {
                var old_color = Console.ForegroundColor;
                Console.ForegroundColor = color;
                Console.WriteLine(message);
                Console.ForegroundColor = old_color;
            }
        }

        void ILog.LogDebug(string message) => WriteLine(message, DebugColor);

        void ILog.LogError(string message) => WriteLine(message, ErrorColor);

        void ILog.LogInfo(string message) => WriteLine(message, InfoColor);

        void ILog.LogSuccess(string message) => WriteLine(message, SuccessColor);
    }
}
