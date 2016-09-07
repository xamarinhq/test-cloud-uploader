using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Microsoft.AppHub.TestCloud.ObjectModel
{
    /// <summary>
    /// Stores data required by the ITestCloudProxy.CheckVersion REST API.
    /// </summary>
    public class CheckVersionRequest
    {
        public CheckVersionRequest(IEnumerable<string> arguments)
        {
            if (arguments == null)
                throw new ArgumentNullException(nameof(arguments));

            this.Arguments = arguments.ToList();
        }

        [JsonProperty("args")]
        public IList<string> Arguments { get; }

        [JsonProperty("uploader_version")]
        public string UploaderVersion { get; set; }

        [JsonProperty("session_id")]
        public string SessionId { get; set; }
    }
}