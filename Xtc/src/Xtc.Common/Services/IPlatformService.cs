using System.Runtime.InteropServices;

namespace Microsoft.Xtc.Common.Services
{
    /// <summary>
    /// Represents servicve that detects current platform.
    /// </summary>
    public interface IPlatformService
    {
        /// <summary>
        /// Gets current platform.
        /// </summary>
        OSPlatform CurrentPlatform { get; }
    }
}