using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Xtc.Common.Services.Logging;
using Microsoft.Xtc.TestCloud.ObjectModel;
using Microsoft.Xtc.TestCloud.ObjectModel.MultipartContent;
using Newtonsoft.Json;

namespace Microsoft.Xtc.TestCloud.Services
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

            var httpResponse = await RetryWebRequest(async () =>
            {
                var path = "ci/check_version";
                _logger.LogDebug(HttpRequestEventId, $"HTTP POST request to {_httpClient.BaseAddress + path}");

                var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                return await _httpClient.PostAsync(path, httpContent);
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

            var resultDSymFile = request.DSymFile != null 
                ? new UploadFileInfo(request.DSymFile, wasAlreadyUploaded: checkFileHashesResult[request.DSymFile.FileHash]) 
                : null;

            return new CheckHashesResult(
                resultAppFile,
                resultDSymFile,
                uploadFiles
                    .Where(fileInfo => fileInfo.FullPath != resultAppFile.FullPath && fileInfo.FullPath != request.DSymFile?.FullPath)
                    .Select(fileInfo =>
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
                contentBuilder.AddChild("dsym_filename", new StringContentBuilderPart($"{Path.GetFileName(request.AppFile.FullPath)}_dSym"));
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

            var httpResponse = await RetryWebRequest(async () =>
            {
                var path = "ci/status_v3";
                _logger.LogDebug(HttpRequestEventId, $"HTTP POST request to {_httpClient.BaseAddress + path}");

                var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                return await _httpClient.PostAsync(path, httpContent);
            });

            var httpResponseString = await httpResponse.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<CheckStatusResult>(httpResponseString);
        }

        private async Task<TResult> SendMultipartPostRequest<TResult>(string path, IContentBuilderPart contentBuilder)
        {
            var httpResponse = await RetryWebRequest(async () =>
            {
                _logger.LogDebug(HttpRequestEventId, $"HTTP POST request to {_httpClient.BaseAddress + path}");
                
                var httpContent = BuildMultipartContent(contentBuilder);
                return await _httpClient.PostAsync(path, httpContent);
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
            {
                return new StringContentBuilderPart(fileInfo.FileHash);
            }
            else
            {
                return new FileContentBuilderPart(fileInfo.FullPath);
            }
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

        private async Task<HttpResponseMessage> RetryWebRequest(Func<Task<HttpResponseMessage>> request)
        {
            var maximumTime = DateTimeOffset.UtcNow + _retryTimeout;

            while (true)
            {
                try
                {
                    var result = await request();

                    if ((int)result.StatusCode < 400)
                    {
                        return result;
                    }

                    if (IsTransientErrorCode(result.StatusCode) && DateTimeOffset.UtcNow < maximumTime)
                    {
                        _logger.LogInformation(
                            RetryingEventId,
                            $"Retrying in {_retryDelay.TotalSeconds} seconds. Server returned status code: {result.StatusCode}");

                        await Task.Delay(_retryDelay);
                    }
                    else
                    {
                        var errorMessage = await GetCustomErrorMessage(result);
                        throw new HttpRequestException(errorMessage);
                    }
                }
                // The HttpException may be thrown when error occurs in layers lower than HTTP.
                catch (HttpRequestException ex) when (ex.InnerException != null)
                {
                    // The default error message for is not useful. For example, when the host name
                    // cannot be resolved, it contains generic message "An error occurred while sending
                    // the request. Inner exception has more information.
                    var errorMessage = $"{ex.Message}{Environment.NewLine}{ex.InnerException.Message}";
                    throw new HttpRequestException(errorMessage, ex);
                }
            }
        }

        private async Task<string> GetCustomErrorMessage(HttpResponseMessage response)
        {
            var defaultErrorMessage = $"Response status code does not indicate success: " + 
                                      $"{(int)response.StatusCode} ({response.StatusCode})";
            var json = await response.Content?.ReadAsStringAsync();
            
            if (!string.IsNullOrEmpty(json))
            {
                var errorResponse = JsonConvert.DeserializeObject<ErrorResult>(json);
                return errorResponse.ErrorMessage ?? defaultErrorMessage;
            }
            else
            {
                return defaultErrorMessage;
            }
        }

        private static bool IsTransientErrorCode(HttpStatusCode statusCode)
        {
            return statusCode == HttpStatusCode.GatewayTimeout ||
                   statusCode == HttpStatusCode.RequestTimeout ||
                   statusCode == HttpStatusCode.ServiceUnavailable ||
                   statusCode == HttpStatusCode.InternalServerError;
        }
    }
}