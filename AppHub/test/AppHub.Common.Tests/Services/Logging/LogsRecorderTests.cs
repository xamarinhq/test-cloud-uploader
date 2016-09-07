using System;
using System.Linq;
using Microsoft.AppHub.Common.Services.Logging;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Microsoft.AppHub.Common.Tests.Services.Logging
{
    public class LogsRecorderTests
    {
        [Fact]
        public void RecorderStoresAllRecorderLogs()
        {
            var recorder = new LogsRecorder();
            var entry1 = new LogEntry(DateTimeOffset.UtcNow, "Category 1", LogLevel.Error, 1, "Log 1");
            var entry2 = new LogEntry(DateTimeOffset.UtcNow, "Category 2", LogLevel.Information, 2, "Log 2");

            recorder.RecordLogEntry(entry1);
            recorder.RecordLogEntry(entry2);

            Assert.Equal(new[] { entry1, entry2 }.ToList(), recorder.GetRecordedLogs());
        }
    }
}