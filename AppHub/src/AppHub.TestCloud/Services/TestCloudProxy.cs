using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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

        public async Task<IDictionary<string, bool>> CheckFileHashesAsync(
            string appHash, IList<string> fileHashes, string dsymHash = null)
        {
            var contentBuilder = new DictionaryContentBuilderPart();
            contentBuilder.AddChild("hashes", 
                new ListContentBuilderPart(
                    fileHashes.Select(hash => new StringContentBuilderPart(hash))));
            contentBuilder.AddChild("app_hash", 
                new StringContentBuilderPart(appHash.ToLowerInvariant()));

            if (dsymHash != null)
            {
                contentBuilder.AddChild("dsym_hash", new
                     StringContentBuilderPart(dsymHash.ToLowerInvariant()));
            }

            var boundary = GetMutiPartContentBoundary();
            var httpContent = new MultipartContent("mixed", boundary);
            contentBuilder.BuildMultipartContent(string.Empty, httpContent);

            var httpResponse = await RetryWebRequest(async () => {
                var response = await _httpClient.PostAsync("ci/check_hash", httpContent);
                response.EnsureSuccessStatusCode();

                return response;
            });
            
            var httpResponseString = await httpResponse.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<IDictionary<string, bool>>(httpResponseString);
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