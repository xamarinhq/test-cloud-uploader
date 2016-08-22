using System.Net.Http;

namespace Microsoft.AppHub.TestCloud
{
    /// <summary>
    /// Interface for nodes in a tree that represents the multi-part HTTP content recognized by Test Cloud 
    /// REST API. 
    /// </summary>
    /// <remarks>
    /// The Test Cloud REST APIs (check_hash and upload) accept multi-part HTTP content, which is
    /// used to represent a tree of various data types: strings, files (as leaves), dictionaries and
    /// lists. This interface represents a node in that tree.
    /// In the original UI Test upload tool, the tree was represented by IDictionary&lt;string, object&rt;, and
    /// then serialized to HTTP content by a few methods that recognized supported types and some of their combinations. 
    /// In this refactored version, each node in the tree is responsible for its own serialization. This makes
    /// the serialization more generic, testable, and prevents user of the API to construct a tree with types that 
    /// are not supported by Test Cloud.
    /// </remarks>
    public interface IContentBuilderPart
    {
        /// <summary>
        /// Serializes the node and writes its representation to MultipartContent (from System.Net.Http). 
        /// </summary>
        /// <param name="parentName">Name of the parent node.</param>
        /// <param name="result">The System.Net.Http.MultipartContent that will store the result.</param>
        void BuildMultipartContent(string parentName, MultipartContent result);
    }
}