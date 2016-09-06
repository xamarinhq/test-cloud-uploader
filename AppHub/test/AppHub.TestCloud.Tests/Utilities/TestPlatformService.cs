using System.Runtime.InteropServices;
using Microsoft.AppHub.Common.Services;

namespace Microsoft.AppHub.TestCloud.Tests.Utilities
{
    public class TestPlatformService: IPlatformService
    {
        public OSPlatform CurrentPlatform { get; set; }
    }
}
