using System;
using Microsoft.Extensions.Logging;

namespace Microsoft.AppHub.Common
{
    public class RecordingLoggerProvider : ILoggerProvider
    {
        private readonly LogLevel _minimumLogLevel;
        private readonly RecordedLogs _recordedLogs;

        public RecordingLoggerProvider(LogLevel minimumLogLevel, RecordedLogs recordedLogs)
        {
            if (recordedLogs == null)
                throw new ArgumentNullException(nameof(recordedLogs));

            _minimumLogLevel = minimumLogLevel;
            _recordedLogs = recordedLogs;
        }

        public ILogger CreateLogger(string categoryName)
        {
            if (categoryName == null)
                throw new ArgumentNullException(nameof(categoryName));

            return new RecordingLogger(_minimumLogLevel, categoryName, _recordedLogs);
        }

        public void Dispose()
        {
        }
    }
}