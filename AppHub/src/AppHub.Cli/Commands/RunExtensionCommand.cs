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
    public class RunCommandDescription: ICommandDescription
    {
        public string Name
        {
            get { return "run"; }
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

        public ICommand CreateCommand(IDictionary<string, ValueObject> options, IServiceProvider serviceProvider)
        {
            var commandName = $"app-{options["<command>"].ToString()}";
            var arguments = options["<arguments>"]?
                .AsList
                .Cast<object>()
                .Select(a => a.ToString())
                .Aggregate(new StringBuilder(), (sb, a) => sb.Append($"{a} "), sb => sb.ToString());

            var processService = (IProcessService)serviceProvider.GetService(typeof(IProcessService));
            var loggerService = (ILoggerService)serviceProvider.GetService(typeof(ILoggerService));

            return new RunExtensionCommand(loggerService, processService, commandName, arguments);
        }
    }

    public class RunExtensionCommand: ICommand
    {
        private readonly ILogger _logger;
        private readonly ILoggerService _loggerService;
        private readonly IProcessService _processService;
        private readonly string _commandName;
        private readonly string _arguments;

        public RunExtensionCommand(ILoggerService loggerService, IProcessService processService, string commandName, string arguments)
        {
            if (loggerService == null)
                throw new ArgumentNullException(nameof(loggerService));
            if (processService == null)
                throw new ArgumentNullException(nameof(processService));
            if (commandName == null)
                throw new ArgumentNullException(nameof(commandName));

            _loggerService = loggerService;
            _logger = loggerService.CreateLogger<RunExtensionCommand>();
            _processService = processService;
            _commandName = commandName;
            _arguments = arguments ?? string.Empty;
        }

        public Task ExecuteAsync()
        {
            var eventId = _loggerService.CreateEventId();
            _logger.LogDebug(eventId, $"Executing extension '{_commandName}' with arguments '{_arguments}'");

            try
            {
                var result = _processService.Run(_commandName, _arguments);
                Console.WriteLine(result.StandardOutput);
                Console.Error.WriteLine(result.StandardError);

                _logger.LogDebug(eventId, $"Executing extension '{_commandName}' completed. Exit code: {result.ExitCode}");
            }
            catch (Exception ex)
            {
                _logger.LogError(eventId, $"Error while executing extension: '{_commandName}': {ex}");
                throw new CommandException("run", $"Cannot exeucte process '{_commandName}'", ex);
            }

            return Tasks.Done;
        }
    }
}