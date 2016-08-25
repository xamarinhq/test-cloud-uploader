using System.Collections.Generic;
using Newtonsoft.Json;

namespace Microsoft.AppHub.TestCloud
{
    /// <summary>
    /// Represents data for JSON written to the console when --async-json option is present.
    /// </summary>
    public class AsyncJsonResult
    {
        /// <summary>
        /// ID of the Test Cloud run.
        /// </summary>
        [JsonProperty("test_run_id")]
        public string TestRunId { get; set; }

        /// <summary>
        /// List of all error messages produced by the uploader tool.
        /// </summary>
        [JsonProperty("error_messages")]
        public IList<string> Errors { get; set; }

        /// <summary>
        /// List of all log messages produces by the uploader tool.
        /// </summary>
        [JsonProperty("log")]
        public IList<string> Logs { get; set; }
    }
}