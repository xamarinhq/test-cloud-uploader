using System;
using Newtonsoft.Json;

namespace Microsoft.AppHub.TestCloud
{
    public class CheckStatusRequest
    {
        public CheckStatusRequest(string jobId)
        {
            if (string.IsNullOrEmpty(jobId))
                throw new ArgumentNullException(nameof(jobId));
            
            this.JobId = jobId;
        }

        [JsonProperty("id")]
        public string JobId { get; }

        [JsonProperty("api_key")]
        public string ApiKey { get; set; }

        [JsonProperty("user")]
        public string User { get; set; }

        [JsonProperty("uploader_version")]
        public string UploaderVersion { get; set; }

        [JsonProperty("session_id")]
        public string SessionId { get; set; }
    }
}