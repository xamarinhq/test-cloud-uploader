using System.Runtime.InteropServices;
using Microsoft.Xtc.Common.Services;

namespace Microsoft.Xtc.TestCloud.Tests.Utilities
{
    public class TestPlatformService: IPlatformService
    {
        public OSPlatform CurrentPlatform { get; set; }
    }
}
