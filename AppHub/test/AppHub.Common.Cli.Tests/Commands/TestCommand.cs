using System;
using System.Collections.Generic;
using DocoptNet;

namespace Microsoft.AppHub.Cli.Tests
{
    public class TestCommand : ICommand
    {
        public TestCommand()
        { }

        public TestCommand(string name)
        {
            this.Name = name;
        }

        public string Name { get; set; }

        public string Summary { get; set; }

        public string Syntax { get; set; }
        
        public ICommandExecutor CreateCommandExecutor(IDictionary<string, ValueObject> options, IServiceProvider serviceProvider)
        {
            throw new NotImplementedException();
        }
    } 
}