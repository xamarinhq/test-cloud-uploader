using System;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Microsoft.Xtc.Common.Services.Logging
{
    /// <summary>
    /// Represents log entry.
    /// </summary>
    public class LogEntry
    {
        /// <summary>
        /// Constructor. Creates new log entry.
        /// </summary>
        /// <param name="timeStamp">Time stamp of the log entry.</param>
        /// <param name="categoryName">Category anme of the logger that produced this log entry.</param>
        /// <param name="logLevel">Log level of the log entry.</param>
        /// <param name="eventId">Event ID of the log entry.</param>
        /// <param name="message">Log message.</param>
        public LogEntry(
            DateTimeOffset timeStamp, string categoryName, LogLevel logLevel, EventId eventId, string message)
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

        /// <summary>
        /// Time stamp of the log entry.
        /// </summary>
        public DateTimeOffset TimeStamp { get; }

        /// <summary>
        /// Category name of the logger that produced this log entry.
        /// </summary>
        public string CategoryName { get; }

        /// <summary>
        /// Log level of the log entry. 
        /// </summary>
        public LogLevel LogLevel { get; }

        /// <summary>
        /// Event ID of the log entry.
        /// </summary>
        public EventId EventId { get; }

        /// <summary>
        /// Message of this log entry.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Creates simple, human-readable string representation of this log entry, i.e. string that
        /// contains only message and information whether the entry represents an error.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (this.LogLevel >= LogLevel.Error)
            {
                return $"Error: {this.Message}";
            }
            else
            {
                return this.Message;
            }
        }

        /// <summary>
        /// Creates diagnostic string representation of this log entry, i.e. string that 
        /// contains log level, category, event ID and the message. 
        /// </summary>
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