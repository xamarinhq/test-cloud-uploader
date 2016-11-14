using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.Xtc.Common.Cli.Commands;
using Microsoft.Xtc.TestCloud.Commands;
using Microsoft.Xtc.TestCloud.Utilities;

namespace Microsoft.Xtc.TestCloud.ObjectModel
{
    /// <summary>
    /// Represents a test workspace
    /// </summary>
    public interface IWorkspace
    {
        /// <summary>
        /// Path to the workspace directory.
        /// </summary>
        string WorkspacePath();

        /// <summary>
        /// Validates the workspace for a given test type
        /// </summary>
        void Validate();

        /// <summary>
        /// What files to upload
        /// </summary>
        IList<UploadFileInfo> GetFilesToUpload(HashAlgorithm hashAlgorithm);
    } 
}