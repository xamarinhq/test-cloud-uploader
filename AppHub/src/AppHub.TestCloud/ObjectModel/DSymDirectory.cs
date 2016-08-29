﻿using System;
using System.IO;
using System.Linq;
using Microsoft.AppHub.Cli;

namespace Microsoft.AppHub.TestCloud
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
        /// <param name="directoryPath">Path to the directory.</param>
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
                throw new CommandException(UploadTestsCommand.CommandName, $"Directory with dSYM files \"{_directoryPath}\" doesn't exist.");

            if (!".dsym".Equals(Path.GetExtension(_directoryPath), StringComparison.OrdinalIgnoreCase))
                throw new CommandException(UploadTestsCommand.CommandName, "Directory with dSYM files must have the extension \".dsym\".");

            var dwarfPath = Path.GetFullPath(Path.Combine(_directoryPath, "Contents", "Resources", "DWARF"));
            var dwarfFiles = Directory.EnumerateFiles(dwarfPath).ToArray();

            if (dwarfFiles.Length != 1)
                throw new CommandException(UploadTestsCommand.CommandName, "Directory with dSYM files contains more than one file in Contents/Resoruces/DWARF");

            return dwarfFiles[0];
        }
    }
}