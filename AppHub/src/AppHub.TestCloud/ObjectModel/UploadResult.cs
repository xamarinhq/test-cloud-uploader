using System.Collections.Generic; 

namespace Microsoft.AppHub.TestCloud
{
    public class UploadResult
    {
        public string Id { get; set; }

        public string UserEmail { get; set; }

        public IList<string> Devices { get; set; }

        public IList<string> RejectedDevices { get; set; }

        public string ErrorMessage { get; set; }
    }
}