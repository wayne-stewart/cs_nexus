namespace Logging
{
    public interface ILog
    {
        void LogError(string message);
        void LogInfo(string message);
        void LogDebug(string message);
        void LogSuccess(string message);
    }
}
