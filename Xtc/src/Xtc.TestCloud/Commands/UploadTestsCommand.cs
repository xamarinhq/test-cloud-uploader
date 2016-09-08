using System;
using System.Collections.Generic;
using System.Text;
using DocoptNet;
using Microsoft.Xtc.Common.Cli.Commands;
using Microsoft.Xtc.Common.Cli.Utilities;
using Microsoft.Xtc.Common.Services.Logging;

namespace Microsoft.Xtc.TestCloud.Commands
{
    public class UploadTestsCommand: ICommand
    {
        public const string CommandName = "test";

        public string Name => CommandName;

        public string Summary => "Upload tests to the Test Cloud";

        public string Syntax => $@"Command '{this.Name}': {this.Summary}. 

Usage:
{GetPossibleOptionSyntax()}

Options: {UploadTestsCommandOptions.OptionsDescription}";

        private string GetPossibleOptionSyntax()
        {
            var result = new StringBuilder();

            foreach (var syntax in UploadTestsCommandOptions.OptionsSyntax)
            {
                result.AppendFormat($"    {ProgramUtilities.CurrentExecutableName} {this.Name} {syntax}");
            }

            return result.ToString();
        }

        public ICommandExecutor CreateCommandExecutor(IDictionary<string, ValueObject> options, IServiceProvider serviceProvider)
        {
            var uploadOptions = new UploadTestsCommandOptions(options);
            
            LogsRecorder logsRecorder = null;
            var loggerService = (ILoggerService)serviceProvider.GetService(typeof(ILoggerService));

            if (uploadOptions.AsyncJson)
            {
                logsRecorder = new LogsRecorder();
                loggerService.SetLoggerProvider(
                    new RecordingLoggerProvider(loggerService.MinimumLogLevel, logsRecorder));
            }

            return new UploadAppiumTestsCommandExecutor(uploadOptions, loggerService, logsRecorder);
        }
    }
}