using System;
using System.Collections.Generic;

namespace Microsoft.AppHub.Cli
{
    public class CommandsRegistry
    {
        private readonly Dictionary<string, ICommandDescription> _commandDescriptions;

        public CommandsRegistry()
        {
            _commandDescriptions = new Dictionary<string, ICommandDescription>();
        }

        public void AddCommandDescription(ICommandDescription commandDescription)
        {
            if (commandDescription == null)
                throw new ArgumentNullException(nameof(commandDescription));

            _commandDescriptions.Add(commandDescription.Name, commandDescription);        
        }

        public IReadOnlyDictionary<string, ICommandDescription> CommandDescriptions
        {
            get { return _commandDescriptions; }
        }
    }
}