using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DocoptNet;
using Microsoft.AppHub.Cli;
using Microsoft.AppHub.Common;
using Microsoft.Extensions.Logging;

namespace Microsoft.AppHub.TestCloud
{
    public class RunExtensionCommandDescription: ICommand
    {
        public const string RunExtensionCommandName = "run";

        public string Name
        {
            get { return RunExtensionCommandName; }
        }

        public string Summary
        {
            get { return "Runs external command extension"; }
        }

        public string Syntax
        {
            get
            {
                return $@"Command '{this.Name}': {this.Summary}.

Usage:
  {ProgramUtilities.CurrentExecutableName} [{this.Name}] <command> [<arguments> ...]

";
            }
        }

        public ICommandExecutor CreateCommandExecutor(IDictionary<string, ValueObject> options, IServiceProvider serviceProvider)
        {
            var commandName = $"app-{options["<command>"].ToString()}";
            var arguments = options["<arguments>"]?
                .AsList
                .Cast<object>()
                .Select(a => a.ToString())
                .Aggregate(new StringBuilder(), (sb, a) => sb.Append($"{a} "), sb => sb.ToString());

            var processService = (IProcessService)serviceProvider.GetService(typeof(IProcessService));
            var loggerService = (ILoggerService)serviceProvider.GetService(typeof(ILoggerService));

            return new RunExtensionCommandExecutor(loggerService, processService, commandName, arguments);
        }
    }

    public class RunExtensionCommandExecutor: ICommandExecutor
    {
        public static readonly EventId ExecutingCommandEventId = 1;
        public static readonly EventId ExecutionSucceededEventId = 2;
        public static readonly EventId ExecutionFailedEventId = 3;

        private readonly object _consoleLock = new object();

        private readonly ILogger _logger;
        private readonly ILoggerService _loggerService;
        private readonly IProcessService _processService;
        private readonly string _commandName;
        private readonly string _arguments;

        public RunExtensionCommandExecutor(
            ILoggerService loggerService, IProcessService processService, string commandName, string arguments)
        {
            if (loggerService == null)
                throw new ArgumentNullException(nameof(loggerService));
            if (processService == null)
                throw new ArgumentNullException(nameof(processService));
            if (commandName == null)
                throw new ArgumentNullException(nameof(commandName));

            _loggerService = loggerService;
            _logger = loggerService.CreateLogger<RunExtensionCommandExecutor>();
            _processService = processService;
            _commandName = commandName;
            _arguments = arguments ?? string.Empty;
        }

        public async Task ExecuteAsync()
        {
            _logger.LogDebug(
                ExecutingCommandEventId, $"Executing extension '{_commandName}' with arguments '{_arguments}'");

            try
            {
                var result = await _processService.RunAsync(
                    _commandName, _arguments, WriteStandardOutput, WriteStandardError);
                _logger.LogDebug(
                    ExecutionSucceededEventId, 
                    $"Executing extension '{_commandName}' completed. Exit code: {result.ExitCode}");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ExecutionFailedEventId, 
                    $"Error while executing extension: '{_commandName}': {ex}");
                throw new CommandException("run", $"Cannot exeucte process '{_commandName}'", ex);
            }
        }

        private void WriteStandardOutput(string line)
        {
            lock (_consoleLock)
            {
                Console.WriteLine(line);
            }
        }

        private void WriteStandardError(string line)
        {
            lock (_consoleLock)
            {
                var originalColor = Console.ForegroundColor;
                try
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.Error.WriteLine(line);
                }
                finally
                {
                    Console.ForegroundColor = originalColor;
                }
            }
        }
    }
}