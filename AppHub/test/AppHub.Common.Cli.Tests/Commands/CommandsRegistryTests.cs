using System;
using Xunit;

namespace Microsoft.AppHub.Cli.Tests
{
    public class CommandsRegistryTests
    {
        [Fact]
        public void CommandDescriptionsContainAllAddedItems()
        {
            var registry = new CommandsRegistry();
            var description = new TestCommandDescription("foo");
            registry.AddCommandDescription(description);

            Assert.Same(description, registry.CommandDescriptions["foo"]);
        }

        [Fact]
        public void CannotAddCommandsWithDuplicateName() 
        {
            var registry = new CommandsRegistry();
            registry.AddCommandDescription(new TestCommandDescription("foo"));

            Assert.Throws<ArgumentException>(() => registry.AddCommandDescription(new TestCommandDescription("foo")));
        }
    }
}
