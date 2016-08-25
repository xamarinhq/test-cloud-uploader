using System;
using System.Collections.Generic;

namespace Microsoft.AppHub.Common
{
    public class RecordedLogs
    {
        private readonly object _syncLock = new object();
        private readonly IList<RecordedLog> _logs;

        public RecordedLogs()
        {
            _logs = new List<RecordedLog>();
        }

        public void AddLog(RecordedLog log)
        {
            if (log == null)
                throw new ArgumentNullException(nameof(log));

            lock (_syncLock)
            {
                _logs.Add(log);
            }
        }

        public IList<RecordedLog> GetAllLogs()
        {
            lock (_syncLock)
            {
                return new List<RecordedLog>(_logs);
            }
        }
    }
}