using System;
using System.IO.Compression;
using System.Linq;

namespace Microsoft.AppHub.TestCloud
{
    public static class ValidationHelper
    {
        /// <summary>
        /// Checks whether an Android app uses shared runtime. 
        /// </summary>
        /// <param name="appPath">Path to the *.apk with Android app.</param>
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
    }
}