using System;

namespace Microsoft.AppHub.TestCloud.ObjectModel
{
    /// <summary>
    /// Stores information about a file to upload to Test Cloud.
    /// </summary>
    /// <remarks>
    /// This class is immutable. To create instance with changed properties, use the "copy constructor" and pass the original object.
    /// </remarks>
    public class UploadFileInfo
    {
        /// <summary>
        /// Constructor. Creates new instance of the UploadFileInfo.
        /// </summary>
        /// <param name="fullPath">Full path to the file.</param>
        /// <param name="relativePath">Relative path to the file (i.e. path relative to the workspace directory).</param>
        /// <param name="fileHash">SHA256 hash of the file.</param>
        /// <param name="wasAlreadyUploaded">Stores information whether Test Cloud already contains this file.</param>
        public UploadFileInfo(string fullPath, string relativePath, string fileHash, bool wasAlreadyUploaded = false)
        {
            if (string.IsNullOrWhiteSpace(fullPath))
                throw new ArgumentNullException(nameof(fullPath));
            if (relativePath == null)
                throw new ArgumentNullException(nameof(relativePath));
            if (string.IsNullOrWhiteSpace(fileHash))
                throw new ArgumentNullException(nameof(fileHash));

            this.FullPath = fullPath;
            this.RelativePath = relativePath;
            this.FileHash = fileHash;
            this.WasAlreadyUploaded = wasAlreadyUploaded;
        }

        /// <summary>
        /// "Copy constructor". Creates new instance of the UploadFileInfo based on existing object.
        /// </summary>
        public UploadFileInfo(UploadFileInfo source, string fullPath = null, string relativePath = null, string fileHash = null, bool? wasAlreadyUploaded = false)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            this.FullPath = fullPath ?? source.FullPath;
            this.RelativePath = relativePath ?? source.RelativePath;
            this.FileHash = fileHash ?? source.FileHash;
            this.WasAlreadyUploaded = wasAlreadyUploaded ?? source.WasAlreadyUploaded;
        }

        /// <summary>
        /// Full path to the file.
        /// </summary>
        public string FullPath { get; }

        /// <summary>
        /// Relative path to the file (i.e. path relative to the workspace directory).
        /// </summary>
        public string RelativePath { get; }

        /// <summary>
        /// SHA256 hash of the file.
        /// </summary>
        public string FileHash { get; }

        /// <summary>
        /// Stores information whether Test Cloud already contains this file.
        /// </summary>
        public bool WasAlreadyUploaded { get; }
    }
}