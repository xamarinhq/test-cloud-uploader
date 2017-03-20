using DocoptNet;
using Microsoft.Xtc.Common.Cli.Commands;
using Microsoft.Xtc.TestCloud.Commands;
using Xunit;

namespace Microsoft.Xtc.TestCloud.Tests.Commands
{
    public class UploadXCUITestsCommandOptionsTests
    {
        [Fact]
        public void ValidationShouldFailWhenSpecifiedAppDoesNotExist() 
        {
            var args = new[] 
            {
                "xcuitest",
                "testApiKey",
                "--app-file", "someapp.ipa",
                "--user", "testUser@xamarin.com",
                "--devices", "testDevices",
                "--workspace", "."
            };

            var uploadOptions = ParseOptions(args);
            Assert.Throws<CommandException>(() => uploadOptions.Validate());
        }

        [Fact]
        public void ValidationShouldFailWhenAppsNotFoundInWorkspace() 
        {
            var args = new[] 
            {
                "xcuitest",
                "testApiKey",
                "--user", "testUser@xamarin.com",
                "--devices", "testDevices",
                "--workspace", "."
            };

            var uploadOptions = ParseOptions(args);

            Assert.Throws<CommandException>(() => uploadOptions.Validate());
        }

        private UploadTestsCommandOptions ParseOptions(string[] args)
        {
            ICommand command = new UploadXCUITestsCommand();
            var docoptOptions = new Docopt().Apply(command.Syntax, args);
            return new UploadXCUITestsCommandOptions(docoptOptions);
        }
    }
}