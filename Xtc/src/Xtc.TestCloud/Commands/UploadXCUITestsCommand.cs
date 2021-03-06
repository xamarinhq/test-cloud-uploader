using System;
using System.Collections.Generic;
using System.Text;
using DocoptNet;
using Microsoft.Xtc.Common.Cli.Commands;
using Microsoft.Xtc.Common.Cli.Utilities;
using Microsoft.Xtc.Common.Services.Logging;
using Microsoft.Xtc.TestCloud.Utilities;

namespace Microsoft.Xtc.TestCloud.Commands
{
    public class UploadXCUITestsCommand: ICommand
    {
        public const string CommandName = "xcuitest";

        public string Name => CommandName;

        public string Summary => "Upload xcuitests to the Test Cloud";

        public string Syntax => $@"Command '{this.Name}': {this.Summary}. 

Usage:
{GetPossibleOptionSyntax()}

Options: {UploadXCUITestsCommandOptions.OptionsDescription()}";

        private string GetPossibleOptionSyntax()
        {
            var result = new StringBuilder();

            foreach (var syntax in UploadXCUITestsCommandOptions.OptionsSyntax())
            {
                result.AppendFormat($"    {ProgramUtilities.CurrentExecutableName} {this.Name} {syntax}");
            }

            return result.ToString();
        }

        public ICommandExecutor CreateCommandExecutor(IDictionary<string, ValueObject> options, IServiceProvider serviceProvider)
        {
            var loggerService = (ILoggerService)serviceProvider.GetService(typeof(ILoggerService));
            var uploadOptions = new UploadXCUITestsCommandOptions(options);
            
            LogsRecorder logsRecorder = null;

            if (uploadOptions.AsyncJson)
            {
                logsRecorder = new LogsRecorder();
                loggerService.SetLoggerProvider(
                    new RecordingLoggerProvider(loggerService.MinimumLogLevel, logsRecorder));
            }

            if (WorkspaceHelper.IsXCUITestWorkspace(uploadOptions.Workspace))
            {
                return new UploadXCUITestCommandExecutor(uploadOptions, loggerService, logsRecorder);
            }
            else
            {
                throw new CommandException(
                    UploadTestsCommand.CommandName,
                    @"Unable to recognize workspace format",
                    (int) UploadCommandExitCodes.InvalidWorkspace);
            }
        }
    }
}