using System;
using System.Collections.Generic;
using DocoptNet;
using Microsoft.AppHub.Cli;
using Microsoft.AppHub.Common;

namespace Microsoft.AppHub.TestCloud
{
    public class UploadTestsCommand: ICommand
    {
        public string Name => "upload-tests";

        public string Summary => "Temporary command for Test Cloud connections";

        public string Syntax => $@"Command '{this.Name}': {this.Summary}. 

Usage:
    {ProgramUtilities.CurrentExecutableName} {this.Name} <app-file> <api-key> [options]

Options: {UploadTestsCommandOptions.OptionsSyntax}";     

        public ICommandExecutor CreateCommandExecutor(IDictionary<string, ValueObject> options, IServiceProvider serviceProvider)
        {
            var uploadOptions = new UploadTestsCommandOptions(options);

            return new UploadAppiumTestsCommandExecutor(uploadOptions, (ILoggerService)serviceProvider.GetService(typeof(ILoggerService)));
        }
    }
}