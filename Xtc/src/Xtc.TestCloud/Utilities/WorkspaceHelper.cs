using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.Xtc.Common.Cli.Commands;
using Microsoft.Xtc.TestCloud.Commands;
using Microsoft.Xtc.TestCloud.Utilities;
using Microsoft.Xtc.TestCloud.ObjectModel;

namespace Microsoft.Xtc.TestCloud.Utilities
{
    /// <summary>
    /// Helper class for common workspace functions
    /// </summary>
    public class WorkspaceHelper
    {
        /// <summary>
        /// Convenience method for determining if workspace is an Appium Workspace
        /// </summary>
        /// <param name="workspacePath">Path to the workspace directory.</param>
        /// <return>true if the workspace is appropriate for an appium test
        public static bool IsAppiumWorkspace(string workspacePath)
        {
            if (workspacePath == null)
                throw new ArgumentNullException(nameof(workspacePath));
            
            //Currently defined as the presence of a pom.xml file
            return File.Exists(Path.Combine(workspacePath, "pom.xml"));
        }

        /// <summary>
        /// Convenience method for determining if workspace is an Espresso Workspace
        /// </summary>
        /// <param name="workspacePath">Path to the workspace directory.</param>
        /// <return>true if the workspace is appropriate for an appium test
        public static bool IsEspressoWorkspace(string workspacePath)
        {
            if (workspacePath == null)
                throw new ArgumentNullException(nameof(workspacePath));

            try 
            {
                new EspressoWorkspace(workspacePath).Validate();
            }
            catch (CommandException) 
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Convenience method for determining if workspace is an XCUITest Workspace
        /// </summary>
        /// <param name="workspacePath">Path to the workspace directory.</param>
        /// <return>true if the workspace is appropriate for an appium test
        public static bool IsXCUITestWorkspace(string workspacePath)
        {
            if (workspacePath == null)
                throw new ArgumentNullException(nameof(workspacePath));

            try 
            {
                new XCUITestWorkspace(workspacePath).Validate();
            }
            catch (CommandException) 
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Returns all files from the workspace that should be uploaded.
        /// </summary>
        /// <returns>List with all files from the workspace directory.</returns>
        public static IList<UploadFileInfo> GetUploadFiles(IWorkspace workspace, HashAlgorithm hashAlgorithm)
        {
            string workspacePath = workspace.WorkspacePath();

            if (hashAlgorithm == null)
                throw new ArgumentNullException(nameof(hashAlgorithm));

            return Directory
                .GetFiles(workspacePath, "*", SearchOption.AllDirectories)
                .Select(filePath =>
                {
                    var relativePath = FileHelper.GetRelativePath(filePath, workspacePath);
                    var hash = hashAlgorithm.GetFileHash(filePath);

                    return new UploadFileInfo(filePath, relativePath, hash);
                })
                .ToList();
        }
    }
}