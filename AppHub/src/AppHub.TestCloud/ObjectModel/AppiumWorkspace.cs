using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.AppHub.Common.Cli.Commands;
using Microsoft.AppHub.TestCloud.Commands;
using Microsoft.AppHub.TestCloud.Utilities;

namespace Microsoft.AppHub.TestCloud.ObjectModel
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
        /// <param name="workspacePath">Path to the workspace directory.</param>
        public AppiumWorkspace(string workspacePath)
        {
            if (workspacePath == null)
                throw new ArgumentNullException(nameof(workspacePath));

            _workspacePath = workspacePath;
        }

        /// <summary>
        /// Path to the workspace directory.
        /// </summary>
        public string WorkspacePath
        {
            get { return _workspacePath; }
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
            var pomFilePath = Path.Combine(_workspacePath, "pom.xml");
            if (!File.Exists(pomFilePath))
            {
                throw new CommandException(
                    UploadTestsCommand.CommandName,
                    "The Appium workspace directory must contain file \"pom.xml\"");
            }
        }

        private void ValidateDependencyJarsDirectory()
        {
            var dependencyJarsPath = Path.Combine(_workspacePath, "dependency-jars");
            if (!Directory.Exists(dependencyJarsPath))
            {
                throw new CommandException(
                    UploadTestsCommand.CommandName,
                    "The Appium workspace directory must contain directory \"dependency-jars\"");
            }
        }

        private void ValidateTestClassesDirectory()
        {
            var testClassesPath = Path.Combine(_workspacePath, "test-classes");
            if (!Directory.Exists(testClassesPath))
            {
                throw new CommandException(
                    UploadTestsCommand.CommandName,
                    "The Appium workspace directory must contain directory \"test-classes\"");
            }

            var classFiles = Directory.GetFiles(testClassesPath, "*.class", SearchOption.AllDirectories);
            if (classFiles.Length == 0)
            {
                throw new CommandException(
                    UploadTestsCommand.CommandName,
                    "Test Appium workspace directory must contain at least one *.class files in directory \"test-classes\"");
            }
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