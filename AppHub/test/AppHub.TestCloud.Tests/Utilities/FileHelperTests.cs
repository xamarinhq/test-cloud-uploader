using System.IO;
using System.Runtime.InteropServices;
using Microsoft.AppHub.Common.Services;
using Microsoft.AppHub.TestCloud.Utilities;
using Xunit;

namespace Microsoft.AppHub.TestCloud.Tests.Utilities
{
    public class FileHelperTests
    {
        [Fact]
        public void GetRelativePathShouldReturnFullPathForUnrelatedDirectory()
        {
            var separator = Path.DirectorySeparatorChar;
            var filePath = $"d:{separator}Temp{separator}Foo.bar";
            var rootDirectoryPath = $"c:{separator}Temp";

            var relativePath = FileHelper.GetRelativePath(filePath, rootDirectoryPath, new PlatformService());

            Assert.Equal("d:/Temp/Foo.bar", relativePath); 
        }

        [Fact]
        public void GetRelativePathShouldReturnCorrectPathForDirectoryWithSlash()
        {
            var separator = Path.DirectorySeparatorChar;
            var filePath = $"d:{separator}Dir1{separator}Dir2{separator}Foo.bar";
            var rootDirectoryPath = $"d:{separator}Dir1{separator}";

            var relativePath = FileHelper.GetRelativePath(filePath, rootDirectoryPath, new PlatformService());

            Assert.Equal("Dir2/Foo.bar", relativePath);
        }

        [Fact]
        public void GetRelativePathShouldReturnCorrectPathForDirectoryWithoutSlash()
        {
            var separator = Path.DirectorySeparatorChar;
            var filePath = $"d:{separator}Dir1{separator}Dir2{separator}Foo.bar";
            var rootDirectoryPath = $"d:{separator}Dir1";

            var relativePath = FileHelper.GetRelativePath(filePath, rootDirectoryPath, new PlatformService());

            Assert.Equal("Dir2/Foo.bar", relativePath);
        }

        [Fact]
        public void ComparisonIsCaseSensitiveOnOsXAndLinux()
        {
            var filePath = $"/Dir1/Dir2/Foo.bar";
            var rootDirectoryPath = $"/dir1";

            var linuxPlatform = new TestPlatformService() { CurrentPlatform = OSPlatform.Linux };
            var osXPlatform = new TestPlatformService() { CurrentPlatform = OSPlatform.OSX };

            Assert.Equal(filePath, FileHelper.GetRelativePath(filePath, rootDirectoryPath, linuxPlatform));
            Assert.Equal(filePath, FileHelper.GetRelativePath(filePath, rootDirectoryPath, osXPlatform));
        }

        [Fact]
        public void ComparisionIsCaseInsensitiveOnWindows()
        {
            var filePath = $"d:\\Dir1\\Dir2\\Foo.bar";
            var rootDirectoryPath = $"d:\\dir1";

            var windowsPlatform = new TestPlatformService() { CurrentPlatform = OSPlatform.Windows };

            Assert.Equal("Dir2/Foo.bar", FileHelper.GetRelativePath(filePath, rootDirectoryPath, windowsPlatform));
        }
    } 
}