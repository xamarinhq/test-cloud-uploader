using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AppHub.Cli;
using Microsoft.AppHub.Common;
using Microsoft.Extensions.Logging;

namespace Microsoft.AppHub.TestCloud
{
    public class UploadAppiumTestsCommandExecutor: ICommandExecutor
    {
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
            
            var checkFileHashesResult = await CheckIfFilesWereAlreadyUploadedAsync(
                _options.AppFile, "TODO", allFilesToUpload);
            LogCheckHashesResponse(checkFileHashesResult);
            
            //var uploadResult = await UploadTestsToTestCloud();            
            //LogUploadTestsResponse(uploadResult);

            // if (!(_options.Async || _options.AsyncJson))
            // {
            //     await WaitForJob(uploadResult.JobId);
            // }
        }

        private void ValidateOptions()
        {
            _options.Validate();

            if (ValidationHelper.IsAndroidApp(_options.AppFile))
            {
                // Shared runtime and internet permissions
                throw new NotImplementedException();
            }
        }

        private IList<string> GetAllFilesToUpload()
        {
            var result = Directory.GetFiles(_options.Workspace, "*", SearchOption.AllDirectories).ToList();
            result.Add(_options.AppFile);

            return result;
        }

        private async Task<IList<CheckHashResult>> CheckIfFilesWereAlreadyUploadedAsync(
            string appFilePath, string dSymFilePath, IList<string> files)
        {
            var appFileInfo = new FileInfo(appFilePath);
            var allFileInfos = files.Select(f => new FileInfo(f)).Concat(new[] { appFileInfo });

            using (var sha256 = SHA256.Create())
            {
                var appFileHash = sha256.GetHash(appFileInfo);
                var hashToFileName = allFileInfos
                    .Select(fi => new { Name = fi.FullName, Hash = sha256.GetHash(fi).ToLowerInvariant() })
                    .GroupBy(nameHash => nameHash.Hash)
                    .ToDictionary(
                        hashGroup => hashGroup.Key,
                        hashGroup => hashGroup.Select(nameHash => nameHash.Name).ToList(),
                        StringComparer.OrdinalIgnoreCase);
                
                var checkFileHashesResult = await _testCloudProxy.CheckFileHashesAsync(
                    appFileHash, hashToFileName.Keys.ToList());

                return checkFileHashesResult
                    .SelectMany(hashExists => 
                        hashToFileName[hashExists.Key].Select(
                            name => new CheckHashResult(name, hashExists.Key, hashExists.Value))) 
                    .ToList();
            }
        }

        private async Task<UploadTestsResult> UploadTestsToTestCloud()
        {
            return new UploadTestsResult();
        }

        private DictionaryContentBuilderPart CreateUploadContent()
        {
            var result = new DictionaryContentBuilderPart();
            // files
            // paths
            result.AddChild("user", new StringContentBuilderPart(_options.User));
            // client_version
            // app_file
            result.AddChild("device_selection", new StringContentBuilderPart(_options.Devices));
            // app (app name)
            // test_parameters
            result.AddChild("locale", new StringContentBuilderPart(_options.Locale));
            result.AddChild("appium", new StringContentBuilderPart("true"));
            result.AddChild("series", new StringContentBuilderPart("series"));
            result.AddChild("api_key", new StringContentBuilderPart(_options.ApiKey));
            // dsym_file (zip)
            // dsym_filename
            // app_filename
            // profile
            //result.AddChild()

            return result;
        }

        private async Task WaitForJob(string jobId)
        {
            
        }

        private void LogCheckHashesResponse(IList<CheckHashResult> response)
        {
            var eventId = _loggerService.CreateEventId();

            foreach (var result in response)
            {
                _logger.LogInformation($"File {result.FilePath} was " + (result.AlreadyUploaded ? "already uploaded." : "not uploaded."));
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