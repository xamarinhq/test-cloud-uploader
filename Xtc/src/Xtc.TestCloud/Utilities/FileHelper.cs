using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Xtc.Common.Services;

namespace Microsoft.Xtc.TestCloud.Utilities
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
            {
                platformService = PlatformService.Instance;
            }

            var filePathSegments = GetPathSegments(filePath);
            var rootDirectoryPathSegments = GetPathSegments(rootDirectoryPath);

            var commonPrefixLength = FindLongestPathPrefix(filePathSegments, rootDirectoryPathSegments, platformService);

            if (commonPrefixLength == 0)
            {
                return filePath.Replace("\\", "/");
            }
            else
            {
                return ConcatenatePathSegments(filePathSegments.Skip(commonPrefixLength));
            }
        }

        /// <summary>
        /// Archives a .app directory into a .ipa file
        /// </summary>
        /// <param name="appBundlePath">Path to application bundle</param>
        /// <returns>Path to the resulting ipa</returns>
        public static string ArchiveAppBundle(string appBundlePath)
        {
            var platformService = new PlatformService();
            if (platformService.CurrentPlatform == OSPlatform.Windows)
            {
                throw new InvalidOperationException("Can not archive an iOS application on Windows");
            }

            // path/to/MyApp.ipa
            var ipaFilePath = Path.ChangeExtension(appBundlePath, ".ipa");

            // /tmp/<uuid>
            string tmpDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tmpDir);

            // /tmp/<uuid>/Payload
            var payloadPath = Path.Combine(tmpDir, "Payload");
            Directory.CreateDirectory(payloadPath);

            // /tmp/<uuid>/Payload/MyApp.app
            var destination = Path.Combine(payloadPath, Path.GetFileName(appBundlePath));
            var processService = new ProcessService();
        
            // ditto path/to/MyApp.app /tmp/<uuid>/Payload/MyApp.app
            var dittoTask = processService.RunAsync("ditto", $"{appBundlePath} {destination}");
            dittoTask.Wait();
            var result = dittoTask.Result;
            if (result.ExitCode != 0) {
                Directory.Delete(tmpDir, true);
                throw new Exception(
                    $@"Unable to copy application bundle into Payload dir with command:\n
                    ditto {appBundlePath} {destination}");
            }

            // ditto -ck --sequesterRsrc /tmp/<uuid> path/to/MyApp.ipa
            dittoTask = processService.RunAsync("ditto", $"-ck --sequesterRsrc {tmpDir} {ipaFilePath}");
            dittoTask.Wait();
            result = dittoTask.Result;
            if (result.ExitCode != 0) {
                Directory.Delete(tmpDir, true);
                throw new Exception($@"Unable archive file with command:\n
                ck --sequesterRsrc {tmpDir} {ipaFilePath}");
            }

            Directory.Delete(tmpDir, true);
            return ipaFilePath;
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