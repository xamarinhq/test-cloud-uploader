using System;
using System.Collections.Generic;
using Microsoft.AppHub.Common.Services.Logging;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Microsoft.AppHub.Common.Tests.Services.Logging
{
    public class RecordingLoggerTests
    {
        [Fact]
        public void LoggerStoresEntries()
        {
            var recorder = new LogsRecorder();
            var logger = new RecordingLogger(LogLevel.Debug, "Test 1", recorder);
            
            logger.LogDebug(new EventId(1), "Log entry 1");
            logger.LogWarning(new EventId(2), "Log entry 2");
            logger.LogTrace(new EventId(3), "Log entry 3");

            var expected = new[]
            {
                new LogEntry(DateTimeOffset.UtcNow, "Test 1", LogLevel.Debug, new EventId(1), "Log entry 1"),
                new LogEntry(DateTimeOffset.UtcNow, "Test 1", LogLevel.Warning, new EventId(2), "Log entry 2"),
            };

            VerifyLogEntries(expected, recorder.GetRecordedLogs());
        }

        [Fact]
        public void LoggerStoresScopesAsInformationEntires()
        {
            var recorder = new LogsRecorder();
            var logger = new RecordingLogger(LogLevel.Debug, "Test 2", recorder);

            using (var scope = logger.BeginScope("Scope 1"))
            {
                logger.LogInformation(new EventId(1), "Log entry 1");
            };

            var expected = new[]
            {
                new LogEntry(DateTimeOffset.UtcNow, "Test 2", LogLevel.Information, new EventId(), "### Scope 1 ###"),
                new LogEntry(DateTimeOffset.UtcNow, "Test 2", LogLevel.Information, new EventId(1), "Log entry 1"),
            };

            VerifyLogEntries(expected, recorder.GetRecordedLogs());
        }

        private void VerifyLogEntries(IList<LogEntry> expected, IList<LogEntry> actual)
        {
            Assert.Equal(expected.Count, actual.Count);

            for (var i = 0; i < expected.Count; i++)
            {
                VerifyLogEntry(expected[i], actual[i]);
            }
        }

        private void VerifyLogEntry(LogEntry expected, LogEntry actual)
        {
            Assert.Equal(expected.CategoryName, actual.CategoryName);
            Assert.Equal(expected.LogLevel, actual.LogLevel);
            Assert.Equal(expected.EventId, actual.EventId);
            Assert.Equal(expected.Message, actual.Message);
        }
    }
}