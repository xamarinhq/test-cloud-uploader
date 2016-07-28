using System.Threading;
using Microsoft.Extensions.Logging;

namespace Microsoft.AppHub.Common
{
    public class LoggerService: ILoggerService
    {
        private readonly ILoggerFactory _loggerFactory = new LoggerFactory().AddDebug();
        private int _nextEventId = 999;

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