using System;
using System.Collections.Generic;
using System.Linq;
using DocoptNet;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AppHub.Cli.Commands
{
    public class HelpCommandDescription: ICommandDescription
    {
        public string Name
        {
            get { return "help"; }
        }

        public string Summary
        {
            get { return "Prints help for a command"; }
        }

        public string Syntax
        {
            get 
            {
                return $@"Command '{this.Name}': {this.Summary}.
                
Usage: 
  {ProgramHelper.CurrentExecutableName} {this.Name}
  {ProgramHelper.CurrentExecutableName} {this.Name} <command>

";
            }
        }

        public ICommand CreateCommand(IDictionary<string, ValueObject> options, IServiceProvider serviceProvider)
        {
            var commandName = options["<command>"];
            var commandsRegistry = (CommandsRegistry)serviceProvider.GetService(typeof(CommandsRegistry));
            if (commandName?.IsNullOrEmpty ?? true)
            {
                return new ListCommandsCommand(commandsRegistry);
            }
            else
            {
                return new PrintCommandSyntaxCommand(commandsRegistry, commandName.ToString());
            }
        }
    }

    public class ListCommandsCommand: ICommand
    {
        private readonly CommandsRegistry _commandsRegistry;

        public ListCommandsCommand(CommandsRegistry commandsRegistry)
        {
            if (commandsRegistry == null)
                throw new ArgumentNullException(nameof(commandsRegistry));

            _commandsRegistry = commandsRegistry; 
        }

        public void Execute()
        {
            var allCommandDescriptions = _commandsRegistry.CommandDescriptions.Values.OrderBy(d => d.Name);

            Console.WriteLine($@"Usage: {Program.ExecutableName} <command> [options])");
            Console.WriteLine("Available commands:");

            var longestLength = allCommandDescriptions.Max(c => c.Name.Length);

            foreach (var commandDescription in allCommandDescriptions)
            {
                Console.WriteLine($"  {commandDescription.Name.PadRight(longestLength + 3)} {commandDescription.Summary}");
            }
        }
    }

    public class PrintCommandSyntaxCommand: ICommand
    {
        private readonly ICommandDescription _commandDescription;

        public PrintCommandSyntaxCommand(CommandsRegistry commandsRegistry, string commandName)
        {
            if (commandsRegistry == null)
                throw new ArgumentNullException(nameof(commandsRegistry));
            if (commandName == null)
                throw new ArgumentNullException(nameof(commandName));            
            
            if (!commandsRegistry.CommandDescriptions.TryGetValue(commandName, out _commandDescription))
                throw new ArgumentException($"Unknown command {commandName}", nameof(commandName));
        }

        public void Execute()
        {
            Console.WriteLine(_commandDescription.Syntax);
        }
    }
}