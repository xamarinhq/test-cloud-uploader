using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Xtc.Common.Services;
using System.IO.Compression;

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
            if (!Path.GetExtension(appBundlePath).Equals(".app"))
            {
                throw new ArgumentException(
                    $@"Expected bundle to end with .app, got ${appBundlePath}");
            }

			if (!Directory.Exists(appBundlePath))
			{
				throw new FileNotFoundException(
				  $@"'${appBundlePath}' does not exist or is not a directory");
			}

			var platformService = new PlatformService();
            if (platformService.CurrentPlatform == OSPlatform.OSX && false) 
            {
				return CreateIpaWithDitto(appBundlePath);
            } 
            else
            {
                return CreateIpaWithZip(appBundlePath);   
            }
        }

        private static string CreateIpaWithZip(string appBundlePath)
        {
            // /tmp/<uuid>
            string tmpDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            // /tmp/<uuid>/Payload/MyApp.app
            string destination = CreateTempAppDestination(appBundlePath, tmpDir);

            try
            {
                Copy(appBundlePath, destination);
				var ipaFilePath = Path.ChangeExtension(appBundlePath, ".ipa");
                ZipFile.CreateFromDirectory(tmpDir, ipaFilePath);
                return ipaFilePath;
            }
            finally
            {
                Directory.Delete(tmpDir, true);
            }
        }

		public static void Copy(string sourceDirectory, string targetDirectory)
		{
			DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
			DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);

			CopyAll(diSource, diTarget);
		}

		public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
		{
			Directory.CreateDirectory(target.FullName);

			foreach (FileInfo fi in source.GetFiles())
			{
				fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
			}

			foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
			{
				DirectoryInfo nextTargetSubDir =
					target.CreateSubdirectory(diSourceSubDir.Name);
				CopyAll(diSourceSubDir, nextTargetSubDir);
			}
		}

        private static string CreateIpaWithDitto(string appBundlePath)
        {
            // /tmp/<uuid>
            string tmpDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
			// /tmp/<uuid>/Payload/MyApp.app
			string destination = CreateTempAppDestination(appBundlePath, tmpDir);

            try
            {
                var processService = new ProcessService();
                // ditto path/to/MyApp.app /tmp/<uuid>/Payload/MyApp.app
                var dittoTask = processService.RunAsync("ditto", $"{appBundlePath} {destination}");
                dittoTask.Wait();
                var result = dittoTask.Result;
                if (result.ExitCode != 0)
                {
                    throw new Exception(
                        $@"Unable to copy application bundle into Payload dir with command:\n
                    ditto {appBundlePath} {destination}");
                }

                // path/to/MyApp.ipa
                var ipaFilePath = Path.ChangeExtension(appBundlePath, ".ipa");

                // ditto -ck --sequesterRsrc /tmp/<uuid> path/to/MyApp.ipa
                dittoTask = processService.RunAsync("ditto", $"-ck --sequesterRsrc {tmpDir} {ipaFilePath}");
                dittoTask.Wait();
                result = dittoTask.Result;
                if (result.ExitCode != 0)
                {
                    throw new Exception($@"Unable archive file with command:\n
                ck --sequesterRsrc {tmpDir} {ipaFilePath}");
                }

                return ipaFilePath;
            }
            finally
            {
                Directory.Delete(tmpDir, true);
            }
        }

        private static string CreateTempAppDestination(string appBundlePath, string tmpDir)
        {
            Directory.CreateDirectory(tmpDir);

            // /tmp/<uuid>/Payload
            var payloadPath = Path.Combine(tmpDir, "Payload");
            Directory.CreateDirectory(payloadPath);

            // /tmp/<uuid>/Payload/MyApp.app
            var destination = Path.Combine(payloadPath, Path.GetFileName(appBundlePath));
            return destination;
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