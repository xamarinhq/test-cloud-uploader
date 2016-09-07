using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DocoptNet;

namespace Microsoft.AppHub.Common.Cli.Commands
{
    public static class CommandExtensions
    {
        /// <summary>
        /// Creates executor for a given command and executes it.
        /// </summary>
        /// <param name="command">Command</param>
        /// <param name="options">Parsed Docopt.NET options.</param>
        /// <param name="serviceProvider">Service provider that can be used to access all dependencies.</param>
        public static Task ExecuteAsync(
            this ICommand command,
            IDictionary<string, ValueObject> options, 
            IServiceProvider serviceProvider)
        {
            var executor = command.CreateCommandExecutor(options, serviceProvider);
            return executor.ExecuteAsync();
        }
    }
}