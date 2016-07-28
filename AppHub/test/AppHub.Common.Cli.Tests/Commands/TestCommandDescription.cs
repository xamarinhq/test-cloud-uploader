using System;
using System.Collections.Generic;
using DocoptNet;

namespace Microsoft.AppHub.Cli.Tests
{
    public class TestCommandDescription : ICommandDescription
    {
        public TestCommandDescription()
        { }

        public TestCommandDescription(string name)
        {
            this.Name = name;
        }

        public string Name { get; set; }

        public string Summary { get; set; }

        public string Syntax { get; set; }
        
        public ICommand CreateCommand(IDictionary<string, ValueObject> options, IServiceProvider serviceProvider)
        {
            throw new NotImplementedException();
        }
    } 
}