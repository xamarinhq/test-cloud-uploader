using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using DocoptNet;
using Microsoft.Xtc.Common.Cli.Commands;
using Microsoft.Xtc.Common.Services;
using Microsoft.Xtc.TestCloud.Commands;
using Xunit;

namespace Microsoft.Xtc.TestCloud.Tests.Commands
{
    public class UploadTestsCommandOptionsTests
    {
        [Fact]
        public void AllOptionsShouldBeCorrectlyParsed()
        {
            var args = new[] 
            {
                "test",
                "testApp.apk", 
                "testApiKey",
                "--user", "testUser@xamarin.com",
                "--workspace", "c:\\TestWorkspace",
                "--app-name", "testApp",
                "--devices", "testDevices",
                "--async",
                "--async-json",
                "--locale", "pl_PL",
                "--series", "testSeries",
                "--dsym-directory", "c:\\TestDSymDirectory",
                "--test-parameters", "testKey1:testValue1:complex,testKey2:testValue2",
                "--debug"
            };
            
            var uploadOptions = ParseOptions(args);

            Assert.Equal("testApp.apk", uploadOptions.AppFile);
            Assert.Equal("testApiKey", uploadOptions.ApiKey);
            Assert.Equal("testUser@xamarin.com", uploadOptions.User);
            Assert.Equal("c:\\TestWorkspace", uploadOptions.Workspace);
            Assert.Equal("testApp", uploadOptions.AppName);
            Assert.Equal("testDevices", uploadOptions.Devices);
            Assert.True(uploadOptions.Async);
            Assert.True(uploadOptions.AsyncJson);
            Assert.Equal("pl_PL", uploadOptions.Locale);
            Assert.Equal("testSeries", uploadOptions.Series);
            Assert.Equal("c:\\TestDSymDirectory", uploadOptions.DSymDirectory);            
            Assert.True(uploadOptions.Debug);
            Assert.Equal(
                new[] 
                { 
                new KeyValuePair<string, string>("testKey1", "testValue1:complex"),
                    new KeyValuePair<string, string>("testKey2", "testValue2") 
                }, 
                uploadOptions.TestParameters.ToArray());
        }

        [Fact]
        public void DefaultValuesShouldBeCorrect()
        {
            var platformService = new PlatformService();
            string apkPath;

            if (platformService.CurrentPlatform == OSPlatform.Windows)
            {
                apkPath = "c:\\Temp\\TestApp.apk";
            }
            else
            {
                apkPath = "/tmp/app/TestApp.apk";
            }

            var args = new[] 
            {
                "test",
                apkPath, 
                "testApiKey",
                "--user", "testUser@xamarin.com",
                "--devices", "testDevices",
                "--workspace", "."
            };

            var uploadOptions = ParseOptions(args);

            Assert.Equal("en_US", uploadOptions.Locale);
            Assert.False(uploadOptions.Async);
            Assert.False(uploadOptions.AsyncJson);
            Assert.False(uploadOptions.Debug);
            Assert.Null(uploadOptions.Series);
            Assert.Null(uploadOptions.DSymDirectory);
            Assert.Null(uploadOptions.AppName);
        }

        [Fact]
        public void ValidationShouldFailWhenAppFileDoesntExist()
        {
            var args = new[] 
            {
                "test",
                "z:\\not_existing_app.apk", 
                "testApiKey",
                "--user", "testUser@xamarin.com",
                "--workspace", ".",
                "--devices", "testDevices"
            };

            var uploadOptions = ParseOptions(args);

            Assert.Throws<CommandException>(() => uploadOptions.Validate());
        }

        [Fact]
        public void ValidationShouldFailWhenWorkspaceDoesntExist()
        {
            var args = new[] 
            {
                "test",
                Assembly.GetEntryAssembly().Location, 
                "testApiKey",
                "--user", "testUser@xamarin.com",
                "--devices", "testDevices",
                "--workspace", "z:\\not_existing_workspace_directory"
            };

            var uploadOptions = ParseOptions(args);

            Assert.Throws<CommandException>(() => uploadOptions.Validate());
        }

        [Fact]
        public void ValidationShouldFailWhenDSymDirectoryDoesntExist()
        {
            var args = new[] 
            {
                "test",
                Assembly.GetEntryAssembly().Location, 
                "testApiKey",
                "--user", "testUser@xamarin.com",
                "--devices", "testDevices",
                "--workspace", ".",
                "--dsym-directory", "z:\\not_existing_dSym_directory"
            };

            var uploadOptions = ParseOptions(args);

            Assert.Throws<CommandException>(() => uploadOptions.Validate());
        }

        [Fact]
        public void ParsingShouldFailWhenUserIsMissing()
        {
            var args = new[] 
            {
                "test",
                Assembly.GetEntryAssembly().Location, 
                "testApiKey",
                "--devices", "testDevices",
            };

            Assert.Throws<DocoptInputErrorException>(() => ParseOptions(args));
        }

        [Fact]
        public void ParsingShouldFailWhenDeviceSelectionIsMissing()
        {
            var args = new[] 
            {
                "test",
                Assembly.GetEntryAssembly().Location, 
                "testApiKey",
                "--user", "testUser@xamarin.com@xamarin.com"
            };

            Assert.Throws<DocoptInputErrorException>(() => ParseOptions(args));
        }

        [Fact]
        public void ParsingShouldFailWhenWorkspaceIsMissing()
        {
            var args = new[] 
            {
                "test",
                Assembly.GetEntryAssembly().Location, 
                "testApiKey",
                "--user", "testUser@xamarin.com@xamarin.com",
                "--devices", "testDevices",
            };

            Assert.Throws<DocoptInputErrorException>(() => ParseOptions(args));
        }

        [Fact]
        public void ValidationShouldFailWhenTestParametersAreIncorrect()
        {
            var args = new[] 
            {
                "test",
                Assembly.GetEntryAssembly().Location, 
                "testApiKey",
                "--user", "testUser@xamarin.com",
                "--devices", "testDevices",
                "--workspace", ".",
                "--test-parameters", "testKey testValue"
            };

            var uploadOptions = ParseOptions(args);

            Assert.Throws<CommandException>(() => uploadOptions.Validate());
        }

        [Fact]
        public void ValidationShouldPassWhenAllRequiredOptionsAreCorrect()
        {
            var args = new[] 
            {
                "test",
                Assembly.GetEntryAssembly().Location, 
                "testApiKey",
                "--user", "testUser@xamarin.com",
                "--devices", "testDevices",
                "--workspace", ".",
                "--test-parameters", "testKey:testValue"
            };

            var uploadOptions = ParseOptions(args);
            uploadOptions.Validate();
        }

        [Fact]
        public void ValidationShouldFailWhenThereAreUnrecognizedOptions()
        {
            var args = new[] 
            {
                "test",
                "c:\\Temp\\testApp.apk", 
                "testApiKey",
                "--user", "testUser@xamarin.com",
                "--devices", "testDevices",
                "--unrecognized-option"
            };

            var command = new UploadTestsCommand();

            Assert.Throws<DocoptInputErrorException>(() => new Docopt().Apply(command.Syntax, args));
        }

        private UploadTestsCommandOptions ParseOptions(string[] args)
        {
            ICommand command = new UploadTestsCommand();
            var docoptOptions = new Docopt().Apply(command.Syntax, args);
            return new UploadTestsCommandOptions(docoptOptions);
        }
    }
}