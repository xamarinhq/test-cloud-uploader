using System.Collections.Generic;
using Newtonsoft.Json;

namespace Microsoft.AppHub.TestCloud.ObjectModel
{
    /// <summary>
    /// Represents a response of the ITestCloudProxy.CheckStatus REST API.
    /// </summary>
    public class CheckStatusResult
    {
        /// <summary>
        /// List of server-side messages that should be printed to the user.
        /// </summary>
        [JsonProperty("message")]
        public IList<string> Messages { get; set; }

        /// <summary>
        /// Time to wait before next check of the test run status.
        /// </summary>
        [JsonProperty("wait_time")]
        public int? WaitTime { get; set; }

        /// <summary>
        /// Exit code that should be used by the uploader. If null, the test run has
        /// not completed yet. 
        /// </summary>
        [JsonProperty("exit_code")]
        public int? ExitCode { get; set; }
    }
}