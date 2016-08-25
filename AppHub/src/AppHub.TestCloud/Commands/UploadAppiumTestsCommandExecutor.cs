using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AppHub.Cli;
using Microsoft.AppHub.Common;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Microsoft.AppHub.TestCloud
{
    /// <summary>
    /// Command executor that uploads Appium tests to the Test Cloud.
    /// </summary>
    public class UploadAppiumTestsCommandExecutor: ICommandExecutor
    {
        public static readonly EventId PackagingFileEventId = 1;
        public static readonly EventId CheckHashResultEventId = 2;
        public static readonly EventId UploadTestsResultEventId = 3;
        public static readonly EventId CheckStatusResultEventId = 4;

        private static readonly TimeSpan DefaultWaitTime = TimeSpan.FromSeconds(10); 
        private static readonly Uri _testCloudUri = new Uri("https://testcloud.xamarin.com/ci");

        private readonly UploadTestsCommandOptions _options;
        private readonly TestCloudProxy _testCloudProxy;
        private readonly ILoggerService _loggerService;
        private readonly ILogger _logger;

        private readonly RecordedLogs _recordedLogs;

        public UploadAppiumTestsCommandExecutor(
            UploadTestsCommandOptions options, ILoggerService loggerService, RecordedLogs recordedLogs)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            if (loggerService == null)
                throw new ArgumentNullException(nameof(loggerService));

            _options = options;
            _loggerService = loggerService;
            _recordedLogs = recordedLogs;
            _logger = loggerService.CreateLogger<UploadAppiumTestsCommandExecutor>();
            _testCloudProxy = new TestCloudProxy(_testCloudUri, loggerService);
        }

        public async Task ExecuteAsync()
        {
            ValidateOptions();
            await CheckVersionAsync();

            var allFilesToUpload = GetAllFilesToUpload();
            var checkHashesResult = await CheckFileHashesAsync(_options.AppFile, null, allFilesToUpload);
            var uploadResult = await UploadTestsToTestCloud(
                _options.AppFile, null, _options.Workspace, allFilesToUpload, checkHashesResult);

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

        private void ValidateOptions()
        {
            _options.Validate();

            if (!ValidationHelper.IsAndroidApp(_options.AppFile))
            {
                if (ValidationHelper.UsesSharedRuntime(_options.AppFile))
                {
                    throw new CommandException(
                        UploadTestsCommand.CommandName, 
@"Xamarin Test Cloud doesn't yet support shared runtime apps.
To test your app it needs to be compiled for release.
You can learn how to compile you app for release here: 
http://docs.xamarin.com/guides/android/deployment%2C_testing%2C_and_metrics/publishing_an_application/part_1_-_preparing_an_application_for_release",
                        (int)UploadCommandExitCodes.InvalidOptions);
                }
            }
        }

        private async Task CheckVersionAsync()
        {
            using (var scope = _logger.BeginScope("Checking version"))
            {
                var request = new CheckVersionRequest(_options.ToArgumentsArray());
                var result = await _testCloudProxy.CheckVersionAsync(request);

                if (result.ErrorMessage != null)
                    throw new CommandException(UploadTestsCommand.CommandName, result.ErrorMessage);
            }
        }

        private IList<string> GetAllFilesToUpload()
        {
            using (var scope = _logger.BeginScope("Packaging"))
            {
                var result = Directory.GetFiles(_options.Workspace, "*", SearchOption.AllDirectories).ToList();

                foreach (var file in result)
                {
                    var relativePath = FileHelper.GetRelativePath(file, _options.Workspace);
                    _logger.LogDebug(PackagingFileEventId, $"Packaging file {relativePath}");
                }

                return result;
            }
        }

        private async Task<CheckHashesResult> CheckFileHashesAsync(
            string appFile, string dSymFile, IList<string> allFilesToUpload)
        {
            using (var scope = _logger.BeginScope("Negotiating upload"))
            {
                var request = new CheckFileHashesRequest(appFile, dSymFile, allFilesToUpload);
                var result = await _testCloudProxy.CheckFileHashesAsync(request);
                LogCheckHashesResponse(result);

                return result;
            }
        }

        private async Task<UploadTestsResult> UploadTestsToTestCloud(
            string appFile, 
            string dSymFile,
            string workspaceDirectory,
            IList<string> otherFiles,
            CheckHashesResult CheckHashesResult)
        {
            using (var scope = _logger.BeginScope("Uploading negotiated files"))
            {
                var request = new UploadTestsRequest(appFile, dSymFile, workspaceDirectory, otherFiles);
                
                request.TestCloudOptions["user"] = _options.User;
                request.TestCloudOptions["device_selection"] = _options.Devices;
                request.TestCloudOptions["app"] = _options.AppName;
                request.TestCloudOptions["locale"] = _options.Locale;
                request.TestCloudOptions["appium"] = "true";
                request.TestCloudOptions["series"] = _options.Series;
                request.TestCloudOptions["api_key"] = _options.ApiKey;

                foreach (var testParameter in _options.TestParameters)
                {
                    request.TestParameters[testParameter.Key] = testParameter.Value;
                }
                request.TestParameters["pipeline"] = "appium";

                foreach (var checkHashResult in CheckHashesResult.Files)
                {
                    request.CheckHashesResult.Files[checkHashResult.Key] = checkHashResult.Value;
                }

                var result = await _testCloudProxy.UploadTestsAsync(request);
                LogUploadTestsResponse(result);

                return result;
            }
        }

        private async Task<int> WaitForJob(UploadTestsResult uploadTestsResult)
        {
            using (var scope = _logger.BeginScope("Waiting for test results"))
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
                        var waitTime = checkStatusResult.WaitTime != null ? 
                            TimeSpan.FromSeconds(checkStatusResult.WaitTime.Value) : DefaultWaitTime;
                        
                        await Task.Delay(waitTime);
                    }
                }
            }
        }

        private void LogCheckHashesResponse(CheckHashesResult response)
        {
            foreach (var result in response.Files.Values.OrderBy(v => v.FilePath))
            {
                var relativePath = FileHelper.GetRelativePath(result.FilePath, _options.Workspace);
                _logger.LogDebug(
                    CheckHashResultEventId,
                    $"File {relativePath} was " + 
                    (result.WasAlreadyUploaded ? "already uploaded." : "not uploaded."));
            }
        }

        private void LogUploadTestsResponse(UploadTestsResult response)
        {
            var logLines = new List<string>();
            logLines.Add("Tests enqueued");
            logLines.Add($"User: {response.UserEmail}");

            if (response.Team != null)
                logLines.Add($"Team: {response.Team}");
            
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
            _logger.LogInformation(
                CheckStatusResultEventId, response.Messages.Select(m => $"{DateTimeOffset.UtcNow.ToString("s")} {m}"));
        }

        private void WriteAsyncJsonResultToConsole(string jobId)
        {
            var asyncJsonResult = new AsyncJsonResult()
            {
                TestRunId = jobId
            };

            var allLogs = _recordedLogs.GetAllLogs();
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
            return devices.Aggregate(new StringBuilder(), (sb, d) => sb.Append($"    {d}"), sb => sb.ToString());
        }
    }
}