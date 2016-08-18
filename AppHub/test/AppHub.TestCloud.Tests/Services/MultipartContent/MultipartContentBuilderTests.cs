using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Xunit;

namespace Microsoft.AppHub.TestCloud.Tests
{
    /// <summary>
    /// Tests which verify that all content builder classes produce correct multi-part HTTP content.
    /// </summary>
    public class MultipartContentBuilderTests
    {
        private const string BoundaryString = "--BOUNDARY--";

        [Fact]
        public void StringValuesAreCorrectlySerialized()
        {
            var builder = new DictionaryContentBuilderPart(
                new KeyValuePair<string, IContentBuilderPart>("foo", new StringContentBuilderPart("bar")));

            var expectedContent = $@"{BoundaryString}
Content-Disposition: form-data; name=""foo""
bar
{BoundaryString}
";
            
            RunTest(builder, expectedContent);
        }

        [Fact]
        public void ListsAreCorrectlySerialized()
        {
            var builder = new DictionaryContentBuilderPart(
                new KeyValuePair<string, IContentBuilderPart>("foo", new ListContentBuilderPart(
                    new StringContentBuilderPart("bar1"),
                    new StringContentBuilderPart("bar2")
            )));

            var expectedContent = $@"{BoundaryString}
Content-Disposition: form-data; name=""foo[]""
bar1
{BoundaryString}
Content-Disposition: form-data; name=""foo[]""
bar2
{BoundaryString}
";

            RunTest(builder, expectedContent);
        }

        [Fact]
        public void DictionariesAreCorrectlySerialized()
        {
            var builder = new DictionaryContentBuilderPart(
                new KeyValuePair<string, IContentBuilderPart>("foo", new DictionaryContentBuilderPart(
                    new KeyValuePair<string, IContentBuilderPart>("key1", new StringContentBuilderPart("value1")),
                    new KeyValuePair<string, IContentBuilderPart>("key2", new StringContentBuilderPart("value2"))
                )));

            var expectedContent = $@"{BoundaryString}
Content-Disposition: form-data; name=""foo[key1]""
value1
{BoundaryString}
Content-Disposition: form-data; name=""foo[key2]""
value2
{BoundaryString}
";

            RunTest(builder, expectedContent);
        }

        [Fact]
        public void NestedStructuresAreCorrectlySerialized()
        {
            var builder = new DictionaryContentBuilderPart(
                new KeyValuePair<string, IContentBuilderPart>("root", new DictionaryContentBuilderPart(
                    new KeyValuePair<string, IContentBuilderPart>("key1", new ListContentBuilderPart(
                        new StringContentBuilderPart("value1"))),
                    new KeyValuePair<string, IContentBuilderPart>("key2", new DictionaryContentBuilderPart(
                        new KeyValuePair<string, IContentBuilderPart>("key2.1", new DictionaryContentBuilderPart(
                            new KeyValuePair<string, IContentBuilderPart>("key2.1.1", new ListContentBuilderPart(
                                new StringContentBuilderPart("value2.1.1.1"),
                                new StringContentBuilderPart("value2.1.1.2")
                            ))
                        ))
                    ))
                ))
            );

            var expectedContent = $@"--BOUNDARY--
Content-Disposition: form-data; name=""root[key1][]""
value1
--BOUNDARY--
Content-Disposition: form-data; name=""root[key2][key2.1][key2.1.1][]""
value2.1.1.1
--BOUNDARY--
Content-Disposition: form-data; name=""root[key2][key2.1][key2.1.1][]""
value2.1.1.2
--BOUNDARY--
";

            RunTest(builder, expectedContent);
        }

        private void RunTest(IContentBuilderPart contentBuilder, string expectedContent)
        {
            var multipartContent = new MultipartContent();
            contentBuilder.BuildMultipartContent(string.Empty, multipartContent);
            
            var actualContent = CleanupActualContent(multipartContent.ReadAsStringAsync().Result);

            Assert.Equal(expectedContent, actualContent); 
        }

        private string CleanupActualContent(string content)
        {
            var result = new StringBuilder();
            var lines = content.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                if (line.StartsWith("--"))
                    result.AppendLine(BoundaryString);
                else
                    result.AppendLine(line);
            }

            return result.ToString();
        }
    }
}