using Microsoft.Extensions.Logging;

namespace Microsoft.AppHub.Common
{
    /// <summary>
    /// Represents service that creates logger and unique log event IDs. 
    /// </summary>
    public interface ILoggerService
    {
        /// <summary>
        /// Creates logger for a given type. 
        /// </summary>
        ILogger CreateLogger<T>();

        /// <summary>
        /// Creates unique event ID. 
        /// </summary>
        EventId CreateEventId();     
    }
}
