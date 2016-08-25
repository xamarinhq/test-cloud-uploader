using System;
using Microsoft.Extensions.Logging;

namespace Microsoft.AppHub.Common
{
    public class RecordingLogger : ILogger
    {
        private readonly LogLevel _minimumLogLevel;
        private readonly string _categoryName;
        private readonly RecordedLogs _logs;

        public RecordingLogger(LogLevel minimumLogLevel, string categoryName, RecordedLogs logs)
        {
            if (logs == null)
                throw new ArgumentNullException(nameof(logs));
            if (categoryName == null)
                throw new ArgumentNullException(nameof(categoryName));

            _minimumLogLevel = minimumLogLevel;
            _categoryName = categoryName;
            _logs = logs;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            var log = new RecordedLog(
                DateTimeOffset.UtcNow,
                _categoryName,
                LogLevel.Information,
                new EventId(),
                $"### {state} ###"
            );

            _logs.AddLog(log);

            return new NoOpDisposable();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= _minimumLogLevel;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var log = new RecordedLog(
                DateTimeOffset.UtcNow,
                _categoryName,
                logLevel,
                eventId,
                formatter(state, exception)
            );
        }

        private class NoOpDisposable: IDisposable
        {
            public void Dispose()
            { }
        }
    }
}