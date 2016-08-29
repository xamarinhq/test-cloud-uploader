using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Microsoft.AppHub.Cli;

namespace Microsoft.AppHub.TestCloud
{
    public static class ValidationHelper
    {
        /// <summary>
        /// Checks whether an Android app uses shared runtime. 
        /// </summary>
        /// <param name="appPath">workspacePath to the *.apk with Android app.</param>
        /// <returns>True if the app uses shared runtime; otherwise, returns false.</returns>
        public static bool UsesSharedRuntime(string appPath)
        {
            if (string.IsNullOrWhiteSpace(appPath))
                throw new ArgumentNullException(nameof(appPath));

            var archive = ZipFile.OpenRead(appPath);

            var monodroid = archive.Entries.Any(x => x.Name.EndsWith("libmonodroid.so"));
            var hasRuntime = archive.Entries.Any(x => x.Name.EndsWith("mscorlib.dll"));
            var hasEnterpriseBundle = archive.Entries.Any(x => x.Name.EndsWith("libmonodroid_bundle_app.so"));
                
            return monodroid && !hasRuntime && !hasEnterpriseBundle;
        }

        /// <summary>
        /// Checkes whether given workspacePath represents an Android app.
        /// </summary>
        /// <param name="appPath">workspacePath to the app file.</param>
        /// <returns>True if the workspacePath points to an Android app; otherwise, returns false.</returns>
        public static bool IsAndroidApp(string appPath)
        {
            return ".apk".Equals(Path.GetExtension(appPath), StringComparison.Ordinal);
        }

        /// <summary>
        /// Checkes whether given workspacePath represents an iOS app.
        /// </summary>
        /// <param name="appPath">workspacePath to the app file.</param>
        /// <returns>True if the workspacePath points to an iOS app; otherwise, returns false.</returns>
        public static bool IsIosApp(string appPath)
        {
            return ".ipk".Equals(Path.GetExtension(appPath), StringComparison.Ordinal);
        }
    }
}