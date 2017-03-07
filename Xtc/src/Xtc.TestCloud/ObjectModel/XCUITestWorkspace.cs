using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Microsoft.Xtc.Common.Cli.Commands;
using Microsoft.Xtc.TestCloud.Commands;
using Microsoft.Xtc.TestCloud.Utilities;

namespace Microsoft.Xtc.TestCloud.ObjectModel
{
    /// <summary>
    /// Represents an XCUITest workspace directory.
    /// </summary>
    public class XCUITestWorkspace : IWorkspace
    {
        private readonly string _workspacePath;

        public static string FindAUT(string workspacePath)
        {
            var regex = new Regex("^*[^-Runner].app$");
            var apps =  Directory.GetDirectories(workspacePath)
                        .Where(path => regex.IsMatch(path))
                        .ToArray();
            return apps.Length > 0 ? apps[0] : null;
        }

        /// <summary>
        /// Constructor. Creates new instance of the XCUITest Workspace.
        /// </summary>
        /// <param name="workspacePath">Path to the workspace directory.</param>
        public XCUITestWorkspace(string workspacePath)
        {
            if (workspacePath == null)
                throw new ArgumentNullException(nameof(workspacePath));

            _workspacePath = workspacePath;
        }

        /// <summary>
        /// Verifies that the workspace is a valid directory for Espresso tests.
        /// </summary>
        public void Validate() 
        {
            ValidateContainsXCUITestBundle();
        }

        /// <summary>
        /// Path to the workspace directory.
        /// </summary>
        public string WorkspacePath()
        {
            return _workspacePath;
        }

        /// <summary>
        /// Returns all files from the workspace that should be uploaded.
        /// </summary>
        /// <returns>List of files from the workspace directory that should be uploaded.</returns>
        public IList<UploadFileInfo> GetFilesToUpload(HashAlgorithm hashAlgorithm)
        {
            string workspacePath = WorkspacePath();

            if (hashAlgorithm == null)
                throw new ArgumentNullException(nameof(hashAlgorithm));
            
            var testRunnerApp = Directory.GetDirectories(workspacePath, "*-Runner.app", SearchOption.TopDirectoryOnly).Single();
            var testRunnerIpa = FileHelper.ArchiveAppBundle(testRunnerApp);
            var relativePath = FileHelper.GetRelativePath(testRunnerIpa, workspacePath);
            var hash = hashAlgorithm.GetFileHash(testRunnerIpa);

            return new List<UploadFileInfo> {new UploadFileInfo(testRunnerIpa, relativePath, hash)};
        }


        protected void ValidateContainsXCUITestBundle()
        {
            var testRunners = Directory.GetDirectories(_workspacePath, "*-Runner.app", SearchOption.TopDirectoryOnly);
            if (testRunners.Length != 1)
            {
                throw new CommandException(
                    UploadTestsCommand.CommandName,
                    "XCUITestWorkspace directory must contain exactly one test runner (should end in '-Runner.ipa')",
                    (int)UploadCommandExitCodes.InvalidWorkspace);
            }
        }
    }
}