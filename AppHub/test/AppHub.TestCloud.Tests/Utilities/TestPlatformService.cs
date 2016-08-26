using System.Runtime.InteropServices;
using Microsoft.AppHub.Common;

namespace Microsoft.AppHub.TestCloud.Tests
{
    public class TestPlatformService: IPlatformService
    {
        public OSPlatform CurrentPlatform { get; set; }
    }
}
