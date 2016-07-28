using System;

namespace Microsoft.AppHub.Cli
{
    /// <summary>
    /// Exception that represents a failure in command execution. 
    /// </summary>
    public class CommandException : Exception
    {
        public CommandException(string commandName): base() 
        { 
            this.CommandName = commandName;
        }
        
        public CommandException(string commandName, string message) 
            : base(message) 
        {
            this.CommandName = commandName;
        }
        
        public CommandException(string commandName, string message, Exception inner) 
            : base(message, inner) 
	    { 
            this.CommandName = commandName;
        }

        /// <summary>
        /// Name of the command.
        /// </summary>
        public string CommandName { get; }
    }
}