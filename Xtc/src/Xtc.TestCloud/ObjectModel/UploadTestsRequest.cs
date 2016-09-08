using System;
using System.Collections.Generic;

namespace Microsoft.Xtc.TestCloud.ObjectModel
{
    /// <summary>
    /// Stores data required by the ITestCloudProxy.UploadTests REST API.
    /// </summary>
    public class UploadTestsRequest
    {
        public UploadTestsRequest(UploadFileInfo appFile, UploadFileInfo dSymFile, IList<UploadFileInfo> otherFiles)
        {
            if (appFile == null)
                throw new ArgumentNullException(nameof(appFile));
            if (otherFiles == null)
                throw new ArgumentNullException(nameof(otherFiles));

            this.AppFile = appFile;
            this.DSymFile = dSymFile;
            this.OtherFiles = new List<UploadFileInfo>(otherFiles);
            this.TestCloudOptions = new Dictionary<string, string>();
            this.TestParameters = new Dictionary<string, string>();
        }

        /// <summary>
        /// workspacePath to the app file.
        /// </summary>
        public UploadFileInfo AppFile { get; }

        /// <summary>
        /// workspacePath to the DSym file
        /// </summary>
        public UploadFileInfo DSymFile { get; }

        /// <summary>
        /// List of other file paths (i.e. files other than app and dSym).
        /// </summary>
        public IList<UploadFileInfo> OtherFiles { get; }

        /// <summary>
        /// Options passed internally by the Test Cloud uploader. 
        /// </summary>
        public IDictionary<string, string> TestCloudOptions { get; }

        /// <summary>
        /// Additional test parameters provided by the user.
        /// </summary>
        public IDictionary<string, string> TestParameters { get; }
    }
}