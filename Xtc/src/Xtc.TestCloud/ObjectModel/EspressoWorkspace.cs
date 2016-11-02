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
    /// Represents an Appium workspace directory.
    /// </summary>
    public class EspressoWorkspace : IWorkspace
    {
        private readonly string _workspacePath;

        /// <summary>
        /// Constructor. Creates new instance of the AppiumWorkspace..
        /// </summary>
        /// <param name="workspacePath">Path to the workspace directory.</param>
        public EspressoWorkspace(string workspacePath)
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
            ValidateContainsOneApk();
        }

        /// <summary>
        /// Path to the workspace directory.
        /// </summary>
        public string WorkspacePath()
        {
            return _workspacePath;
        }

        protected void ValidateContainsOneApk()
        {
            var apks = Directory.GetFiles(_workspacePath, "*.apk", SearchOption.TopDirectoryOnly);
            if (apks.Length != 1)
            {
                throw new CommandException(
                    UploadTestsCommand.CommandName,
                    "Espresso workspace directory must contain exactly one apk file",
                    (int)UploadCommandExitCodes.InvalidWorkspace);
            }
        }
    }
}