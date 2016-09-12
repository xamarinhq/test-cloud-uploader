using System.Threading.Tasks;
using Microsoft.Xtc.TestCloud.ObjectModel;

namespace Microsoft.Xtc.TestCloud.Services
{
    /// <summary>
    /// Interface that represents TestCloud REST API.
    /// </summary>
    public interface ITestCloudProxy
    {
        /// <summary>
        /// Checks whether the current version of the uploader is supported by the Test Cloud.
        /// </summary>
        Task<CheckVersionResult> CheckVersionAsync(CheckVersionRequest request);

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