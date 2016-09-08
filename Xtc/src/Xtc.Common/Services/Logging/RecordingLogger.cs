using System;
using Microsoft.Extensions.Logging;

namespace Microsoft.Xtc.Common.Services.Logging
{
    /// <summary>
    /// ILogger implementation that records all received log entries.
    /// </summary>
    public class RecordingLogger : ILogger
    {
        private readonly LogLevel _minimumLogLevel;
        private readonly string _categoryName;
        private readonly LogsRecorder _logsRecorder;

        /// <summary>
        /// Constructor. Creates new instance of the logger.
        /// </summary>
        /// <param name="minimumLogLevel">Minimum log level to store.</param>
        /// <param name="categoryName">Category name for the logger.</param>
        /// <param name="logsRecorder">Logs recorder used to store recieved log entries.</param>
        public RecordingLogger(LogLevel minimumLogLevel, string categoryName, LogsRecorder logsRecorder)
        {
            if (logsRecorder == null)
                throw new ArgumentNullException(nameof(logsRecorder));
            if (categoryName == null)
                throw new ArgumentNullException(nameof(categoryName));

            _minimumLogLevel = minimumLogLevel;
            _categoryName = categoryName;
            _logsRecorder = logsRecorder;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            var logEntry = new LogEntry(
                DateTimeOffset.UtcNow,
                _categoryName,
                LogLevel.Information,
                new EventId(),
                $"### {state} ###"
            );

            _logsRecorder.RecordLogEntry(logEntry);

            return new NoOpDisposable();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= _minimumLogLevel;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            var logEntry = new LogEntry(
                DateTimeOffset.UtcNow,
                _categoryName,
                logLevel,
                eventId,
                formatter(state, exception)
            );

            _logsRecorder.RecordLogEntry(logEntry);
        }

        private class NoOpDisposable: IDisposable
        {
            public void Dispose()
            { }
        }
    }
}