using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using DocoptNet;
using Microsoft.AppHub.Common;
using Microsoft.AppHub.Cli.Commands;
using Microsoft.AppHub.TestCloud;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AppHub.Cli
{
    public class Program
    {
        public const string HelpCommandName = "help";
        public const string RunExtensionCommandName = "run";

        private static readonly Lazy<string> _executableName = new Lazy<string>(
            () => Path.GetFileName(typeof(Program).GetTypeInfo().Assembly.Location));
        
        public static string ExecutableName
        {
            get { return _executableName.Value; }
        }

        public static void Main(string[] args)
        {
            var commandsRegistry = CreateCommandsRegistry();
            var services = new ServiceCollection();
            
            services.AddSingleton<IProcessService, ProcessService>();
            services.AddSingleton(commandsRegistry);
            
            var commandDescription = FindCommandDescription(commandsRegistry, args);

            var options = ParseCommandOptions(args, commandDescription);           
	        var serviceProvider = services.BuildServiceProvider();
            var command = commandDescription.CreateCommand(options, serviceProvider);
            command.Execute();
        }

        private static IDictionary<string, ValueObject> ParseCommandOptions(string[] args, ICommandDescription commandDescription)
        {
            return new Docopt().Apply(commandDescription.Syntax, args);
        }

        private static ICommandDescription FindCommandDescription(CommandsRegistry registry, string[] args)
        {
            if (args.Length == 0)
                return registry.CommandDescriptions[HelpCommandName];
            
            var commandName = args[0];
            ICommandDescription result;
            if (!registry.CommandDescriptions.TryGetValue(commandName, out result))
                result = registry.CommandDescriptions[RunExtensionCommandName];

            return result;
        }

        private static CommandsRegistry CreateCommandsRegistry()
        {
            var registry = new CommandsRegistry();
            registry.AddCommandDescription(new HelpCommandDescription());
            registry.AddCommandDescription(new RunCommandDescription());

            return registry;
        }
    }
}
