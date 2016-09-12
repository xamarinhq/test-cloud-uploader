using System;
using System.Collections.Generic;

namespace Microsoft.Xtc.TestCloud.ObjectModel.MultipartContent
{
    /// <summary>
    /// A multi-part tree node that represents a list.
    /// </summary>
    public class ListContentBuilderPart: IContentBuilderPart
    {
        private readonly IList<IContentBuilderPart> _items;

        public ListContentBuilderPart(IEnumerable<IContentBuilderPart> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            _items = new List<IContentBuilderPart>(items);
        }

        public ListContentBuilderPart(params IContentBuilderPart[] items)
        {
            _items = new List<IContentBuilderPart>(items);
        }

        /// <summary>
        /// Adds child element to the node.
        /// </summary>
        /// <param name="childPart">Child element</param>
        public void AddChild(IContentBuilderPart childPart)
        {
            if (childPart == null)
                throw new ArgumentNullException(nameof(childPart));

            _items.Add(childPart);
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

            var fullName = $"{parentName}[]";

            foreach (var childPart in _items)
            {
                childPart.BuildMultipartContent(fullName, result);
            }
        }
    }
}