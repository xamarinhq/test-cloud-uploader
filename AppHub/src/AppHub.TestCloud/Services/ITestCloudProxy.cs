using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.AppHub.TestCloud
{
    /// <summary>
    /// Interface that represents TestCloud REST API.
    /// </summary>
    public interface ITestCloudProxy
    {
        Task<IDictionary<string, bool>> CheckFileHashesAsync(
            string appHash, IList<string> fileHashes, string dsymHash = null);
    }
}