using System;
using System.Net.Http;
using System.Text;

namespace Microsoft.AppHub.TestCloud
{
    /// <summary>
    /// A multi-part tree leaf that represents a string value.
    /// </summary>
    public class StringContentBuilderPart: IContentBuilderPart
    {
        private readonly string _value;

        public StringContentBuilderPart(string value)
        {
            _value = value ?? string.Empty;
        }

        /// <summary>
        /// Serializes the node and writes its representation to MultipartContent (from System.Net.Http). 
        /// </summary>
        /// <param name="parentName">Name of the parent node.</param>
        /// <param name="result">The System.Net.Http.MultipartContent that will store the result.</param>
        public void BuildMultipartContent(string parentName, MultipartContent result)
        {
            if (parentName == null)
                throw new ArgumentNullException(nameof(parentName));
            if (result == null)
                throw new ArgumentNullException(nameof(result));

            var content = new ByteArrayContent(Encoding.UTF8.GetBytes(_value));
            content.Headers.Add("Content-Disposition", $"form-data; name=\"{parentName}\"");

            result.Add(content);
        }
    }
}