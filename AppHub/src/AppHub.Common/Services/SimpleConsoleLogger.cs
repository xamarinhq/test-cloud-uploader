using System;
using Microsoft.Extensions.Logging;

namespace Microsoft.AppHub.Common
{
    /// <summary>
    /// A simple console logger that prints only text messages, without event IDs, timestamps, categories, etc. 
    /// </summary>
    public class SimpleConsoleLogger : ILogger
    {
        private LogLevel _minimumLogLevel;

        public SimpleConsoleLogger(LogLevel minimumLogLevel)
        {
            _minimumLogLevel = minimumLogLevel;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return new ConsoleLogScope(state?.ToString() ?? "New scope");
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return (int)logLevel >= (int)_minimumLogLevel;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            var message = state.ToString();
            if ((int)logLevel >= (int)LogLevel.Error)
            {
                Console.Error.WriteLine($"Error: {message}");
            }
            else
            {
                Console.WriteLine(message);
            }
        }

        private class ConsoleLogScope: IDisposable
        {
            public ConsoleLogScope(string name)
            {
                Console.WriteLine($"### {name} ###");
            }

            public void Dispose()
            {
                Console.WriteLine();
            }
        }
    }
}