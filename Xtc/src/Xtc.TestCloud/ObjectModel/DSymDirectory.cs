using System;
using System.IO;
using System.Linq;
using Microsoft.Xtc.Common.Cli.Commands;
using Microsoft.Xtc.TestCloud.Commands;

namespace Microsoft.Xtc.TestCloud.ObjectModel
{
    /// <summary>
    /// Represents a dSYM directory.
    /// </summary>
    public class DSymDirectory
    {
        private readonly string _directoryPath;
        private string _dSymFile;

        /// <summary>
        /// Constructors. Creates new instance of DSymDirectory.
        /// </summary>
        /// <param name="directoryPath">workspacePath to the directory.</param>
        public DSymDirectory(string directoryPath)
        {
            if (directoryPath == null)
                throw new ArgumentNullException(nameof(directoryPath));

            _directoryPath = directoryPath;
        }

        /// <summary>
        /// Validates that the directory points to the valid DSym directory.
        /// </summary>
        public void Validate()
        {
            if (_dSymFile == null)
                _dSymFile = ValidateAndGetDSymFile();
        }

        /// <summary>
        /// Returns the single dSYM file from the directory.
        /// </summary>
        /// <returns>The single symbol file from Contents/Resources/DWARF.</returns>
        public string GetDSymFile()
        {
            if (_dSymFile == null)
                _dSymFile = ValidateAndGetDSymFile();

            return _dSymFile;
        }

        private string ValidateAndGetDSymFile()
        {
            if (!Directory.Exists(_directoryPath))
            {
                throw new CommandException(
                    UploadTestsCommand.CommandName, 
                    $"Directory with dSYM files \"{_directoryPath}\" doesn't exist.",
                    (int)UploadCommandExitCodes.InvalidDSymDirectory);
            }

            if (!".dsym".Equals(Path.GetExtension(_directoryPath), StringComparison.OrdinalIgnoreCase))
            {
                throw new CommandException(
                    UploadTestsCommand.CommandName, 
                    "Directory with dSYM files must have the extension \".dsym\".",
                    (int)UploadCommandExitCodes.InvalidDSymDirectory);
            }

            var dwarfPath = Path.GetFullPath(Path.Combine(_directoryPath, "Contents", "Resources", "DWARF"));
            var dwarfFiles = Directory.EnumerateFiles(dwarfPath).ToArray();

            if (dwarfFiles.Length != 1)
            {
                throw new CommandException(
                    UploadTestsCommand.CommandName, 
                    "Directory with dSYM files contains more than one file in Contents/Resoruces/DWARF",
                    (int)UploadCommandExitCodes.InvalidDSymDirectory);
            }

            return dwarfFiles[0];
        }
    }
}