using System;
using Microsoft.Extensions.Logging;

namespace Microsoft.AppHub.Common
{
    /// <summary>
    /// A logger provider (logger factory) that creates recording loggers.
    /// </summary>
    public class RecordingLoggerProvider : ILoggerProvider
    {
        private readonly LogLevel _minimumLogLevel;
        private readonly LogsRecorder _logsRecorder;

        /// <summary>
        /// Constructor. Creates new logger provider instance.
        /// </summary>
        /// <param name="minimumLogLevel">Minimum log level.</param>
        /// <param name="logsRecorder">Logs recorder that will store created logs.</param>
        public RecordingLoggerProvider(LogLevel minimumLogLevel, LogsRecorder logsRecorder)
        {
            if (logsRecorder == null)
                throw new ArgumentNullException(nameof(logsRecorder));

            _minimumLogLevel = minimumLogLevel;
            _logsRecorder = logsRecorder;
        }

        /// <summary>
        /// Creates new recording logger instance.
        /// </summary>
        /// <param name="categoryName">Category name for the logger.</param>
        /// <returns>New recording logger instance.</returns>
        public ILogger CreateLogger(string categoryName)
        {
            if (categoryName == null)
                throw new ArgumentNullException(nameof(categoryName));

            return new RecordingLogger(_minimumLogLevel, categoryName, _logsRecorder);
        }

        /// <summary>
        /// Disposes the provider.
        /// </summary>
        public void Dispose()
        {
        }
    }
}