using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Microsoft.AppHub.TestCloud
{
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

        public void AddChild(IContentBuilderPart childPart)
        {
            if (childPart == null)
                throw new ArgumentNullException(nameof(childPart));

            _items.Add(childPart);
        }

        public void BuildMultipartContent(string parentName, MultipartContent result)
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