
using System.Diagnostics;

namespace Logging
{
    public sealed class DebugLogger : ILog
    {
        void ILog.LogDebug(string message) => Debug.WriteLine(message);

        void ILog.LogError(string message) => Debug.WriteLine(message);

        void ILog.LogInfo(string message) => Debug.WriteLine(message);

        void ILog.LogSuccess(string message) => Debug.WriteLine(message);
    }
}
