using System;
using Newtonsoft.Json;

namespace Microsoft.AppHub.TestCloud.ObjectModel
{
    /// <summary>
    /// Stores data required by the ITestCloudProxy.CheckStatus REST API.
    /// </summary>
    public class CheckStatusRequest
    {
        public CheckStatusRequest(string jobId)
        {
            if (string.IsNullOrEmpty(jobId))
                throw new ArgumentNullException(nameof(jobId));
            
            this.JobId = jobId;
        }

        /// <summary>
        /// ID of the scheduled test run job.
        /// </summary>
        [JsonProperty("id")]
        public string JobId { get; }

        /// <summary>
        /// An API key of the user uploading tests.
        /// </summary>
        [JsonProperty("api_key")]
        public string ApiKey { get; set; }

        /// <summary>
        /// An user email.
        /// </summary>
        [JsonProperty("user")]
        public string User { get; set; }

        /// <summary>
        /// Version of the uploader.
        /// </summary>
        [JsonProperty("uploader_version")]
        public string UploaderVersion { get; set; }

        /// <summary>
        /// Session ID.
        /// </summary>
        [JsonProperty("session_id")]
        public string SessionId { get; set; }
    }
}