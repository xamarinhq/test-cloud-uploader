using System;
using System.Collections.Generic;
using DocoptNet;

namespace Microsoft.AppHub.Cli
{
    /// <summary> 
    /// Describes a CLI command.
    /// </summary>
    public interface ICommandDescription
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
        /// Creates a command implmentations.
        /// </summary>
        /// <param name="options">Parsed Docopt.NET options.</param>
        /// <param name="serviceProvider">Service provider that can be used to access all dependencies.</param>
        ICommand CreateCommand(IDictionary<string, ValueObject> options, IServiceProvider serviceProvider);        
    }
}