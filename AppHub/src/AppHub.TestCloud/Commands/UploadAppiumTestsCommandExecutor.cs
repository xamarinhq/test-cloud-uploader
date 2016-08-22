using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AppHub.Cli;
using Microsoft.AppHub.Common;
using Microsoft.Extensions.Logging;

namespace Microsoft.AppHub.TestCloud
{
    public class UploadAppiumTestsCommandExecutor: ICommandExecutor
    {
        private static readonly Uri _testCloudUri = new Uri("https://testcloud.xamarin.com/ci");

        private readonly UploadTestsCommandOptions _options;
        private readonly TestCloudProxy _testCloudProxy;

        public UploadAppiumTestsCommandExecutor(UploadTestsCommandOptions options, ILoggerService loggerService)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            if (loggerService == null)
                throw new ArgumentNullException(nameof(loggerService));

            _options = options;
            _testCloudProxy = new TestCloudProxy(_testCloudUri, loggerService);
        }

        public async Task ExecuteAsync()
        {
            var appFileInfo = new FileInfo(_options.AppFile);
            
            var allFileInfos = (new[] { _options.AppFile }).Select(file => new FileInfo(file)).ToArray();

            using (var sha256 = SHA256.Create())
            {
                var appFileHash = sha256.GetHash(appFileInfo);
                var hashToFileName = allFileInfos.ToDictionary(
                    fi => sha256.GetHash(fi).ToLowerInvariant(),
                    fi => fi.FullName,
                    StringComparer.OrdinalIgnoreCase);
                
                var result = await _testCloudProxy.CheckFileHashesAsync(appFileHash, hashToFileName.Keys.ToList());

                foreach (var hashExists in result)
                {
                    var fileName = hashToFileName[hashExists.Key];
                    var exists = hashExists.Value;

                    Console.Write($"File: {fileName}: ");
                    if (exists)
                        Console.WriteLine("uploaded");
                    else
                        Console.WriteLine("not uploaded");
                }
            }
        }
    }
}