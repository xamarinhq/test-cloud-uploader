using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DocoptNet;
using Microsoft.AppHub.Cli;
using Microsoft.AppHub.Common;

namespace Microsoft.AppHub.TestCloud
{
    public class TestCloudCommand: ICommand
    {
        public string Name => "test-cloud";

        public string Summary => "Temporary command for Test Cloud connections";

        public string Syntax => $@"Command '{this.Name}': {this.Summary}. 

Usage:
    {ProgramUtilities.CurrentExecutableName} {this.Name} <appFile> <files>...
"; 

        public ICommandExecutor CreateCommandExecutor(IDictionary<string, ValueObject> options, IServiceProvider serviceProvider)
        {
            throw new NotImplementedException();
        }
    }

    public class TestCloudCommandExecutor: ICommandExecutor
    {
        private static readonly Uri _testCloudUri = new Uri("https://testcloud.xamarin.com/ci");

        private readonly string _appFile;
        private readonly IList<string> _files; 

        public Task ExecuteAsync()
        {
            throw new NotImplementedException();
        }
    }
}