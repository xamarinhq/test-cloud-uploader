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
        public const string UploaderClientVersion = "1.2.0";

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
            LogCheckHashesResponse(checkHashesResult);
            
            var uploadResult = await UploadTestsToTestCloud(
                _options.AppFile, null, _options.Workspace, allFilesToUpload, checkHashesResult);
            LogUploadTestsResponse(uploadResult);

            // if (!(_options.Async || _options.AsyncJson))
            // {
            //     await WaitForJob(uploadResult.JobId);
            // }
        }

        private void ValidateOptions()
        {
            _options.Validate();

            // if (ValidationHelper.IsAndroidApp(_options.AppFile))
            // {
            //     // Shared runtime and internet permissions
            //     throw new NotImplementedException();
            // }
        }

        private IList<string> GetAllFilesToUpload()
        {
            return Directory.GetFiles(_options.Workspace, "*", SearchOption.AllDirectories).ToList();
        }

        private Task<IDictionary<string, CheckHashResult>> CheckFileHashesAsync(
            string appFile, 
            string dSymFile,
            IList<string> allFilesToUpload)
        {
            var request = new CheckFileHashesRequest(appFile, dSymFile, allFilesToUpload);
            return _testCloudProxy.CheckFileHashesAsync(request);
        }

        private async Task<UploadTestsResult> UploadTestsToTestCloud(
            string appFile, 
            string dSymFile,
            string workspaceDirectory,
            IList<string> otherFiles,
            IDictionary<string, CheckHashResult> checkHashResults)
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

            return await _testCloudProxy.UploadTestsAsync(request);
        }

        private async Task WaitForJob(string jobId)
        {            
        }

        private void LogCheckHashesResponse(IDictionary<string, CheckHashResult> response)
        {
            var eventId = _loggerService.CreateEventId();

            foreach (var result in response.Values.OrderBy(v => v.FilePath))
            {
                _logger.LogInformation(
                    $"File {result.FilePath} was " + 
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