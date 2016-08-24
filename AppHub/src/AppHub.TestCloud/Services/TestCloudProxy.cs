using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
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
    public class TestCloudProxy: ITestCloudProxy
    {
        public const string UploaderClientVersion = "1.2.0";
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

            var httpResponse = await RetryWebRequest(async () => {
                var response = await _httpClient.PostAsync("ci/check_version", httpContent);
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

            using (var sha256 = SHA256.Create())
            {
                var appFileHash = sha256.GetHash(new FileInfo(request.AppFile));
                var fileNameToHash = request
                    .OtherFiles
                    .ToDictionary(path => path, path => sha256.GetHash(new FileInfo(path)));
                fileNameToHash[request.AppFile] = appFileHash;

                contentBuilder.AddChild("hashes", new ListContentBuilderPart(
                    fileNameToHash.Values.Select(hash => new StringContentBuilderPart(hash))));
                contentBuilder.AddChild("app_hash", new StringContentBuilderPart(appFileHash.ToLowerInvariant()));
                
                if (request.DSymFile != null)
                {
                    var dSymFileHash = sha256.GetHash(new FileInfo(request.DSymFile));
                    contentBuilder.AddChild("dsym_hash", new StringContentBuilderPart(appFileHash));
                    fileNameToHash[request.DSymFile] = dSymFileHash; 
                }
                               
                var checkFileHashesResult = await SendMultipartPostRequest<IDictionary<string, bool>>(
                    "ci/check_hash", contentBuilder);
                
                return new CheckHashesResult(
                    fileNameToHash.Keys.Select(
                        path => 
                        {
                            var hash = fileNameToHash[path];
                            var alreadyUploaded = checkFileHashesResult[hash];
                            var fileResult = new SingleFileCheckHashResult(path, hash, alreadyUploaded); 
                            
                            return new KeyValuePair<string, SingleFileCheckHashResult>(path, fileResult); 
                        }));
            }
        }

        /// <summary>
        /// Uploads files to the Test Cloud.
        /// </summary>
        public Task<UploadTestsResult> UploadTestsAsync(UploadTestsRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var contentBuilder = new DictionaryContentBuilderPart();
            contentBuilder.AddChild("files", CreateFilesContent(request.OtherFiles, request.CheckHashesResult));
            contentBuilder.AddChild("paths", CreatePathsContent(request.WorkspaceDirectory, request.OtherFiles));
            contentBuilder.AddChild("app_file", CreateAppFileContent(request.AppFile, request.CheckHashesResult));
            contentBuilder.AddChild("app_filename", new StringContentBuilderPart(Path.GetFileName(request.AppFile)));
            contentBuilder.AddChild("test_parameters", CreateTestParametersContent(request.TestParameters));
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

            var httpResponse = await RetryWebRequest(async () => {
                var response = await _httpClient.PostAsync("ci/status_v3", httpContent);
                response.EnsureSuccessStatusCode();

                return response;
            });
            
            var httpResponseString = await httpResponse.Content.ReadAsStringAsync();
            
            return JsonConvert.DeserializeObject<CheckStatusResult>(httpResponseString);
        }

        private async Task<TResult> SendMultipartPostRequest<TResult>(string path, IContentBuilderPart contentBuilder)
        {
            var httpContent = BuildMultipartContent(contentBuilder);

            var httpResponse = await RetryWebRequest(async () => {
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

        private IContentBuilderPart CreateAppFileContent(string appFile, CheckHashesResult CheckHashesResult)
        {
            var checkAppHashResult = CheckHashesResult.Files[appFile];
            if (checkAppHashResult.WasAlreadyUploaded)
                return new StringContentBuilderPart(checkAppHashResult.FileHash);
            else
                return new FileContentBuilderPart(appFile);
        }

        private IContentBuilderPart CreateFilesContent(IList<string> files, CheckHashesResult CheckHashesResult)
        {
            return new ListContentBuilderPart(
                files.Select<string, IContentBuilderPart>(path => 
                {
                    var fileCheckHashResult = CheckHashesResult.Files[path];
                    if (fileCheckHashResult.WasAlreadyUploaded)
                        return new StringContentBuilderPart(fileCheckHashResult.FileHash);
                    else
                        return new FileContentBuilderPart(path);
                }));
        }

        private IContentBuilderPart CreatePathsContent(string workspace, IList<string> files)
        {
            return new ListContentBuilderPart(
                files
                    .Select(path => FileHelper.GetRelativePath(path, workspace))
                    .Select(relativePath => new StringContentBuilderPart(relativePath)));
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
                    _logger.LogInformation($"Retrying in 10 seconds. Failed to reach server: {ex.Message}");
                    _logger.LogDebug($"Exception: {ex.ToString()}");

                    await Task.Delay(TimeSpan.FromSeconds(10));
                }
            }

            return await request();
        }

        private static bool IsTransientException(Exception ex)
        {
            if (ex is HttpRequestException)
                return true;

            if (ex is AggregateException)
                return ((AggregateException)ex).InnerExceptions.Any(iex => IsTransientException(iex));
            
            return false;
        }
    }
}