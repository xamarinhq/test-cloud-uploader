using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.AppHub.TestCloud
{
    /// <summary>
    /// Represents a result of checking whether uploadFiles were already uploaded to the Test Cloud.
    /// </summary>
    public class CheckHashesResult
    {
        /// <summary>
        /// Constructor. Creates new instance of CheckHashesResult.
        /// </summary>
        /// <param name="appFile">Information about the app file.</param>
        /// <param name="dSymFile">Information about the dSym file.</param>
        /// <param name="uploadFiles">Upload files with information whether each file was already uploaded to Test Cloud.</param>
        public CheckHashesResult(UploadFileInfo appFile, UploadFileInfo dSymFile, IEnumerable<UploadFileInfo> uploadFiles)
        {
            if (appFile == null)
                throw new ArgumentNullException(nameof(appFile));
            if (uploadFiles == null)
                throw new ArgumentNullException(nameof(uploadFiles));

            this.AppFile = appFile;
            this.DSymFile = dSymFile;
            this.UploadFiles = uploadFiles.ToList();
        }

        /// <summary>
        /// Information about the app file.
        /// </summary>
        public UploadFileInfo AppFile { get; }

        /// <summary>
        /// Information about the dSYM file.
        /// </summary>
        public UploadFileInfo DSymFile { get; }

        /// <summary>
        /// Upload files with information whether each file was already uploaded to Test Cloud.
        /// </summary>
        public IList<UploadFileInfo> UploadFiles { get; }
    }
}