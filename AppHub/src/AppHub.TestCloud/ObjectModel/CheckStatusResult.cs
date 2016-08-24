using System.Collections.Generic;
using Newtonsoft.Json;

namespace Microsoft.AppHub.TestCloud
{
    public class CheckStatusResult
    {
        [JsonProperty("message")]
        public IList<string> Messages { get; set; }

        [JsonProperty("wait_time")]
        public int? WaitTime { get; set; }

        [JsonProperty("exit_code")]
        public int? ExitCode { get; set; }
    }
}