using Microsoft.Extensions.Logging;

namespace Microsoft.Xtc.Common.Services.Logging
{
    /// <summary>
    /// Represents service that creates logger and unique log event IDs. 
    /// </summary>
    public interface ILoggerService
    {
        /// <summary>
        /// Minimum log level.
        /// </summary>
        LogLevel MinimumLogLevel { get; set; }

        /// <summary>
        /// Sets logger provider. 
        /// </summary>
        /// <param name="loggerProvider">Logger provider.</param>
        void SetLoggerProvider(ILoggerProvider loggerProvider);

        /// <summary>
        /// Creates logger for a given type. 
        /// </summary>
        ILogger CreateLogger<T>();
    }
}
