using System;
using System.Collections.Generic;

namespace Microsoft.AppHub.TestCloud
{
    /// <summary>
    /// Stores data required by the ITestCloudProxy.CheckFileHashesAsync REST API.
    /// </summary>
    public class CheckFileHashesRequest
    {
        public CheckFileHashesRequest(UploadFileInfo appFile, UploadFileInfo dSymFile, IList<UploadFileInfo> otherFiles)
        {
            if (appFile == null)
                throw new ArgumentNullException(nameof(appFile));
            if (otherFiles == null)
                throw new ArgumentNullException(nameof(otherFiles));

            this.AppFile = appFile;
            this.DSymFile = dSymFile;
            this.OtherFiles = otherFiles;
        }

        /// <summary>
        /// workspacePath to the file with application.
        /// </summary>
        public UploadFileInfo AppFile { get; }

        /// <summary>
        /// Optional workspacePath to the file with dSym symbols.
        /// </summary>
        /// <returns></returns>
        public UploadFileInfo DSymFile { get; }

        /// <summary>
        /// Paths to all other files.
        /// </summary>
        public IList<UploadFileInfo> OtherFiles { get; }
    }
}