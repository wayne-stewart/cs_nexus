using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Logging.Tests
{
    public class LogLeveler_Test
    {
        [TestCase(LogLevels.Debug, "111", LogTypes.Debug, true)]
        [TestCase(LogLevels.Debug, "222", LogTypes.Error, true)]
        [TestCase(LogLevels.Debug, "333", LogTypes.Info, true)]
        [TestCase(LogLevels.Debug, "444", LogTypes.Success, true)]
        [TestCase(LogLevels.Info, "555", LogTypes.Debug, false)]
        [TestCase(LogLevels.Info, "666", LogTypes.Error, true)]
        [TestCase(LogLevels.Info, "777", LogTypes.Info, true)]
        [TestCase(LogLevels.Info, "888", LogTypes.Success, true)]
        [TestCase(LogLevels.Error, "999", LogTypes.Debug, false)]
        [TestCase(LogLevels.Error, "AAA", LogTypes.Error, true)]
        [TestCase(LogLevels.Error, "BBB", LogTypes.Info, false)]
        [TestCase(LogLevels.Error, "CCC", LogTypes.Success, true)]
        public void Test(LogLevels level_under_test, string message, LogTypes type, bool should_log)
        {
            var mock_logger = new MockLoggerImpl();

            var log_leveler = new LogLeveler(mock_logger, level_under_test) as ILog;

            mock_logger.ResetForTest();

            switch (type)
            {
                case LogTypes.Debug:
                    log_leveler.LogDebug(message);
                    break;
                case LogTypes.Error:
                    log_leveler.LogError(message);
                    break;
                case LogTypes.Info:
                    log_leveler.LogInfo(message);
                    break;
                case LogTypes.Success:
                    log_leveler.LogSuccess(message);
                    break;
            }
            
            Assert.That(mock_logger.LogHappened, Is.EqualTo(should_log));
            if (should_log)
            {
                Assert.That(mock_logger.LastMessage, Is.EqualTo(message));
                Assert.That(mock_logger.LastType, Is.EqualTo(type));
            }
        }
    }
}
