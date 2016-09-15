using System;
using System.Collections.Generic;
using System.Linq;
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
    public class UploadAppiumTestsCommandExecutor : ICommandExecutor
    {
        private const string TestCloudEndpointEnvironmentVariable = "XTC_ENDPOINT"; 

        public static readonly EventId PackagingFileEventId = 1;
        public static readonly EventId CheckHashResultEventId = 2;
        public static readonly EventId UploadTestsResultEventId = 3;
        public static readonly EventId CheckStatusResultEventId = 4;
        public static readonly EventId CustomEndpointEventId = 5;

        private static readonly TimeSpan _defaultWaitTime = TimeSpan.FromSeconds(10);
        private static readonly Uri _defaultTestCloudUri = new Uri("https://testcloud.xamarin.com/");

        private readonly UploadTestsCommandOptions _options;
        private readonly TestCloudProxy _testCloudProxy;
        private readonly ILogger _logger;

        private readonly LogsRecorder _logsRecorder;
        private readonly AppiumWorkspace _workspace;
        private readonly DSymDirectory _dSymDirectory;

        public UploadAppiumTestsCommandExecutor(
            UploadTestsCommandOptions options, ILoggerService loggerService, LogsRecorder logsRecorder)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            if (loggerService == null)
                throw new ArgumentNullException(nameof(loggerService));

            _options = options;
            _logsRecorder = logsRecorder;
            _logger = loggerService.CreateLogger<UploadAppiumTestsCommandExecutor>();
            
            var testCloudUri = GetTestCloudUri();
            _testCloudProxy = new TestCloudProxy(testCloudUri, loggerService);
            _workspace = new AppiumWorkspace(options.Workspace);
            _dSymDirectory = options.DSymDirectory != null ? new DSymDirectory(options.DSymDirectory) : null;
        }

        private Uri GetTestCloudUri()
        {
            var customEndpoint = Environment.GetEnvironmentVariable(TestCloudEndpointEnvironmentVariable);
            if (string.IsNullOrEmpty(customEndpoint))
            {
                return _defaultTestCloudUri;
            }

            _logger.LogDebug(
                CustomEndpointEventId,
                $"Environment variable {TestCloudEndpointEnvironmentVariable} was set. " + 
                $"Using custom Test Cloud endpoint URI: {customEndpoint}.");

            try
            {
                return new Uri(customEndpoint);
            }
            catch (UriFormatException)
            {
                throw new CommandException(
                    UploadTestsCommand.CommandName, 
                    $"Invalid custom Test Cloud endpoint URI: {customEndpoint}");
            }
        }

        public async Task ExecuteAsync()
        {
            ValidateOptions();
            await CheckVersionAsync();

            using (var sha256 = SHA256.Create())
            {
                var allFilesToUpload = GetAllFilesToUpload(sha256);
                var appFile = new UploadFileInfo(_options.AppFile, _options.AppFile, sha256.GetFileHash(_options.AppFile));
                var dSymFile = _dSymDirectory != null 
                    ? new UploadFileInfo(
                        _dSymDirectory.GetDSymFile(),
                        _dSymDirectory.GetDSymFile(),
                        sha256.GetFileHash(_dSymDirectory.GetDSymFile())) 
                    : null;

                var checkHashesResult = await CheckFileHashesAsync(appFile, dSymFile, allFilesToUpload);
                var uploadResult = await UploadTestsToTestCloud(checkHashesResult.AppFile, checkHashesResult.DSymFile, checkHashesResult.UploadFiles);

                if (!(_options.Async || _options.AsyncJson))
                {
                    var exitCode = await WaitForJob(uploadResult);
                    Environment.Exit(exitCode);
                }
                else if (_options.AsyncJson)
                {
                    WriteAsyncJsonResultToConsole(uploadResult.JobId);
                }
            }
        }

        private void ValidateOptions()
        {
            _options.Validate();

            if (ValidationHelper.IsAndroidApp(_options.AppFile))
            {
                if (ValidationHelper.UsesSharedRuntime(_options.AppFile))
                {
                    throw new CommandException(
                        UploadTestsCommand.CommandName,
                        @"Xamarin Test Cloud doesn't yet support shared runtime apps.
To test your app it needs to be compiled for release.
You can learn how to compile you app for release here: 
http://docs.xamarin.com/guides/android/deployment%2C_testing%2C_and_metrics/publishing_an_application/part_1_-_preparing_an_application_for_release",
                        (int) UploadCommandExitCodes.InvalidOptions);
                }
            }
            else if (!ValidationHelper.IsIosApp(_options.AppFile))
            {
                throw new CommandException(
                    UploadTestsCommand.CommandName,
                    @"Provided file with application must be either Android or iOS application",
                    (int) UploadCommandExitCodes.InvalidOptions);
            }

            _workspace.Validate();
            _dSymDirectory?.Validate();
        }

        private async Task CheckVersionAsync()
        {
            using (_logger.BeginScope("Checking version"))
            {
                var request = new CheckVersionRequest(_options.ToArgumentsArray());
                var result = await _testCloudProxy.CheckVersionAsync(request);

                if (result.ErrorMessage != null)
                {
                    throw new CommandException(UploadTestsCommand.CommandName, result.ErrorMessage);
                }
            }
        }

        private IList<UploadFileInfo> GetAllFilesToUpload(HashAlgorithm hashAlgorithm)
        {
            using (_logger.BeginScope("Packaging"))
            {
                var result = _workspace.GetUploadFiles(hashAlgorithm);

                foreach (var file in result)
                {
                    _logger.LogDebug(PackagingFileEventId, $"Packaging file {file.RelativePath}");
                }

                return result;
            }
        }

        private async Task<CheckHashesResult> CheckFileHashesAsync(
            UploadFileInfo appFile, UploadFileInfo dSymFile, IList<UploadFileInfo> allFilesToUpload)
        {
            using (_logger.BeginScope("Negotiating upload"))
            {
                var request = new CheckFileHashesRequest(appFile, dSymFile, allFilesToUpload);
                var result = await _testCloudProxy.CheckFileHashesAsync(request);
                LogCheckHashesResponse(result);

                return result;
            }
        }

        private async Task<UploadTestsResult> UploadTestsToTestCloud(
            UploadFileInfo appFile,
            UploadFileInfo dSymFile,
            IList<UploadFileInfo> otherFiles)
        {
            using (_logger.BeginScope("Uploading negotiated files"))
            {
                var request = new UploadTestsRequest(appFile, dSymFile, otherFiles);

                request.TestCloudOptions["user"] = _options.User;
                request.TestCloudOptions["device_selection"] = _options.Devices;
                request.TestCloudOptions["locale"] = _options.Locale;
                request.TestCloudOptions["appium"] = "true";
                request.TestCloudOptions["series"] = _options.Series;
                request.TestCloudOptions["api_key"] = _options.ApiKey;

                if (_options.AppName != null)
                {
                    request.TestCloudOptions["app"] = _options.AppName;
                }

                if (_dSymDirectory != null)
                {
                    request.TestCloudOptions["crash_reporting"] = "true";
                }

                foreach (var testParameter in _options.TestParameters)
                {
                    request.TestParameters[testParameter.Key] = testParameter.Value;
                }

                var result = await _testCloudProxy.UploadTestsAsync(request);
                LogUploadTestsResponse(result);

                return result;
            }
        }

        private async Task<int> WaitForJob(UploadTestsResult uploadTestsResult)
        {
            using (_logger.BeginScope("Waiting for test results"))
            {
                var checkStatusRequest = new CheckStatusRequest(uploadTestsResult.JobId)
                {
                    ApiKey = _options.ApiKey,
                    User = _options.User
                };

                while (true)
                {
                    var checkStatusResult = await _testCloudProxy.CheckStatusAsync(checkStatusRequest);
                    LogCheckStatusResponse(checkStatusResult);

                    if (checkStatusResult.ExitCode != null)
                    {
                        return checkStatusResult.ExitCode.Value;
                    }
                    else
                    {
                        var waitTime = checkStatusResult.WaitTime != null 
                            ? TimeSpan.FromSeconds(checkStatusResult.WaitTime.Value) 
                            : _defaultWaitTime;

                        await Task.Delay(waitTime);
                    }
                }
            }
        }

        private void LogCheckHashesResponse(CheckHashesResult response)
        {
            foreach (var result in response.UploadFiles.OrderBy(fileInfo => fileInfo.FullPath))
            {
                var relativePath = FileHelper.GetRelativePath(result.FullPath, _workspace.WorkspacePath, new PlatformService());
                _logger.LogDebug(
                    CheckHashResultEventId,
                    $"File {relativePath} was " +
                    (result.WasAlreadyUploaded ? "already uploaded." : "not uploaded."));
            }
        }

        private void LogUploadTestsResponse(UploadTestsResult response)
        {
            var logLines = new List<string>
            {
                "Tests enqueued",
                $"User: {response.UserEmail}"
            };

            if (response.Team != null)
            {
                logLines.Add($"Team: {response.Team}");
            }

            if (response.RejectedDevices != null && response.RejectedDevices.Count > 0)
            {
                logLines.Add($"Skipping devices (you can update your selections via https://testcloud.xamarin.com):");
                logLines.Add(GetDevicesListLog(response.RejectedDevices));
            }

            logLines.Add($"Running on devices:");
            logLines.Add(GetDevicesListLog(response.AcceptedDevices));

            _logger.LogInformation(UploadTestsResultEventId, logLines);
        }

        private void LogCheckStatusResponse(CheckStatusResult response)
        {
            LoggerExtensions.LogInformation(_logger, CheckStatusResultEventId, response.Messages.Select(m => $"{DateTimeOffset.UtcNow.ToString("s")} {m}"));
        }

        private void WriteAsyncJsonResultToConsole(string jobId)
        {
            var asyncJsonResult = new AsyncJsonResult()
            {
                TestRunId = jobId
            };

            var allLogs = _logsRecorder.GetRecordedLogs();
            asyncJsonResult.Logs = allLogs
                .Where(log => log.LogLevel <= LogLevel.Warning)
                .Select(log => _options.Debug ? log.ToDiagnosticString() : log.ToString())
                .ToList();

            asyncJsonResult.Errors = allLogs
                .Where(log => log.LogLevel > LogLevel.Warning)
                .Select(log => _options.Debug ? log.ToDiagnosticString() : log.ToString())
                .ToList();

            Console.WriteLine(JsonConvert.SerializeObject(asyncJsonResult));
        }

        private string GetDevicesListLog(IEnumerable<string> devices)
        {
            return devices.Aggregate(new StringBuilder(), (sb, d) => sb.AppendLine($"    {d}"), sb => sb.ToString());
        }
    }
}