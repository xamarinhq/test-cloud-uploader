using Microsoft.Extensions.Logging;

namespace Microsoft.AppHub.Common
{
    /// <summary>
    /// A provider (factory class) for SimpleConsoleLogger.
    /// </summary>
    public class SimpleConsoleLoggerProvider : ILoggerProvider
    {
        private readonly LogLevel _minimumLogLevel;

        public SimpleConsoleLoggerProvider(LogLevel minimumLogLevel)
        {
            _minimumLogLevel = minimumLogLevel;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new SimpleConsoleLogger(_minimumLogLevel);
        }

        public void Dispose()
        {
        }
    }
}