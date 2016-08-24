using System;
using System.Collections.Generic;

namespace Microsoft.AppHub.TestCloud
{
    /// <summary>
    /// Represents a result of checking whether files were already uploaded to the Test Cloud.
    /// </summary>
    public class CheckHashesResult
    {
        public CheckHashesResult()
        {
            this.Files = new Dictionary<string, SingleFileCheckHashResult>();
        }

        public CheckHashesResult(IEnumerable<KeyValuePair<string, SingleFileCheckHashResult>> files)
        {
            if (files == null)
                throw new ArgumentNullException(nameof(files));

            this.Files = new Dictionary<string, SingleFileCheckHashResult>();
            
            foreach (var kv in files)
            {
                this.Files.Add(kv.Key, kv.Value);
            }
        }

        /// <summary>
        /// Results for individual files. Keys in the dictionary are file paths.
        /// </summary>
        public IDictionary<string, SingleFileCheckHashResult> Files { get; }
    }
}