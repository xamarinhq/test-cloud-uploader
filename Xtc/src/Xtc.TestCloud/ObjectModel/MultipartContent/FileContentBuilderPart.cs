using System;
using System.IO;
using System.Net.Http;

namespace Microsoft.Xtc.TestCloud.ObjectModel.MultipartContent
{
    /// <summary>
    /// A multi-part tree leaf that represents a file that should be uploaded.
    /// </summary>
    public class FileContentBuilderPart : IContentBuilderPart
    {
        private readonly string _filePath;

        public FileContentBuilderPart(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(nameof(filePath));

            _filePath = filePath;
        }

        /// <summary>
        /// Serializes the node and writes its representation to MultipartContent (from System.Net.Http). 
        /// </summary>
        /// <param name="parentName">Name of the parent node.</param>
        /// <param name="result">The System.Net.Http.MultipartContent that will store the result.</param>
        public void BuildMultipartContent(string parentName, System.Net.Http.MultipartContent result)
        {
            if (parentName == null)
                throw new ArgumentNullException(nameof(parentName));
            if (result == null)
                throw new ArgumentNullException(nameof(result));

            var fileName = Path.GetFileName(_filePath);

            var content = new StreamContent(File.OpenRead(_filePath));
            content.Headers.Add("Content-Disposition", $"form-data; name=\"{parentName}\"; filename=\"{fileName}\"");
            content.Headers.Add("Content-Type", "application/octet-stream");

            result.Add(content);
        }
    }
}