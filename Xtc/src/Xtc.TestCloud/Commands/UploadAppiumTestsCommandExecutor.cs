using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Xtc.Common.Cli.Commands;
using Microsoft.Xtc.Common.Services;
using Microsoft.Xtc.Common.Services.Logging;
using Microsoft.Xtc.TestCloud.ObjectModel;
using Microsoft.Xtc.TestCloud.Services;
using Microsoft.Xtc.TestCloud.Utilities;
using Newtonsoft.Json;
using LoggerExtensions = Microsoft.Xtc.Common.Services.Logging.LoggerExtensions;

namespace Microsoft.Xtc.TestCloud.Commands
{
    /// <summary>
    /// Command executor that uploads Appium tests to the Test Cloud.
    /// </summary>
    public class UploadAppiumTestsCommandExecutor : UploadXTCTestCommandExecutor
    {
        public UploadAppiumTestsCommandExecutor(
            UploadTestsCommandOptions options, ILoggerService loggerService, LogsRecorder logsRecorder) : base(options, loggerService, logsRecorder)
        {
            this.TestName = "appium";
            this.Workspace = new AppiumWorkspace(options.Workspace);
        }
    }
}