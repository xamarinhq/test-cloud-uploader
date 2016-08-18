using System;
using System.Net.Http;
using System.Text;

namespace Microsoft.AppHub.TestCloud
{
    public class StringContentBuilderPart: IContentBuilderPart
    {
        private readonly string _value;

        public StringContentBuilderPart(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException(nameof(value));

            _value = value;
        }

        public void BuildMultipartContent(string parentName, MultipartContent result)
        {
            if (parentName == null)
                throw new ArgumentNullException(nameof(parentName));
            if (result == null)
                throw new ArgumentNullException(nameof(result));

            var data = $"Content-Disposition: form-data; name=\"{parentName}\"\r\n\r\n{_value}";
            var content = new ByteArrayContent(Encoding.UTF8.GetBytes(data));

            result.Add(content);
        }
    }
}