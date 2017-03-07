using Microsoft.Xtc.Common.Services.Logging;
using Microsoft.Xtc.TestCloud.ObjectModel;
using Microsoft.Xtc.TestCloud.Utilities;
using Microsoft.Xtc.Common.Cli.Commands;
using System.IO;
using System;

namespace Microsoft.Xtc.TestCloud.Commands
{
    /// <summary>
    /// Command executor that uploads XCUITest tests to the Test Cloud.
    /// </summary>
    public class UploadXCUITestCommandExecutor : UploadXTCTestCommandExecutor
    {
        public UploadXCUITestCommandExecutor(UploadTestsCommandOptions options, ILoggerService loggerService, LogsRecorder logsRecorder) : base(options, loggerService, logsRecorder)
        {
            this.TestName = "xcuitest";
            this.Workspace = new XCUITestWorkspace(options.Workspace);
            
            if (_options.AppFile == null) {
                var appBundle = XCUITestWorkspace.FindAUT(options.Workspace);
                _options.AppFile = FileHelper.ArchiveAppBundle(appBundle);
            }
        }

        protected override void ValidateOptions()
        {
            if (_options.AppFile == null)
            {
                throw new CommandException(
                    UploadXCUITestsCommand.CommandName,
                    $"Appfile not provided and not found in workspace {_options.Workspace}",
                    (int)UploadCommandExitCodes.MissingAppFile);
            }
            
            if (!File.Exists(_options.AppFile))
            {
                throw new CommandException(
                    UploadTestsCommand.CommandName, 
                    $"Cannot find app file: {_options.AppFile}",
                    (int)UploadCommandExitCodes.InvalidAppFile);
            }

            if (ValidationHelper.IsAndroidApp(_options.AppFile)) 
            {
                throw new CommandException(
                    UploadTestsCommand.CommandName,
                    @"Application file must be an iOS application (.ipa) file.",
                    (int) UploadCommandExitCodes.InvalidAppFile);
            } 
            base.ValidateOptions();
        }
    }
}