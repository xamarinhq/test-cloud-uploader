using System;
using System.Collections.Generic;
using System.Text;
using DocoptNet;
using Microsoft.AppHub.Cli;
using Microsoft.AppHub.Common;
using Microsoft.Extensions.Logging;

namespace Microsoft.AppHub.TestCloud
{
    public class UploadTestsCommand : ICommand
    {
        public const string CommandName = "upload-tests";

        public static readonly EventId FrameworkDetectedEventId = 1;
        public static readonly EventId FrameworkNotDetectedEventId = 2;

        public string Name => CommandName;

        public string Summary => "Upload tests to the Test Cloud";

        public string Syntax
        {
            get
            {
                var result = new StringBuilder();
                result.AppendLine($"Command '{this.Name}': {this.Summary}.");
                result.AppendLine();

                result.AppendLine("Usage:");
                foreach (var syntax in UploadTestsCommandOptions.OptionsSyntax)
                {
                    result.AppendLine($"    {ProgramUtilities.CurrentExecutableName} {this.Name} {syntax}");
                }
                result.AppendLine();

                result.AppendLine("Options:");
                result.Append(UploadTestsCommandOptions.OptionsDescription);

                return result.ToString();
            }
        }

        public ICommandExecutor CreateCommandExecutor(IDictionary<string, ValueObject> options,
            IServiceProvider serviceProvider)
        {
            var uploadOptions = new UploadTestsCommandOptions(options);

            LogsRecorder logsRecorder = null;
            var loggerService = (ILoggerService) serviceProvider.GetService(typeof (ILoggerService));

            if (uploadOptions.AsyncJson)
            {
                logsRecorder = new LogsRecorder();
                loggerService.SetLoggerProvider(
                    new RecordingLoggerProvider(loggerService.MinimumLogLevel, logsRecorder));
            }

            var logger = loggerService.CreateLogger<UploadTestsCommand>();
            uploadOptions.Validate();

            using (logger.BeginScope("Detecting test framework"))
            {
                var testFramework = TestFrameworkDetector.Instance.DetectTestFramework(uploadOptions.Workspace);

                if (testFramework == TestFramework.Appium)
                {
                    logger.LogInformation(FrameworkDetectedEventId, "Detected Appium tests in the workspace.");
                    return new UploadAppiumTestsCommandExecutor(uploadOptions, loggerService, logsRecorder);
                }
                else
                {
                    logger.LogCritical(FrameworkNotDetectedEventId, "Cannot detect test framework for the workspace");
                    throw new CommandException(
                        CommandName,
                        $"Workspace folder \"{uploadOptions.Workspace}\" doesn't " +
                        "contain tests from any of supported test frameworks");
                }
            }
        }
    }
}