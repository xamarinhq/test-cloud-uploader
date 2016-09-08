using System;
using System.Collections.Generic;
using System.Linq;
using DocoptNet;
using Microsoft.Xtc.Common;
using Microsoft.Xtc.Common.Cli.Commands;
using Microsoft.Xtc.Common.Services;
using Microsoft.Xtc.Common.Services.Logging;
using Microsoft.Xtc.TestCloud;
using Microsoft.Xtc.TestCloud.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Xtc.Cli.Commands;

namespace Microsoft.Xtc.Cli
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var services = new ServiceCollection();

            var commandsRegistry = CreateCommandsRegistry();

            var logLevel = args.Any(a => a == "--debug") ? LogLevel.Debug : LogLevel.Information;
            var loggerService = new LoggerService(logLevel);
            
            services.AddSingleton<IProcessService, ProcessService>();
            services.AddSingleton(commandsRegistry);
            services.AddSingleton<ILoggerService>(loggerService);
            
            var command = GetCommand(commandsRegistry, args);
            var serviceProvider = services.BuildServiceProvider();

            try
            {
                var options = ParseCommandOptions(args, command);
                command.ExecuteAsync(options, serviceProvider).Wait();
            }
            catch (Exception ex)
            {
                var logger = loggerService.CreateLogger<Program>();
                
                logger.LogError(ex.Message);
                logger.LogError(ex.ToString());
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
            {
                result = registry.Commands[RunExtensionCommandDescription.RunExtensionCommandName];
            }

            return result;
        }

        private static CommandsRegistry CreateCommandsRegistry()
        {
            var registry = new CommandsRegistry();
            registry.AddCommand(new HelpCommandDescription());
            registry.AddCommand(new RunExtensionCommandDescription());
            registry.AddCommand(new UploadTestsCommand());

            return registry;
        }
    }
}
