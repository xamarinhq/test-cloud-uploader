using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DocoptNet;
using Microsoft.AppHub.Common;
using Microsoft.AppHub.Cli.Commands;
using Microsoft.AppHub.TestCloud;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Microsoft.AppHub.Cli
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var services = new ServiceCollection();

	        var commandsRegistry = CreateCommandsRegistry();
            var loggerService = new LoggerService();
            
            services.AddSingleton<IProcessService, ProcessService>();
            services.AddSingleton(commandsRegistry);
            services.AddSingleton<ILoggerService>(loggerService);
            
            var commandDescription = GetCommandDescription(commandsRegistry, args);

            var options = ParseCommandOptions(args, commandDescription);           
	        var serviceProvider = services.BuildServiceProvider();
            var command = commandDescription.CreateCommand(options, serviceProvider);
            
            try
            {
                Task.Run(() => command.ExecuteAsync()).Wait();
            }
            catch (Exception ex)
            {
                var logger = loggerService.CreateLogger<Program>();
                logger.LogError(ex.ToString());

                Console.Error.WriteLine(ex.Message);
            }
        }

        private static IDictionary<string, ValueObject> ParseCommandOptions(string[] args, ICommandDescription commandDescription)
        {
            return new Docopt().Apply(commandDescription.Syntax, args);
        }

        private static ICommandDescription GetCommandDescription(CommandsRegistry registry, string[] args)
        {
            if (args.Length == 0)
                return registry.CommandDescriptions[HelpCommandDescription.HelpCommandName];
            
            var commandName = args[0];
            ICommandDescription result;
            if (!registry.CommandDescriptions.TryGetValue(commandName, out result))
                result = registry.CommandDescriptions[RunExtensionCommandDescription.RunExtensionCommandName];

            return result;
        }

        private static CommandsRegistry CreateCommandsRegistry()
        {
            var registry = new CommandsRegistry();
            registry.AddCommandDescription(new HelpCommandDescription());
            registry.AddCommandDescription(new RunExtensionCommandDescription());

            return registry;
        }
    }
}
