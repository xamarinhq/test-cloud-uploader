using System;
using Microsoft.Extensions.Logging;

namespace Microsoft.AppHub.Common
{
    /// <summary>
    /// Default implementation of ILoggerService. 
    /// </summary>
    public class LoggerService: ILoggerService
    {
        private readonly object _syncLock = new object();

        private LogLevel _minimumLogLevel;
        private ILoggerProvider _loggerProvider;
        private LoggerFactory _loggerFactory;

        /// <summary>
        /// Creates new instance of the logger service, which doesn't write to console and logs only errors.
        /// </summary>
        public LoggerService()
            : this(LogLevel.Information)
	    { } 

        /// <summary>
        /// Creates new instance of the logger service.
        /// </summary>
        /// <param name="logToConsole">Flag that controls whether logs should be written to console.</param>
        /// <param name="minimumLogLevel">Minimum log level used by the logger.</param>
        public LoggerService(LogLevel minimumLogLevel)
        {
            _minimumLogLevel = minimumLogLevel;
            _loggerProvider = new SimpleConsoleLoggerProvider(minimumLogLevel);
        }

        /// <summary>
        /// Minimum log level.
        /// </summary>
        public LogLevel MinimumLogLevel
        {
            get { return _minimumLogLevel; }
            set
            {
                lock (_syncLock)
                {
                    _minimumLogLevel = value;
                    _loggerFactory = null;
                }
            }
        }

        /// <summary>
        /// Sets logger provider. 
        /// </summary>
        /// <param name="loggerProvider">Logger provider.</param>
        public void SetLoggerProvider(ILoggerProvider loggerProvider)
        {
            if (loggerProvider == null)
                throw new ArgumentNullException(nameof(loggerProvider));
            
            lock (_syncLock)
            {
                _loggerProvider = loggerProvider;
                _loggerFactory = null;
            }
        }

        /// <summary>
        /// Creates logger for a given type. 
        /// </summary>
        public ILogger CreateLogger<T>()
        {            
            lock (_syncLock)
            {
                if (_loggerFactory == null)
                {
                    _loggerFactory = new LoggerFactory();
                    _loggerFactory.AddProvider(_loggerProvider);
                    _loggerFactory.AddDebug(_minimumLogLevel);
                }
                return _loggerFactory.CreateLogger<T>();
            }
        }
    }
}