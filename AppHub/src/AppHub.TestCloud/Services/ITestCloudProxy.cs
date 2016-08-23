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
        // Task<UploadTestsResult> UploadTestAsync(
        //     string appFile,
        //     string dSymFile,
        //     IList<string> otherFiles,
        //     IDictionary<string, CheckHashResult> checkHashResults,
        //     IDictionary<string, string> testCloudOptions,
        //     IDictionary<string, string> testParameters
        // );
    }
}