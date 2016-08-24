using System.Threading.Tasks;

namespace Microsoft.AppHub.TestCloud
{
    /// <summary>
    /// Interface that represents TestCloud REST API.
    /// </summary>
    public interface ITestCloudProxy
    {
        /// <summary>
        /// Checks whether files were already uploaded to the Test Cloud. 
        /// </summary>
        Task<CheckHashesResult> CheckFileHashesAsync(CheckFileHashesRequest request);

        /// <summary>
        /// Uploads files to the Test Cloud.
        /// </summary>
        Task<UploadTestsResult> UploadTestsAsync(UploadTestsRequest request);

        /// <summary>
        /// Checks status of previously scheduled Test Cloud job.
        /// </summary>
        Task<CheckStatusResult> CheckStatusAsync(CheckStatusRequest request);
    }
}