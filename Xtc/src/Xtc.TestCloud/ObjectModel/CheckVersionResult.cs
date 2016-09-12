using Newtonsoft.Json;

namespace Microsoft.Xtc.TestCloud.ObjectModel
{
    /// <summary>
    /// Represents a response of the ITestCloudProxy.CheckVersion REST API.
    /// </summary>
    public class CheckVersionResult
    {
        [JsonProperty("error_message")]
        public string ErrorMessage { get; set; }
    }
}