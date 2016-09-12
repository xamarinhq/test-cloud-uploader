using System;
using System.Collections.Generic;

namespace Microsoft.Xtc.Common.Services.Logging
{
    /// <summary>
    /// Thread-safe collection of log entries.
    /// </summary>
    public class LogsRecorder
    {
        private readonly object _syncLock = new object();
        private readonly IList<LogEntry> _logs;

        /// <summary>
        /// Constructor. Creates new logs recorder instance.
        /// </summary>
        public LogsRecorder()
        {
            _logs = new List<LogEntry>();
        }

        /// <summary>
        /// Adds new log entry.
        /// </summary>
        /// <param name="logEntry">Log entry to add.</param>
        public void RecordLogEntry(LogEntry logEntry)
        {
            if (logEntry == null)
                throw new ArgumentNullException(nameof(logEntry));

            lock (_syncLock)
            {
                _logs.Add(logEntry);
            }
        }

        /// <summary>
        /// Gets all recorded log entries.
        /// </summary>
        /// <returns>Recorded log entries.</returns>
        public IList<LogEntry> GetRecordedLogs()
        {
            lock (_syncLock)
            {
                return new List<LogEntry>(_logs);
            }
        }
    }
}