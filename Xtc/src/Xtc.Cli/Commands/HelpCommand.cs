using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocoptNet;
using Microsoft.Xtc.Common;
using Microsoft.Xtc.Common.Cli.Commands;
using Microsoft.Xtc.Common.Cli.Utilities;

namespace Microsoft.Xtc.Cli.Commands
{
    public class HelpCommandDescription: ICommand
    {
        public const string HelpCommandName = "help";

        public string Name
        {
            get { return HelpCommandName; }
        }

        public string Summary
        {
            get { return "Print help for a command"; }
        }

        public string Syntax
        {
            get 
            {
                return $@"Command '{this.Name}': {this.Summary}.
                
Usage: 
  {ProgramUtilities.CurrentExecutableName} [{this.Name}]
  {ProgramUtilities.CurrentExecutableName} {this.Name} <command>
";
            }
        }

        public ICommandExecutor CreateCommandExecutor(IDictionary<string, ValueObject> options, IServiceProvider serviceProvider)
        {
            var commandName = options["<command>"];
            var commandsRegistry = (CommandsRegistry)serviceProvider.GetService(typeof(CommandsRegistry));
            
            if (commandName?.IsNullOrEmpty ?? true)
            {
                return new ListCommandsCommandExecutor(commandsRegistry);
            }
            else
            {
                return new PrintCommandSyntaxCommandExecutor(commandsRegistry, commandName.ToString());
            }
        }
    }

    public class ListCommandsCommandExecutor: ICommandExecutor
    {
        private readonly CommandsRegistry _commandsRegistry;

        public ListCommandsCommandExecutor(CommandsRegistry commandsRegistry)
        {
            if (commandsRegistry == null)
                throw new ArgumentNullException(nameof(commandsRegistry));

            _commandsRegistry = commandsRegistry;
        }

        public Task ExecuteAsync()
        {
            var allCommands = _commandsRegistry.Commands.Values.OrderBy(d => d.Name);

            Console.WriteLine($@"Usage: {ProgramUtilities.CurrentExecutableName} <command> [options]");
            Console.WriteLine("Available commands:");

            var longestLength = allCommands.Max(c => c.Name.Length);

            foreach (var commandDescription in allCommands)
            {
                Console.WriteLine(
                    $"  {commandDescription.Name.PadRight(longestLength + 3)} {commandDescription.Summary}");
            }

            return Tasks.Completed;
        }
    }

    public class PrintCommandSyntaxCommandExecutor: ICommandExecutor
    {
        private readonly ICommand _command;

        public PrintCommandSyntaxCommandExecutor(CommandsRegistry commandsRegistry, string commandName)
        {
            if (commandsRegistry == null)
                throw new ArgumentNullException(nameof(commandsRegistry));
            if (commandName == null)
                throw new ArgumentNullException(nameof(commandName));            
            
            if (!commandsRegistry.Commands.TryGetValue(commandName, out _command))
                throw new ArgumentException($"Unknown command {commandName}", nameof(commandName));
        }

        public Task ExecuteAsync()
        {
            Console.WriteLine(_command.Syntax);

            return Tasks.Completed;
        }
    }
}