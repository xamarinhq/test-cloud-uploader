using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.AppHub.Common.Services;

namespace Microsoft.AppHub.TestCloud.Utilities
{
    public static class FileHelper
    {
        /// <summary>
        /// Gets a file workspacePath relative to a given directory. 
        /// </summary>
        /// <param name="filePath">Full file workspacePath.</param>
        /// <param name="rootDirectoryPath">Full directory workspacePath.</param>
        /// <param name="platformService">(Optional) A platform service that should be used to detect the current OS</param>
        /// <returns>A file workspacePath relative to given directory.</returns>
        public static string GetRelativePath(string filePath, string rootDirectoryPath, IPlatformService platformService = null)
        {
            if (filePath == null)
                throw new ArgumentNullException(nameof(filePath));
            if (rootDirectoryPath == null)
                throw new ArgumentNullException(nameof(rootDirectoryPath));

            if (platformService == null)
                platformService = PlatformService.Instance;

            var filePathSegments = GetPathSegments(filePath);
            var rootDirectoryPathSegments = GetPathSegments(rootDirectoryPath);

            var commonPrefixLength = FindLongestPathPrefix(filePathSegments, rootDirectoryPathSegments, platformService);

            if (commonPrefixLength == 0)
                return filePath.Replace("\\", "/");
            else
                return ConcatenatePathSegments(filePathSegments.Skip(commonPrefixLength));
        }

        private static string[] GetPathSegments(string path)
        {
            return path.Split(new[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries);
        }

        private static string ConcatenatePathSegments(IEnumerable<string> segments)
        {
            var result = new StringBuilder();

            foreach (var segment in segments)
            {
                if (result.Length > 0)
                    result.Append("/");

                result.Append(segment);
            }

            return result.ToString();
        }

        private static int FindLongestPathPrefix(string[] pathSegmentsA, string[] pathSegmentsB, IPlatformService platformService)
        {
            var minLength = Math.Min(pathSegmentsA.Length, pathSegmentsB.Length);

            for (var i = 0; i < minLength; i++)
            {
                var stringComparision = platformService.CurrentPlatform == OSPlatform.Windows ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
                if (!string.Equals(pathSegmentsA[i], pathSegmentsB[i], stringComparision))
                    return i;
            }

            return minLength;
        }
    }
}