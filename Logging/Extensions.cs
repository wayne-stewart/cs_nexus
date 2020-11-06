using System;
using System.Linq;

namespace Logging
{
    public static class Extensions
    {
        public static void LogError(this ILog logger, Exception ex, string message)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));

            var msg = string.Empty;
            if (!string.IsNullOrWhiteSpace(message))
            {
                msg = message;
            }
            if (ex != null)
            {
                if (msg.Length > 0)
                {
                    msg += Environment.NewLine;
                }

                msg +=
                    (ex.Message ?? string.Empty) +
                    Environment.NewLine +
                    string.Join(Environment.NewLine, (ex.StackTrace ?? string.Empty).Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).Take(3).Select(s => s.Trim()));
            }
            logger.LogError(msg);
        }
    }
}
