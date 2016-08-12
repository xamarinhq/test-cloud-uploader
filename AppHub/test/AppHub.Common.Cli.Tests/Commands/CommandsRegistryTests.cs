﻿using System;
using Xunit;

namespace Microsoft.AppHub.Cli.Tests
{
    public class CommandsRegistryTests
    {
        [Fact]
        public void CommandDescriptionsContainAllAddedItems()
        {
            var registry = new CommandsRegistry();
            var description = new TestCommand("foo");
            registry.AddCommand(description);

            Assert.Same(description, registry.Commands["foo"]);
        }

        [Fact]
        public void CannotAddCommandsWithDuplicateName() 
        {
            var registry = new CommandsRegistry();
            registry.AddCommand(new TestCommand("foo"));

            Assert.Throws<ArgumentException>(() => registry.AddCommand(new TestCommand("foo")));
        }
    }
}
