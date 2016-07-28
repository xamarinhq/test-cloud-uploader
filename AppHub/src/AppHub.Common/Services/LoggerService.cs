using System.Threading;
using Microsoft.Extensions.Logging;

namespace Microsoft.AppHub.Common
{
    public class LoggerService: ILoggerService
    {
        private readonly ILoggerFactory _loggerFactory;        
        private int _nextEventId = 0;

        public LoggerService()
            : this(false, LogLevel.Error)
	    { } 

        public LoggerService(bool logToConsole, LogLevel minimumLogLevel)
        {
            _loggerFactory = new LoggerFactory();
            _loggerFactory.AddDebug(minimumLogLevel);
            if (logToConsole)
                _loggerFactory.AddConsole();
        }

        public ILogger CreateLogger<T>()
        {            
            return _loggerFactory.CreateLogger<T>();
        }

        public EventId CreateEventId()
        {
            return Interlocked.Increment(ref _nextEventId);
        }
    }
}