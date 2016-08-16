using System;
using System.Collections.Generic;
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
            
            var command = GetCommand(commandsRegistry, args);

            var options = ParseCommandOptions(args, command);
            var serviceProvider = services.BuildServiceProvider();
            
            try
            {
                command.ExecuteAsync(options, serviceProvider).Wait();
            }
            catch (Exception ex)
            {
                var logger = loggerService.CreateLogger<Program>();
                logger.LogError(ex.ToString());

                Console.Error.WriteLine(ex.Message);
            }
        }

        private static IDictionary<string, ValueObject> ParseCommandOptions(string[] args, ICommand command)
        {
            return new Docopt().Apply(command.Syntax, args);
        }

        private static ICommand GetCommand(CommandsRegistry registry, string[] args)
        {
            if (args.Length == 0)
                return registry.Commands[HelpCommandDescription.HelpCommandName];
            
            var commandName = args[0];
            ICommand result;
            if (!registry.Commands.TryGetValue(commandName, out result))
                result = registry.Commands[RunExtensionCommandDescription.RunExtensionCommandName];

            return result;
        }

        private static CommandsRegistry CreateCommandsRegistry()
        {
            var registry = new CommandsRegistry();
            registry.AddCommand(new HelpCommandDescription());
            registry.AddCommand(new RunExtensionCommandDescription());

            return registry;
        }
    }
}
