using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DocoptNet;
using Microsoft.AppHub.Cli;
using Microsoft.AppHub.Common;

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

            var processHelper = (IProcessService)serviceProvider.GetService(typeof(IProcessService));

            return new RunExtensionCommand(processHelper, commandName, arguments);
        }
    }

    public class RunExtensionCommand: ICommand
    {
        private readonly IProcessService _processService;
        private readonly string _commandName;
        private readonly string _arguments;

        public RunExtensionCommand(IProcessService processService, string commandName, string arguments)
        {
            if (processService == null)
                throw new ArgumentNullException(nameof(processService));
            if (commandName == null)
                throw new ArgumentNullException(nameof(commandName));

            _processService = processService;
            _commandName = commandName;
            _arguments = arguments ?? string.Empty;
        }

        public async Task ExecuteAsync()
        {
            var result = _processService.Run(_commandName, _arguments);
            
            Console.WriteLine(result.StandardOutput);
            Console.Error.WriteLine(result.StandardError);
        }
    }
}