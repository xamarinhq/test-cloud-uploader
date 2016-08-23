using System;
using System.IO;

namespace Microsoft.AppHub.TestCloud
{
    public static class FileHelper
    {
        /// <summary>
        /// Gets a file path relative to a given directory. 
        /// </summary>
        /// <param name="filePath">Full file path.</param>
        /// <param name="rootDirectoryPath">Full directory path.</param>
        /// <returns>A file path relative to given directory.</returns>
        public static string GetRelativePath(string filePath, string rootDirectoryPath)
        {
            if (filePath == null)
                throw new ArgumentNullException(nameof(filePath));
            if (rootDirectoryPath == null)
                throw new ArgumentNullException(nameof(rootDirectoryPath));

            var rootDirectoryUri = new Uri(AppendDirectorySeparatorChar(rootDirectoryPath));
            var fileUri = new Uri(filePath);
            var relativeUri = rootDirectoryUri.MakeRelativeUri(fileUri);

            return Uri.UnescapeDataString(relativeUri.ToString());
        }

        private static string AppendDirectorySeparatorChar(string path)
        {
            if (!Path.HasExtension(path) && !path.EndsWith(Path.DirectorySeparatorChar.ToString()))
                return path + Path.DirectorySeparatorChar;
            else
                return path;
        }
    }
}