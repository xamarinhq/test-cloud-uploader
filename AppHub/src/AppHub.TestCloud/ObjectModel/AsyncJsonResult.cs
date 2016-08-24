using System.Collections.Generic;
using Newtonsoft.Json;

namespace Microsoft.AppHub.TestCloud
{
    public class AsyncJsonResult
    {
        [JsonProperty("test_run_id")]
        public string TestRunId { get; set; }

        [JsonProperty("error_messages")]
        public IList<string> ErrorMessages { get; set; }

        [JsonProperty("log")]
        public IList<string> Logs { get; set; }
    }
}