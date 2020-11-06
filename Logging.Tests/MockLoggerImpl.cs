
namespace Logging.Tests
{
    public class MockLoggerImpl : ILog
    {
        public LogTypes LastType { get; set; }
        public string LastMessage { get; set; }
        public bool LogHappened { get; set; }

        public void ResetForTest()
        {
            LastType = 0;
            LastMessage = null;
            LogHappened = false;
        }

        public void LogDebug(string message)
        {
            LastType = LogTypes.Debug;
            LastMessage = message;
            LogHappened = true;
        }

        public void LogError(string message)
        {
            LastType = LogTypes.Error;
            LastMessage = message;
            LogHappened = true;
        }

        public void LogInfo(string message)
        {
            LastType = LogTypes.Info;
            LastMessage = message;
            LogHappened = true;
        }

        public void LogSuccess(string message)
        {
            LastType = LogTypes.Success;
            LastMessage = message;
            LogHappened = true;
        }
    }
}