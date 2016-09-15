using Newtonsoft.Json;

namespace Microsoft.Xtc.TestCloud.ObjectModel
{
    /// <summary>
    /// Represents an error response.
    /// </summary>
    public class ErrorResult
    {
        [JsonProperty("error_message")]
        public string ErrorMessage { get; set; }
    }
}