using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Microsoft.AppHub.TestCloud
{
    /// <summary>
    /// A multi-part tree node that represents a dictionary.
    /// </summary>
    public class DictionaryContentBuilderPart: IContentBuilderPart
    {
        private readonly IDictionary<string, IContentBuilderPart> _items;

        public DictionaryContentBuilderPart()
        {
            _items = new Dictionary<string, IContentBuilderPart>();
        }

        public DictionaryContentBuilderPart(params KeyValuePair<string, IContentBuilderPart>[] items)
        {
            _items = new Dictionary<string, IContentBuilderPart>();

            foreach (var keyValue in items)
            {
                _items.Add(keyValue.Key, keyValue.Value);
            }
        }

        /// <summary>
        /// Adds child element to the node.
        /// </summary>
        /// <param name="name">Name (key) of the child element.</param>
        /// <param name="childPart">Child element.</param>
        public void AddChild(string name, IContentBuilderPart childPart)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (childPart == null)
                throw new ArgumentNullException(nameof(childPart));

            _items.Add(name, childPart);
        }

        /// <summary>
        /// /// Serializes the node and writes its representation to MultipartContent (form System.Net.Http). 
        /// </summary>
        /// <param name="parentName">Name of the parent node.</param>
        /// <param name="result">The System.Net.Http.MultipartContent that will store the result.</param>
        public void BuildMultipartContent(string parentName, MultipartContent result)
        {
            if (parentName == null)
                throw new ArgumentNullException(nameof(parentName));
            if (result == null)
                throw new ArgumentNullException(nameof(result));

            foreach (var childPart in _items)
            {
                var fullName = string.IsNullOrEmpty(parentName) ?
                    childPart.Key : 
                    $"{parentName}[{childPart.Key}]";
                
                childPart.Value.BuildMultipartContent(fullName, result);
            }
        }
    }
}