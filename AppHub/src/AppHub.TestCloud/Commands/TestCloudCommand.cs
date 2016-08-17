using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using DocoptNet;
using Microsoft.AppHub.Cli;
using Microsoft.AppHub.Common;
using Microsoft.Extensions.Logging;

namespace Microsoft.AppHub.TestCloud
{
    public class TestCloudCommand: ICommand
    {
        public string Name => "test-cloud";

        public string Summary => "Temporary command for Test Cloud connections";

        public string Syntax => $@"Command '{this.Name}': {this.Summary}. 

Usage:
    {ProgramUtilities.CurrentExecutableName} {this.Name} <appFile> <files>...
"; 

        public ICommandExecutor CreateCommandExecutor(IDictionary<string, ValueObject> options, IServiceProvider serviceProvider)
        {
            var appFile = options["<appFile>"].ToString();
            var regularFiles = options["<files>"]
                .AsList
                .Cast<ValueObject>()
                .Select(vo => vo.ToString())
                .ToList();
            
            return new TestCloudCommandExecutor(appFile, regularFiles, (ILoggerService)serviceProvider.GetService(typeof(ILoggerService)));
        }
    }

    public class TestCloudCommandExecutor: ICommandExecutor
    {
        private static readonly Uri _testCloudUri = new Uri("https://testcloud.xamarin.com/ci");

        private readonly string _appFile;
        private readonly IList<string> _files; 
        private readonly TestCloudProxy _testCloudProxy;

        public TestCloudCommandExecutor(string appFile, IList<string> files, ILoggerService loggerService)
        {
            if (string.IsNullOrEmpty(appFile)) 
                throw new ArgumentNullException(nameof(appFile));
            if (files == null)
                throw new ArgumentNullException(nameof(files));
            if (loggerService == null)
                throw new ArgumentNullException(nameof(loggerService));

            _appFile = appFile;
            _files = files;
            _testCloudProxy = new TestCloudProxy(_testCloudUri, loggerService);
        }

        public async Task ExecuteAsync()
        {
            var appFileInfo = new FileInfo(_appFile);
            
            var allFileInfos = _files.Concat(new[] { _appFile }).Select(file => new FileInfo(file)).ToArray();

            using (var sha256 = SHA256.Create())
            {
                var appFileHash = sha256.GetHash(appFileInfo);
                var hashToFileName = allFileInfos.ToDictionary(
                    fi => sha256.GetHash(fi),
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