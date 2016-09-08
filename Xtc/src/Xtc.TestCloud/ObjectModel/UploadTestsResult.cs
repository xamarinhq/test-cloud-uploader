using System.Collections.Generic;
using Newtonsoft.Json;

namespace Microsoft.Xtc.TestCloud.ObjectModel
{
    /// <summary>
    /// Represents tests upload result from Test Cloud REST API.
    /// </summary>
    public class UploadTestsResult
    {
        /// <summary>
        /// ID of the Test Cloud job.
        /// </summary>
        /// <returns></returns>
        [JsonProperty("id")]
        public string JobId { get; set; }

        /// <summary>
        /// Email of the user submitting tests.
        /// </summary>
        [JsonProperty("user_email")]
        public string UserEmail { get; set; }

        /// <summary>
        /// Name of the team submitting tests.
        /// </summary>
        [JsonProperty("team")]
        public string Team { get; set; }

        /// <summary>
        /// List of rejected devices.
        /// </summary>
        [JsonProperty("rejected_devices")]
        public IList<string> RejectedDevices { get; set; }

        /// <summary>
        /// List of accepted devices.
        /// </summary>
        [JsonProperty("devices")]
        public IList<string> AcceptedDevices { get; set; }
    }
}