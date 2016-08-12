using System;
using System.Collections.Generic;
using DocoptNet;

namespace Microsoft.AppHub.Cli
{
    /// <summary> 
    /// Describes a CLI command.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Name of the command.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Quick description of the command.
        /// </summary>
        string Summary { get; }

        /// <summary>
        /// Syntax of the command, in a format that can be parsed by Docopt.NET.
        /// </summary>
        string Syntax { get; }
        
        /// <summary>
        /// Creates command executor.
        /// </summary>
        /// <param name="options">Parsed Docopt.NET options.</param>
        /// <param name="serviceProvider">Service provider that can be used to access all dependencies.</param>
        ICommandExecutor CreateCommandExecutor(IDictionary<string, ValueObject> options, IServiceProvider serviceProvider);        
    }
}