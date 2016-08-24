using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AppHub.Cli;
using Microsoft.AppHub.Common;
using Microsoft.Extensions.Logging;

namespace Microsoft.AppHub.TestCloud
{
    public class UploadAppiumTestsCommandExecutor: ICommandExecutor
    {
        private static readonly TimeSpan DefaultWaitTime = TimeSpan.FromSeconds(10); 
        private static readonly Uri _testCloudUri = new Uri("https://testcloud.xamarin.com/ci");

        private readonly UploadTestsCommandOptions _options;
        private readonly TestCloudProxy _testCloudProxy;
        private readonly ILoggerService _loggerService;
        private readonly ILogger _logger;

        public UploadAppiumTestsCommandExecutor(UploadTestsCommandOptions options, ILoggerService loggerService)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            if (loggerService == null)
                throw new ArgumentNullException(nameof(loggerService));

            _options = options;
            _testCloudProxy = new TestCloudProxy(_testCloudUri, loggerService);
            _loggerService = loggerService;
            _logger = loggerService.CreateLogger<UploadAppiumTestsCommandExecutor>();
        }

        public async Task ExecuteAsync()
        {
            ValidateOptions();

            var allFilesToUpload = GetAllFilesToUpload();
            var checkHashesResult = await CheckFileHashesAsync(_options.AppFile, null, allFilesToUpload);
            var uploadResult = await UploadTestsToTestCloud(
                _options.AppFile, null, _options.Workspace, allFilesToUpload, checkHashesResult);

            if (!(_options.Async || _options.AsyncJson))
            {
                var exitCode = await WaitForJob(uploadResult);
                Environment.Exit(exitCode);
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

        private IList<string> GetAllFilesToUpload()
        {
            using (var scope = _logger.BeginScope("Packaging"))
            {
                var result = Directory.GetFiles(_options.Workspace, "*", SearchOption.AllDirectories).ToList();

                foreach (var file in result)
                {
                    var relativePath = FileHelper.GetRelativePath(file, _options.Workspace);
                    _logger.LogDebug($"Packaging file {relativePath}");
                }

                return result;
            }
        }

        private async Task<IDictionary<string, CheckHashResult>> CheckFileHashesAsync(
            string appFile, 
            string dSymFile,
            IList<string> allFilesToUpload)
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
            IDictionary<string, CheckHashResult> checkHashResults)
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

                foreach (var checkHashResult in checkHashResults)
                {
                    request.CheckHashResults[checkHashResult.Key] = checkHashResult.Value;
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

                    var eventId = _loggerService.CreateEventId();
                    foreach (var message in checkStatusResult.Messages)
                    {
                        _logger.LogInformation(eventId, $"{DateTimeOffset.UtcNow.ToString("s")} {message}");
                    }

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

        private void LogCheckHashesResponse(IDictionary<string, CheckHashResult> response)
        {
            var eventId = _loggerService.CreateEventId();

            foreach (var result in response.Values.OrderBy(v => v.FilePath))
            {
                var relativePath = FileHelper.GetRelativePath(result.FilePath, _options.Workspace);
                _logger.LogDebug(
                    $"File {relativePath} was " + 
                    (result.AlreadyUploaded ? "already uploaded." : "not uploaded."));
            }
        }

        private void LogUploadTestsResponse(UploadTestsResult response)
        {
            var eventId = _loggerService.CreateEventId();
            _logger.LogInformation(eventId, "Tests enqueued");
            _logger.LogInformation(eventId, $"User: {response.UserEmail}");
            
            if (response.Team != null)
                _logger.LogInformation(eventId, $"Team: {response.Team}");
            
            if (response.RejectedDevices != null && response.RejectedDevices.Count > 0)
            {
                _logger.LogInformation(
                    eventId, 
                    $"Skipping devices (you can update your selections via https://testcloud.xamarin.com):" + 
                    $"{Environment.NewLine}{GetDevicesListLog(response.RejectedDevices)}");
            }

            _logger.LogInformation(
                eventId, $"Running on devices: {Environment.NewLine}{GetDevicesListLog(response.Devices)}");
        }

        private string GetDevicesListLog(IEnumerable<string> devices)
        {
            return devices.Aggregate(
                    new StringBuilder(),
                    (sb, d) => sb.Append($"    {d}"),
                    sb => sb.ToString()
                );
        }
    }
}