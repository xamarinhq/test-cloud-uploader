using System;
using System.Collections.Generic;
using System.Linq;
using DocoptNet;
using Microsoft.Xtc.Common.Cli.Commands;
using Microsoft.Xtc.Common.Services;
using Microsoft.Xtc.Common.Services.Logging;
using Microsoft.Xtc.TestCloud.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Xtc.Cli.Commands;
using System.Runtime.InteropServices;

namespace Microsoft.Xtc.Cli
{
    public class Program
    {
        public const int DefaultErrorExitCode = 3;

        private const string OsXNoOpenSslErrorMessage = @"In order to use this command, you first need to install the latest version of OpenSSL.
The easiest way to get this is by using one of package managers.
If your are using Homebrew (http://brew.sh), execute this command:
    brew install openssl

If you are using MacPorts (https://www.macports.org/), execute this command:
    sudo port install openssl
";

        public static void Main(string[] args)
        {
            var services = new ServiceCollection();

            var commandsRegistry = CreateCommandsRegistry();

            var logLevel = args.Any(a => a == "--debug") ? LogLevel.Debug : LogLevel.Information;
            var loggerService = new LoggerService(logLevel);
            var platformService = new PlatformService();
            
            services.AddSingleton<IProcessService, ProcessService>();
            services.AddSingleton(commandsRegistry);
            services.AddSingleton<ILoggerService>(loggerService);
            services.AddSingleton<IPlatformService>(platformService);
            var command = GetCommand(commandsRegistry, args);

            try
            {
                var serviceProvider = services.BuildServiceProvider();
                var options = ParseCommandOptions(args, command);
                command.ExecuteAsync(options, serviceProvider).Wait();
            }
            catch (Exception ex)
            {
                var logger = loggerService.CreateLogger<Program>();

                ExceptionsHandler(command, logger, platformService, ex);
            }
        }

        private static void ExceptionsHandler(ICommand command, ILogger logger, IPlatformService platformService, Exception ex)
        {
            if (ex is AggregateException)
            {
                var typedException = (AggregateException)ex;
                ExceptionsHandler(command, logger, platformService, ex.InnerException);
            }
            else if (ex is CommandException)
            {                
                var typedException = (CommandException)ex;
                logger.LogError(typedException.Message);
                Environment.Exit(typedException.ExitCode ?? DefaultErrorExitCode);
            }
            else if (ex is DocoptInputErrorException)
            {
                logger.LogError(
                    $"Invalid arguments for command \"{command.Name}\".{Environment.NewLine}{command.Syntax}");
                Environment.Exit(DefaultErrorExitCode);
            }
            else if (ex is TypeInitializationException && 
                     ex.ToString().Contains("System.Security.Cryptography.Native") && 
                     platformService.CurrentPlatform == OSPlatform.OSX)
            {
                logger.LogError(OsXNoOpenSslErrorMessage);
                Environment.Exit(DefaultErrorExitCode);                
            }
            else
            {
                logger.LogError($"Error while executing command {command.Name}: {ex.Message}");
                logger.LogDebug(ex.ToString());
                Environment.Exit(DefaultErrorExitCode);
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
