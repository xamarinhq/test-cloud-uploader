using System;
using System.Collections.Generic;
using DocoptNet;
using Microsoft.AppHub.Cli;
using Microsoft.AppHub.Common;

namespace Microsoft.AppHub.TestCloud
{
    public class UploadTestsCommand: ICommand
    {
        public const string CommandName = "upload-tests";

        public string Name => CommandName;

        public string Summary => "Temporary command for Test Cloud connections";

        public string Syntax => $@"Command '{this.Name}': {this.Summary}. 

Usage:
    {ProgramUtilities.CurrentExecutableName} {this.Name} <app-file> <api-key> [options]

Options: {UploadTestsCommandOptions.OptionsSyntax}";     

        public ICommandExecutor CreateCommandExecutor(IDictionary<string, ValueObject> options, IServiceProvider serviceProvider)
        {
            var uploadOptions = new UploadTestsCommandOptions(options);
            
            RecordedLogs recordedLogs = null;
            var loggerService = (ILoggerService)serviceProvider.GetService(typeof(ILoggerService));

            if (uploadOptions.AsyncJson)
            {
                recordedLogs = new RecordedLogs();
                loggerService.SetLoggerProvider(
                    new RecordingLoggerProvider(loggerService.MinimumLogLevel, recordedLogs));
            }

            return new UploadAppiumTestsCommandExecutor(uploadOptions, loggerService, recordedLogs);
        }
    }
}