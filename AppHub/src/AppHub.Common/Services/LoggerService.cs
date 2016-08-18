using System.Threading;
using Microsoft.Extensions.Logging;

namespace Microsoft.AppHub.Common
{
    /// <summary>
    /// Default implementation of ILoggerService. 
    /// </summary>
    public class LoggerService: ILoggerService
    {
        private readonly ILoggerFactory _loggerFactory;        
        private int _nextEventId = 0;

        /// <summary>
        /// Creates new instance of the logger service, which doesn't write to console and logs only errors.
        /// </summary>
        public LoggerService()
            : this(true, LogLevel.Information)
	    { } 

        /// <summary>
        /// Creates new instance of the logger service.
        /// </summary>
        /// <param name="logToConsole">Flag that controls whether logs should be written to console.</param>
        /// <param name="minimumLogLevel">Minimum log level used by the logger.</param>
        public LoggerService(bool logToConsole, LogLevel minimumLogLevel)
        {
            _loggerFactory = new LoggerFactory();
            _loggerFactory.AddDebug(minimumLogLevel);
            
            if (logToConsole)
                _loggerFactory.AddConsole();
        }

        /// <summary>
        /// Creates logger for a given type. 
        /// </summary>
        public ILogger CreateLogger<T>()
        {            
            return _loggerFactory.CreateLogger<T>();
        }

        /// <summary>
        /// Creates unique event ID. 
        /// </summary>
        public EventId CreateEventId()
        {
            return Interlocked.Increment(ref _nextEventId);
        }
    }
}