using System;
using System.Collections.Generic;

namespace Microsoft.AppHub.TestCloud
{
    /// <summary>
    /// Stores data required by the ITestCloudProxy.CheckFileHashesAsync REST API.
    /// </summary>
    public class CheckFileHashesRequest
    {
        public CheckFileHashesRequest(string appFile, string dSymFile, IList<string> otherFiles)
        {
            if (string.IsNullOrEmpty(appFile))
                throw new ArgumentNullException(nameof(appFile));
            if (otherFiles == null)
                throw new ArgumentNullException(nameof(otherFiles));

            this.AppFile = appFile;
            this.DSymFile = dSymFile;
            this.OtherFiles = otherFiles;
        }

        /// <summary>
        /// Path to the file with application.
        /// </summary>
        public string AppFile { get; }

        /// <summary>
        /// Optional path to the file with dSym symbols.
        /// </summary>
        /// <returns></returns>
        public string DSymFile { get; }

        /// <summary>
        /// Paths to all other files.
        /// </summary>
        public IList<string> OtherFiles { get; }
    }
}