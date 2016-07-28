using System;
using System.Collections.Generic;

namespace Microsoft.AppHub.Cli
{
    /// <summary>
	/// Registry that stores command descriptions. 
    /// </summary>
    public class CommandsRegistry
    {
        private readonly Dictionary<string, ICommandDescription> _commandDescriptions;

        public CommandsRegistry()
        {
            _commandDescriptions = new Dictionary<string, ICommandDescription>();
        }

        /// <summary>
        /// Adds new command description.
        /// </summary>
        public void AddCommandDescription(ICommandDescription commandDescription)
        {
            if (commandDescription == null)
                throw new ArgumentNullException(nameof(commandDescription));

            _commandDescriptions.Add(commandDescription.Name, commandDescription);        
        }

        /// <summary>
        /// A read-only dictionary with all registered commands. Keys in the dictionary are command names, 
        /// values are command descriptions (implementations of ICommandDescription).
        /// </summary>
        public IReadOnlyDictionary<string, ICommandDescription> CommandDescriptions
        {
            get { return _commandDescriptions; }
        }
    }
}