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
    public class AppiumWorkspace : IWorkspace
    {
        private readonly string _workspacePath;

        /// <summary>
        /// Constructor. Creates new instance of the AppiumWorkspace..
        /// </summary>
        /// <param name="workspacePath">Path to the workspace directory.</param>
        public AppiumWorkspace(string workspacePath)
        {
            if (workspacePath == null)
                throw new ArgumentNullException(nameof(workspacePath));

            _workspacePath = workspacePath;
        }

        /// <summary>
        /// Convenience method for determining if workspace is an Appium Workspace
        /// </summary>
        /// <param name="workspacePath">Path to the workspace directory.</param>
        /// <return>true if the workspace is appropriate for an appium test
        public static bool IsAppiumWorkspace(string workspacePath)
        {
            if (workspacePath == null)
                throw new ArgumentNullException(nameof(workspacePath));
            
            AppiumWorkspace ws = new AppiumWorkspace(workspacePath);
            
            //Currently defined as the presence of a pom.xml file
            return ws.PomFileExists();
        }

        /// <summary>
        /// Path to the workspace directory.
        /// </summary>
        public string WorkspacePath()
        {
            return _workspacePath;
        }

        /// <summary>
        /// Verifies that the workspace is a valid directory with Appium tests.
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        public void Validate()
        {
            ValidatePomFile();
            ValidateDependencyJarsDirectory();
            ValidateTestClassesDirectory();
        }

        private void ValidatePomFile()
        {
            if (!PomFileExists())
            {
                throw new CommandException(
                    UploadTestsCommand.CommandName,
                    "The Appium workspace directory must contain file \"pom.xml\"",
                    (int)UploadCommandExitCodes.InvalidWorkspace);
            }
        }

        private bool PomFileExists()
        {
            var pomFilePath = Path.Combine(_workspacePath, "pom.xml");
            return File.Exists(pomFilePath);
        }

        private void ValidateDependencyJarsDirectory()
        {
            var dependencyJarsPath = Path.Combine(_workspacePath, "dependency-jars");
            if (!Directory.Exists(dependencyJarsPath))
            {
                throw new CommandException(
                    UploadTestsCommand.CommandName,
                    "The Appium workspace directory must contain directory \"dependency-jars\"",
                    (int)UploadCommandExitCodes.InvalidWorkspace);
            }
        }

        private void ValidateTestClassesDirectory()
        {
            var testClassesPath = Path.Combine(_workspacePath, "test-classes");
            if (!Directory.Exists(testClassesPath))
            {
                throw new CommandException(
                    UploadTestsCommand.CommandName,
                    "The Appium workspace directory must contain directory \"test-classes\"",
                    (int)UploadCommandExitCodes.InvalidWorkspace);
            }

            var classFiles = Directory.GetFiles(testClassesPath, "*.class", SearchOption.AllDirectories);
            if (classFiles.Length == 0)
            {
                throw new CommandException(
                    UploadTestsCommand.CommandName,
                    "Test Appium workspace directory must contain at least one *.class files in directory \"test-classes\"",
                    (int)UploadCommandExitCodes.InvalidWorkspace);
            }
        }
    }
}