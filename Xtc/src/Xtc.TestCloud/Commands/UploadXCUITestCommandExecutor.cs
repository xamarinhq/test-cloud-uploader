using Microsoft.Extensions.Logging;
using Microsoft.Xtc.Common.Services.Logging;
using Microsoft.Xtc.TestCloud.ObjectModel;
using Microsoft.Xtc.TestCloud.Utilities;
using Microsoft.Xtc.Common.Cli.Commands;

namespace Microsoft.Xtc.TestCloud.Commands
{
    /// <summary>
    /// Command executor that uploads XCUITest tests to the Test Cloud.
    /// </summary>
    public class UploadXCUITestCommandExecutor : UploadXTCTestCommandExecutor
    {
        public UploadXCUITestCommandExecutor(UploadTestsCommandOptions options, ILoggerService loggerService, LogsRecorder logsRecorder) : base(options, loggerService, logsRecorder)
        {
            this.TestName = "XCUITest";
            this.Workspace = new XCUITestWorkspace(options.Workspace);
        }

        protected override void ValidateOptions()
        {
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