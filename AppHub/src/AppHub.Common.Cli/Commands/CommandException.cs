using System;

namespace Microsoft.AppHub.Common.Cli.Commands
{
    /// <summary>
    /// Exception that represents a failure in command execution. 
    /// </summary>
    public class CommandException : Exception
    {
        public CommandException(string commandName, int? exitCode = null)
        { 
            this.CommandName = commandName;
            this.ExitCode = exitCode;
        }
        
        public CommandException(string commandName, string message, int? exitCode = null) 
            : base(message) 
        {
            this.CommandName = commandName;
            this.ExitCode = exitCode;
        }
        
        public CommandException(string commandName, string message, Exception inner, int? exitCode = null) 
            : base(message, inner) 
	    { 
            this.CommandName = commandName;
            this.ExitCode = exitCode;
        }

        /// <summary>
        /// Name of the command.
        /// </summary>
        public string CommandName { get; }

        /// <summary>
        /// Exit code that represents this exception.
        /// </summary>
        public int? ExitCode { get; }
    }
}