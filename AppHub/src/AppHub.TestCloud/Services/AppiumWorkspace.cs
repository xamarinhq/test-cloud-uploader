using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.AppHub.Cli;

namespace Microsoft.AppHub.TestCloud
{
    /// <summary>
    /// Represents an Appium workspace directory.
    /// </summary>
    public class AppiumWorkspace : ITestWorkspace
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
        public void Validate()
        {
            ValidatePomFile(true);
            ValidateDependencyJarsDirectory(true);
            ValidateTestClassesDirectory(true);
        }

        public bool IsValid()
        {
            return ValidatePomFile(false) &&
                   ValidateDependencyJarsDirectory(false) &&
                   ValidateTestClassesDirectory(false);
        }

        private bool ValidatePomFile(bool throwException)
        {
            var pomFilePath = Path.Combine(_workspacePath, "pom.xml");
            if (!File.Exists(pomFilePath))
            {
                return ThrowOrReturnFalse(
                    throwException,
                    "The Appium workspace directory must contain file \"pom.xml\"");
            }

            return true;
        }

        private bool ValidateDependencyJarsDirectory(bool throwException)
        {
            var dependencyJarsPath = Path.Combine(_workspacePath, "dependency-jars");
            if (!Directory.Exists(dependencyJarsPath))
            {
                return ThrowOrReturnFalse(
                    throwException,
                    "The Appium workspace directory must contain directory \"dependency-jars\"");
            }

            return true;
        }

        private bool ValidateTestClassesDirectory(bool throwException)
        {
            var testClassesPath = Path.Combine(_workspacePath, "test-classes");
            if (!Directory.Exists(testClassesPath))
            {
                return ThrowOrReturnFalse(
                    throwException,
                    "The Appium workspace directory must contain directory \"test-classes\"");
            }

            var classFiles = Directory.GetFiles(testClassesPath, "*.class", SearchOption.AllDirectories);
            if (classFiles.Length == 0)
            {
                return ThrowOrReturnFalse(
                    throwException,
                    "Test Appium workspace directory must contain at least one *.class files in directory \"test-classes\"");
            }

            return true;
        }

        private bool ThrowOrReturnFalse(bool throwException, string errorMessage)
        {
            if (throwException)
                throw new CommandException(UploadTestsCommand.CommandName, errorMessage);

            return false;
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