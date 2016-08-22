using System.Collections.Generic;
using Newtonsoft.Json;

namespace Microsoft.AppHub.TestCloud
{
    /// <summary>
    /// Represents tests upload result from Test Cloud REST API.
    /// </summary>
    public class UploadTestsResult
    {
        [JsonProperty("id")]
        public string JobId { get; set; }

        [JsonProperty("user_email")]
        public string UserEmail { get; set; }

        [JsonProperty("team")]
        public string Team { get; set; }

        [JsonProperty("rejected_devices")]
        public IList<string> RejectedDevices { get; set; }

        [JsonProperty("devices")]
        public IList<string> Devices { get; set; }
    }
}