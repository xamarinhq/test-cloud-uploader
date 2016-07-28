using System;
using System.Collections.Generic;
using DocoptNet;

namespace Microsoft.AppHub.Cli
{
    /// <summary> 
    /// Interface for all command line commands.
    /// </summary>
    public interface ICommandDescription
    {
        string Name { get; }
        string Summary { get; }
        string Syntax { get; }
        
        ICommand CreateCommand(IDictionary<string, ValueObject> options, IServiceProvider serviceProvider);        
    }
}