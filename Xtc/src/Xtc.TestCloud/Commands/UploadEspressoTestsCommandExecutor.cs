using Microsoft.Extensions.Logging;
using Microsoft.Xtc.Common.Cli.Commands;
using Microsoft.Xtc.Common.Services;
using Microsoft.Xtc.Common.Services.Logging;
using Microsoft.Xtc.TestCloud.ObjectModel;
using Microsoft.Xtc.TestCloud.Services;
using Microsoft.Xtc.TestCloud.Utilities;

namespace Microsoft.Xtc.TestCloud.Commands
{
    /// <summary>
    /// Command executor that uploads Espresso tests to the Test Cloud.
    /// </summary>
    public class UploadEspressoTestsCommandExecutor : UploadJUnitTestsCommandExecutor
    {
        public UploadEspressoTestsCommandExecutor(UploadTestsCommandOptions options, ILoggerService loggerService, LogsRecorder logsRecorder) : base(options, loggerService, logsRecorder)
        {
            this.Logger = loggerService.CreateLogger<UploadEspressoTestsCommandExecutor>();
            this.TestName = "espresso";
            this.Workspace = new EspressoWorkspace(options.Workspace);
        }

        protected override void ValidateOptions()
        {
            if (ValidationHelper.IsIosApp(_options.AppFile)) 
            {
                throw new CommandException(
                    UploadTestsCommand.CommandName,
                    @"Application file must be an android application (.apk) file.",
                    (int) UploadCommandExitCodes.InvalidAppFile);
            } 
            base.ValidateOptions();
        }
    }
}