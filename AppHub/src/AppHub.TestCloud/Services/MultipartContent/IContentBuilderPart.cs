using System.Net.Http;

namespace Microsoft.AppHub.TestCloud
{
    public interface IContentBuilderPart
    {
        void BuildMultipartContent(string parentName, MultipartContent result);        
    }
}