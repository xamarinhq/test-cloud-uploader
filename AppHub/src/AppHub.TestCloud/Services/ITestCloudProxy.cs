using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.AppHub.TestCloud
{
    /// <summary>
    /// Interface that represents TestCloud REST API.
    /// </summary>
    public interface ITestCloudProxy
    {
        Task<IDictionary<string, CheckHashResult>> CheckFileHashesAsync(CheckFileHashesRequest request);

        Task<UploadTestsResult> UploadTestsAsync(UploadTestsRequest request);

        Task<CheckStatusResult> CheckStatusAsync(CheckStatusRequest request);
    }
}