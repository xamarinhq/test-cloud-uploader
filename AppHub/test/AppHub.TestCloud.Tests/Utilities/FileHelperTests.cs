using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text;
using Xunit;

namespace Microsoft.AppHub.TestCloud.Tests
{
    public class FileHelperTests
    {
        [Fact]
        public void GetRelativePathShouldReturnFullPathForUnrelatedDirectory()
        {
            var separator = Path.DirectorySeparatorChar;
            var filePath = $"d:{separator}Temp{separator}Foo.bar";
            var rootDirectoryPath = $"c:{separator}Temp";

            var relativePath = FileHelper.GetRelativePath(filePath, rootDirectoryPath);

            Assert.Equal("d:/Temp/Foo.bar", relativePath); 
        }

        [Fact]
        public void GetRelativePathShouldReturnCorrectPathForDirectoryWithSlash()
        {
            var separator = Path.DirectorySeparatorChar;
            var filePath = $"d:{separator}Dir1{separator}Dir2{separator}Foo.bar";
            var rootDirectoryPath = $"d:{separator}Dir1{separator}";

            var relativePath = FileHelper.GetRelativePath(filePath, rootDirectoryPath);

            Assert.Equal("Dir2/Foo.bar", relativePath);
        }

        [Fact]
        public void GetRleativePathShouldReturnCorrectPathForDirectoryWithoutSlash()
        {
            var separator = Path.DirectorySeparatorChar;
            var filePath = $"d:{separator}Dir1{separator}Dir2{separator}Foo.bar";
            var rootDirectoryPath = $"d:{separator}Dir1";

            var relativePath = FileHelper.GetRelativePath(filePath, rootDirectoryPath);

            Assert.Equal("Dir2/Foo.bar", relativePath);
        }
    } 
}