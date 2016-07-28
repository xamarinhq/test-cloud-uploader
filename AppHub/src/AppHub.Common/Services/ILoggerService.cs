using Microsoft.Extensions.Logging;

namespace Microsoft.AppHub.Common
{
    public interface ILoggerService
    {
        ILogger CreateLogger<T>();

        EventId CreateEventId();     
    }
}
