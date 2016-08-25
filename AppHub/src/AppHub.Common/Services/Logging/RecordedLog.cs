using System;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Microsoft.AppHub.Common
{
    public class RecordedLog
    {
        public RecordedLog(DateTimeOffset timeStamp, string categoryName, LogLevel logLevel, EventId eventId, string message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            if (categoryName == null)
                throw new ArgumentNullException(nameof(categoryName));

            this.TimeStamp = timeStamp;
            this.CategoryName = categoryName;
            this.LogLevel = logLevel;
            this.EventId = eventId;
            this.Message = message;
        }

        public DateTimeOffset TimeStamp { get; }

        public string CategoryName { get; }

        public LogLevel LogLevel { get; }

        public EventId EventId { get; }

        public string Message { get; }

        public override string ToString()
        {
            if (this.LogLevel >= LogLevel.Error)
                return $"Error: {this.Message}";
            else
                return this.Message;
        }

        public string ToDiagnosticString()
        {
            var result = new StringBuilder();
            
            var prefix = LoggerExtensions.GetLogLevelPrefix(this.LogLevel);
            result.AppendLine($"{prefix}: {this.CategoryName}[{this.EventId.Id}]");
            
            foreach (var line in this.Message.Split('\n'))
            {
                result.AppendLine($"      {line.Trim('\r')}");
            }

            return result.ToString();
        }
    }
}