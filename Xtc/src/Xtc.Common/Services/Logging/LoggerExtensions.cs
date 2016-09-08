using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Microsoft.Xtc.Common.Services.Logging
{
    /// <summary>
    /// Extension methods for the ILogger interface.
    /// </summary>
    public static class LoggerExtensions
    {
        public static void LogTrace(this ILogger logger, EventId eventId, IEnumerable<string> lines)
        {
            logger.LogTrace(eventId, ConcatenateLines(lines));
        }

        public static void LogDebug(this ILogger logger, EventId eventId, IEnumerable<string> lines)
        {
            logger.LogDebug(eventId, ConcatenateLines(lines));
        }

        public static void LogInformation(this ILogger logger, EventId eventId, IEnumerable<string> lines)
        {
            logger.LogInformation(eventId, ConcatenateLines(lines));
        }

        public static void LogWarning(this ILogger logger, EventId eventId, IEnumerable<string> lines)
        {
            logger.LogWarning(eventId, ConcatenateLines(lines));
        }

        public static void LogError(this ILogger logger, EventId eventId, IEnumerable<string> lines)
        {
            logger.LogError(eventId, ConcatenateLines(lines));
        }

        public static void LogCritical(this ILogger logger, EventId eventId, IEnumerable<string> lines)
        {
            logger.LogCritical(eventId, ConcatenateLines(lines));
        }

        private static string ConcatenateLines(IEnumerable<string> lines)
        {
            var result = new StringBuilder();

            foreach (var line in lines)
            {
                if (result.Length > 0)
                    result.AppendLine();

                result.Append(line);
            }

            return result.ToString();
        }

        /// <summary>
        /// Returns string prefix for a given log level. The prefix is the same as default 
        /// Console Logger uses. See https://docs.asp.net/en/latest/fundamentals/logging.html
        /// for more information.
        /// </summary>
        public static string GetLogLevelPrefix(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Critical:
                    return "crit";
                case LogLevel.Error:
                    return "fail";
                case LogLevel.Warning:
                    return "warn";
                case LogLevel.Information:
                    return "info";
                case LogLevel.Debug:
                    return "dbug";
                case LogLevel.Trace:
                    return "trce";
                default:
                    throw new ArgumentException($"Unrecognized LogLevel \"{logLevel}\"", nameof(logLevel));
            }
        }
    }
}