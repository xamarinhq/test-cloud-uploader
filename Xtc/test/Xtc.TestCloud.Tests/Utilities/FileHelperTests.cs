using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Xtc.Common.Services;
using Microsoft.Xtc.TestCloud.Utilities;
using Xunit;

namespace Microsoft.Xtc.TestCloud.Tests.Utilities
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

        [Fact]
        public void ArchiveAppBundleShouldFailForInvalidFileTypes()
        {
            var invalid = new string[] { "bluemoon.txt", "bluemoon.ipa", "bluemoon.apk", "bluemoon" };
            foreach (string invalidAppname in invalid) 
            {
                Assert.Throws<ArgumentException>(() => FileHelper.ArchiveAppBundle(invalidAppname));
            }
        }

        [Fact]
        public void ArchiveAppBundleShouldFailIfFileDoesNotExist()
        {
            var imaginary = "Unicorn.app";
            Assert.Throws<FileNotFoundException>(() => FileHelper.ArchiveAppBundle(imaginary));
        }

        [Fact]
        public void ArchiveAppBundleShouldSucceedAppFileExists()
        {
            var real = "Dragon.app";
            Directory.CreateDirectory(real);
            FileHelper.ArchiveAppBundle(real);
            Assert.True(File.Exists("Dragon.ipa"));
            Directory.Delete(real);
            File.Delete("Dragon.ipa");
        }

        [Fact]
        public void ArchiveAppBundleShouldSucceedApiFileExists()
        {
            var real = "Dragon.app";
            Directory.CreateDirectory(real);
            var file = System.IO.File.Create("Dragon.ipa");
            var writer = new System.IO.StreamWriter(file);
            writer.WriteLine("Here be dragons");
            writer.Dispose();
            FileHelper.ArchiveAppBundle(real);
            Assert.True(File.Exists("Dragon.ipa"));
            Directory.Delete(real);
            File.Delete("Dragon.ipa");
        }

		[Fact]
		public void ArchiveAppBundleShouldSucceedApiFileExistsNotDitto()
		{
			var real = "Dragon.app";
			Directory.CreateDirectory(real);
			var file = System.IO.File.Create("Dragon.ipa");
			var writer = new System.IO.StreamWriter(file);
			writer.WriteLine("Here be dragons");
			writer.Dispose();
			FileHelper.ArchiveAppBundle(real, true);
			Assert.True(File.Exists("Dragon.ipa"));
			Directory.Delete(real);
			File.Delete("Dragon.ipa");
		}
    } 
}