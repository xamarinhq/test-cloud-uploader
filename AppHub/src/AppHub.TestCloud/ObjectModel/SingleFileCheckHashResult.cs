using System;

namespace Microsoft.AppHub.TestCloud
{
    /// <summary>
    /// Represents a result of checking whether a single file was already uploaded to Test Cloud.
    /// </summary>
    public class SingleFileCheckHashResult
    {
        public SingleFileCheckHashResult(string filePath, string fileHash, bool alreadyUploaded)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentNullException(nameof(filePath));
            if (string.IsNullOrWhiteSpace(fileHash))
                throw new ArgumentNullException(nameof(fileHash));

            this.FilePath = filePath;
            this.FileHash = fileHash;
            this.WasAlreadyUploaded = alreadyUploaded;
        }
        
        /// <summary>
        /// Path to the file.
        /// </summary>
        public string FilePath { get; }

        /// <summary>
        /// Hash of the file.
        /// </summary>
        public string FileHash { get; }

        /// <summary>
        /// Stores information whether Test Cloud already contains this file.
        /// </summary>
        public bool WasAlreadyUploaded { get; }
    }
}