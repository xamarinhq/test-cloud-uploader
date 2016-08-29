using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AppHub.Common;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Microsoft.AppHub.TestCloud
{
    /// <summary>
    /// Proxy for TestCloud REST API.
    /// </summary>
    public class TestCloudProxy : ITestCloudProxy
    {
        public static readonly EventId HttpRequestEventId = 1;
        public static readonly EventId RetryingEventId = 2;
        public static readonly EventId HttpExceptionEventId = 3;

        public const string UploaderClientVersion = "1.2.0";

        private static readonly TimeSpan _retryDelay = TimeSpan.FromSeconds(10);
        private static readonly TimeSpan _retryTimeout = TimeSpan.FromMinutes(5);

        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;

        public TestCloudProxy(Uri endpointUri, ILoggerService loggerService)
        {
            if (endpointUri == null)
                throw new ArgumentNullException(nameof(endpointUri));
            if (loggerService == null)
                throw new ArgumentNullException(nameof(loggerService));

            _httpClient = new HttpClient()
            {
                BaseAddress = endpointUri
            };

            _logger = loggerService.CreateLogger<TestCloudProxy>();
        }

        /// <summary>
        /// Checks whether the current version of the uploader is supported by the Test Cloud.
        /// </summary>
        public async Task<CheckVersionResult> CheckVersionAsync(CheckVersionRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            request.UploaderVersion = UploaderClientVersion;
            var json = JsonConvert.SerializeObject(request);

            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var httpResponse = await RetryWebRequest(async () =>
            {
                var path = "ci/check_version";
                _logger.LogDebug(HttpRequestEventId, $"HTTP POST request to {_httpClient.BaseAddress + path}");

                var response = await _httpClient.PostAsync(path, httpContent);
                response.EnsureSuccessStatusCode();

                return response;
            });

            var httpResponseString = await httpResponse.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<CheckVersionResult>(httpResponseString);
        }

        /// <summary>
        /// Checks whether files were already uploaded to the Test Cloud. 
        /// </summary>
        public async Task<CheckHashesResult> CheckFileHashesAsync(CheckFileHashesRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var contentBuilder = new DictionaryContentBuilderPart();
            var uploadFiles = new List<UploadFileInfo>(request.OtherFiles) { request.AppFile };

            contentBuilder.AddChild("hashes", new ListContentBuilderPart(
                uploadFiles.Select(fileInfo => new StringContentBuilderPart(fileInfo.FileHash))));
            contentBuilder.AddChild("app_hash", new StringContentBuilderPart(request.AppFile.FileHash));

            if (request.DSymFile != null)
            {
                contentBuilder.AddChild("dsym_hash", new StringContentBuilderPart(request.DSymFile.FileHash));
                uploadFiles.Add(request.DSymFile);
            }

            var checkFileHashesResult = await SendMultipartPostRequest<IDictionary<string, bool>>(
                "ci/check_hash", contentBuilder);

            var resultAppFile = new UploadFileInfo(request.AppFile, wasAlreadyUploaded: checkFileHashesResult[request.AppFile.FileHash]);
            var resultDSymFile = request.DSymFile != null ?
                new UploadFileInfo(request.DSymFile, wasAlreadyUploaded: checkFileHashesResult[request.DSymFile.FileHash]) :
                null;

            return new CheckHashesResult(
                resultAppFile,
                resultDSymFile,
                uploadFiles.Select(fileInfo =>
                    new UploadFileInfo(fileInfo, wasAlreadyUploaded: checkFileHashesResult[fileInfo.FileHash])));
        }

        /// <summary>
        /// Uploads files to the Test Cloud.
        /// </summary>
        public Task<UploadTestsResult> UploadTestsAsync(UploadTestsRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var contentBuilder = new DictionaryContentBuilderPart();
            contentBuilder.AddChild("files", CreateFilesContent(request.OtherFiles));
            contentBuilder.AddChild("paths", CreatePathsContent(request.OtherFiles));
            contentBuilder.AddChild("app_file", CreateFileContent(request.AppFile));
            contentBuilder.AddChild("app_filename", new StringContentBuilderPart(Path.GetFileName(request.AppFile.FullPath)));
            contentBuilder.AddChild("test_parameters", CreateTestParametersContent(request.TestParameters));

            if (request.DSymFile != null)
            {
                contentBuilder.AddChild("dsym_file", CreateFileContent(request.DSymFile));
                contentBuilder.AddChild("dsym_filename", new StringContentBuilderPart(Path.GetFileName(request.DSymFile.FullPath)));
            }

            AddTestCloudOptionsContent(contentBuilder, request.TestCloudOptions);

            return SendMultipartPostRequest<UploadTestsResult>("ci/upload", contentBuilder);
        }

        /// <summary>
        /// Checks status of previously scheduled Test Cloud job.
        /// </summary>
        public async Task<CheckStatusResult> CheckStatusAsync(CheckStatusRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            request.UploaderVersion = UploaderClientVersion;
            var json = JsonConvert.SerializeObject(request);

            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var httpResponse = await RetryWebRequest(async () =>
            {
                var path = "ci/status_v3";
                _logger.LogDebug(HttpRequestEventId, $"HTTP POST request to {_httpClient.BaseAddress + path}");

                var response = await _httpClient.PostAsync(path, httpContent);
                response.EnsureSuccessStatusCode();

                return response;
            });

            var httpResponseString = await httpResponse.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<CheckStatusResult>(httpResponseString);
        }

        private async Task<TResult> SendMultipartPostRequest<TResult>(string path, IContentBuilderPart contentBuilder)
        {
            var httpContent = BuildMultipartContent(contentBuilder);

            var httpResponse = await RetryWebRequest(async () =>
            {
                _logger.LogDebug(HttpRequestEventId, $"HTTP POST request to {_httpClient.BaseAddress + path}");
                var response = await _httpClient.PostAsync(path, httpContent);
                response.EnsureSuccessStatusCode();

                return response;
            });

            var httpResponseString = await httpResponse.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<TResult>(httpResponseString);
        }

        private void AddTestCloudOptionsContent(
            DictionaryContentBuilderPart contentBuilder, IDictionary<string, string> testCloudOptions)
        {
            foreach (var option in testCloudOptions)
            {
                contentBuilder.AddChild(option.Key, new StringContentBuilderPart(option.Value));
            }

            contentBuilder.AddChild("client_version", new StringContentBuilderPart(UploaderClientVersion));
        }

        private IContentBuilderPart CreateTestParametersContent(IDictionary<string, string> testParameters)
        {
            var keyValueItems = testParameters
                .Select(kv =>
                    new KeyValuePair<string, IContentBuilderPart>(kv.Key, new StringContentBuilderPart(kv.Value)))
                .ToArray();

            return new DictionaryContentBuilderPart(keyValueItems);
        }

        private IContentBuilderPart CreateFileContent(UploadFileInfo fileInfo)
        {
            if (fileInfo.WasAlreadyUploaded)
                return new StringContentBuilderPart(fileInfo.FileHash);
            else
                return new FileContentBuilderPart(fileInfo.FullPath);
        }

        private IContentBuilderPart CreateFilesContent(IEnumerable<UploadFileInfo> files)
        {
            return new ListContentBuilderPart(files.Select(CreateFileContent));
        }

        private IContentBuilderPart CreatePathsContent(IEnumerable<UploadFileInfo> files)
        {
            return new ListContentBuilderPart(
                files
                    .Select(fileInfo => new StringContentBuilderPart(fileInfo.RelativePath)));
        }

        private MultipartContent BuildMultipartContent(IContentBuilderPart contentBuilder)
        {
            var boundary = GetMutiPartContentBoundary();
            var httpContent = new MultipartContent("mixed", boundary);
            contentBuilder.BuildMultipartContent(string.Empty, httpContent);

            return httpContent;
        }

        private string GetMutiPartContentBoundary()
        {
            return $"--{DateTimeOffset.UtcNow.Ticks.ToString("x")}--";
        }

        private async Task<T> RetryWebRequest<T>(Func<Task<T>> request)
        {
            var maximumTime = DateTimeOffset.UtcNow + _retryTimeout;

            while (DateTimeOffset.UtcNow < maximumTime)
            {
                try
                {
                    return await request();
                }
                catch (Exception ex) when (IsTransientException(ex))
                {
                    _logger.LogInformation(
                        RetryingEventId,
                        $"Retrying in {_retryDelay.TotalSeconds} seconds. Failed to reach server: {ex.Message}");
                    _logger.LogDebug(HttpExceptionEventId, $"Exception: {ex}");

                    await Task.Delay(_retryDelay);
                }
            }

            return await request();
        }

        private static bool IsTransientException(Exception ex)
        {
            if (ex is HttpRequestException)
                return true;

            if (ex is AggregateException)
                return ((AggregateException) ex).InnerExceptions.Any(IsTransientException);

            return false;
        }
    }
}