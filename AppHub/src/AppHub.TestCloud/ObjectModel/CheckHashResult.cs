using System;

namespace Microsoft.AppHub.TestCloud
{
    /// <summary>
    /// Represents a result of checking whether a single file was already uploaded to Test Cloud.
    /// </summary>
    public class CheckHashResult
    {
        public CheckHashResult(string filePath, string fileHash, bool alreadyUploaded)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentNullException(nameof(filePath));
            if (string.IsNullOrWhiteSpace(fileHash))
                throw new ArgumentNullException(nameof(fileHash));

            this.FilePath = filePath;
            this.FileHash = fileHash;
            this.AlreadyUploaded = alreadyUploaded;
        }

        public string FilePath { get; }

        public string FileHash { get; }

        public bool AlreadyUploaded { get; }
    }
}