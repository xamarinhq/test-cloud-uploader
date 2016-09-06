using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Microsoft.AppHub.Common
{
    /// <summary>
    /// A simple console logger that prints only text messages, without event IDs, timestamps, categories, etc. 
    /// </summary>
    public class SimpleConsoleLogger : ILogger
    {
        private static readonly IDictionary<LogLevel, ConsoleColors> LogLevelColors = new Dictionary<LogLevel, ConsoleColors>()
        {
            [LogLevel.Critical] = new ConsoleColors(ConsoleColor.White, ConsoleColor.Red),
            [LogLevel.Error] = new ConsoleColors(ConsoleColor.Black, ConsoleColor.Red),
            [LogLevel.Warning] = new ConsoleColors(ConsoleColor.Yellow, ConsoleColor.Black),
            [LogLevel.Information] = new ConsoleColors(ConsoleColor.DarkGreen, ConsoleColor.Black),
            [LogLevel.Debug] = new ConsoleColors(ConsoleColor.Gray, ConsoleColor.Black),
            [LogLevel.Trace] = new ConsoleColors(ConsoleColor.Gray, ConsoleColor.Black)
        };

        private readonly LogLevel _minimumLogLevel;
        private readonly string _categoryName;

        public SimpleConsoleLogger(LogLevel minimumLogLevel, string categoryName)
        {
            _minimumLogLevel = minimumLogLevel;
            _categoryName = categoryName ?? string.Empty;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return new ConsoleLogScope(state?.ToString() ?? "New scope");
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= _minimumLogLevel;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            var message = formatter(state, exception);
            if (_minimumLogLevel > LogLevel.Debug)
            {
                WriteSimpleLog(logLevel, message);
            }
            else
            {
                WriteDiagnosticLog(logLevel, eventId, message);
            }
        }

        private void WriteSimpleLog(LogLevel logLevel, string message)
        {
            if (logLevel >= LogLevel.Error)
            {
                Console.Error.WriteLine($"Error: {message}");
            }
            else
            {
                Console.Out.WriteLine(message);
            }
        }

        private void WriteDiagnosticLog(LogLevel logLevel, EventId eventId, string message)
        {
            var output = logLevel >= LogLevel.Error ? Console.Error : Console.Out;
            var prefix = LoggerExtensions.GetLogLevelPrefix(logLevel);

            var originalColors = ConsoleColors.FromCurrent();
            try
            {
                LogLevelColors[logLevel].Apply();
                output.Write(prefix);
            }
            finally
            {
                originalColors.Apply();
            }

            output.WriteLine($": {_categoryName}[{eventId.Id}]");

            foreach (var line in message.Split('\n'))
            {
                output.WriteLine($"      {line.Trim('\r')}");
            }
        }

        private class ConsoleColors
        {
            public static ConsoleColors FromCurrent()
            {
                return new ConsoleColors(Console.ForegroundColor, Console.BackgroundColor);
            }

            public ConsoleColors(ConsoleColor foreground, ConsoleColor background)
            {
                this.Foreground = foreground; 
                this.Background = background;
            }

            public ConsoleColor Foreground { get; }

            public ConsoleColor Background { get; }

            public void Apply()
            {
                Console.ForegroundColor = this.Foreground;
                Console.BackgroundColor = this.Background;
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