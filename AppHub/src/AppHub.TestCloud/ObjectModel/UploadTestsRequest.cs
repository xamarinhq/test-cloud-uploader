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
            this.CheckHashResults = new Dictionary<string, CheckHashResult>();
            this.TestCloudOptions = new Dictionary<string, string>();
            this.TestParameters = new Dictionary<string, string>();
        }

        public string AppFile { get; }

        public string DSymFile { get; }

        public string WorkspaceDirectory { get; }

        public IList<string> OtherFiles { get; }

        public IDictionary<string, CheckHashResult> CheckHashResults { get; }

        public IDictionary<string, string> TestCloudOptions { get; }

        public IDictionary<string, string> TestParameters { get; }
    }
}