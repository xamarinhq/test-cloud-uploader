using Microsoft.Extensions.Logging;

namespace Microsoft.AppHub.Common
{
    /// <summary>
    /// A provider (factory class) for SimpleConsoleLogger.
    /// </summary>
    public class SimpleConsoleLoggerProvider : ILoggerProvider
    {
        private readonly LogLevel _minimumLogLevel;

        /// <summary>
        /// Constructor. Creates new provider instance.
        /// </summary>
        /// <param name="minimumLogLevel"></param>
        public SimpleConsoleLoggerProvider(LogLevel minimumLogLevel)
        {
            _minimumLogLevel = minimumLogLevel;
        }

        /// <summary>
        /// Creates new console logger instance.
        /// </summary>
        /// <param name="categoryName">Category name for the logger.</param>
        /// <returns>New console logger instance.</returns>
        public ILogger CreateLogger(string categoryName)
        {
            return new SimpleConsoleLogger(_minimumLogLevel, categoryName);
        }

        /// <summary>
        /// Disposes the provider.
        /// </summary>
        public void Dispose()
        {
        }
    }
}