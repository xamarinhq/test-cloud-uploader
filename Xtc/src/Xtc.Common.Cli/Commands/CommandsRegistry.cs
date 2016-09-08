using System;
using System.Collections.Generic;

namespace Microsoft.Xtc.Common.Cli.Commands
{
    /// <summary>
	/// Registry that stores command descriptions. 
    /// </summary>
    public class CommandsRegistry
    {
        private readonly Dictionary<string, ICommand> _commands;

        public CommandsRegistry()
        {
            _commands = new Dictionary<string, ICommand>();
        }

        /// <summary>
        /// Adds new command description.
        /// </summary>
        public void AddCommand(ICommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            _commands.Add(command.Name, command);        
        }

        /// <summary>
        /// A read-only dictionary with all registered commands. Keys in the dictionary are command names, 
        /// values are command descriptions (implementations of ICommandDescription).
        /// </summary>
        public IReadOnlyDictionary<string, ICommand> Commands
        {
            get { return _commands; }
        }
    }
}