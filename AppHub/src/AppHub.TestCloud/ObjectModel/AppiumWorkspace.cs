using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.AppHub.TestCloud;

namespace AppHub.TestCloud.ObjectModel
{
    /// <summary>
    /// Represents an Appium workspace directory.
    /// </summary>
    public class AppiumWorkspace
    {
        private readonly string _workspacePath;

        /// <summary>
        /// Constructor. Creates new instance of the AppiumWorkspace..
        /// </summary>
        /// <param name="workspacePath">workspacePath to the workspace directory.</param>
        public AppiumWorkspace(string workspacePath)
        {
            if (workspacePath == null)
                throw new ArgumentNullException(nameof(workspacePath));

            _workspacePath = workspacePath;
        }

        /// <summary>
        /// workspacePath to the workspace directory.
        /// </summary>
        public string WorkspacePath
        {
            get { return _workspacePath; }
        }

        /// <summary>
        /// Verifies that the workspace is a valid directory with Appium tests.
        /// </summary>
        public void Validate()
        {
        }

        /// <summary>
        /// Returns all files from the workspace that should be uploaded.
        /// </summary>
        /// <returns>List with all files from the workspace directory.</returns>
        public IList<UploadFileInfo> GetUploadFiles(HashAlgorithm hashAlgorithm)
        {
            if (hashAlgorithm == null)
                throw new ArgumentNullException(nameof(hashAlgorithm));

            return Directory
                .GetFiles(_workspacePath, "*", SearchOption.AllDirectories)
                .Select(filePath =>
                {
                    var relativePath = FileHelper.GetRelativePath(filePath, _workspacePath);
                    var hash = hashAlgorithm.GetFileHash(filePath);

                    return new UploadFileInfo(filePath, relativePath, hash);
                })
                .ToList();
        }
    }
}