using System;
using System.Collections.Generic;

namespace Microsoft.AppHub.TestCloud
{
    /// <summary>
    /// Stores data required by the ITestCloudProxy.UploadTests REST API.
    /// </summary>
    public class UploadTestsRequest
    {
        public UploadTestsRequest(string appFile, string dSymFile, string workspaceDirectory, IList<string> otherFiles)
        {
            if (appFile == null)
                throw new ArgumentNullException(nameof(appFile));
            if (workspaceDirectory == null)
                throw new ArgumentNullException(nameof(workspaceDirectory));
            if (otherFiles == null)
                throw new ArgumentNullException(nameof(otherFiles));
            
            this.AppFile = appFile;
            this.DSymFile = dSymFile;
            this.WorkspaceDirectory = workspaceDirectory;
            this.OtherFiles = new List<string>(otherFiles);
            this.CheckHashesResult = new CheckHashesResult();
            this.TestCloudOptions = new Dictionary<string, string>();
            this.TestParameters = new Dictionary<string, string>();
        }

        /// <summary>
        /// Path to the app file.
        /// </summary>
        public string AppFile { get; }

        /// <summary>
        /// Path to the DSym file
        /// </summary>
        public string DSymFile { get; }

        /// <summary>
        /// Path to the workspace directory, i.e. directory with tests.
        /// </summary>
        public string WorkspaceDirectory { get; }

        /// <summary>
        /// List of other file paths (i.e. files other than app and dSym).
        /// </summary>
        public IList<string> OtherFiles { get; }

        /// <summary>
        /// Result of calling CheckHashResult REST API. 
        /// </summary>
        public CheckHashesResult CheckHashesResult { get; }

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