using System;
using System.Net.Http;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using Microsoft.Xtc.Common.Cli.Commands;
using Microsoft.Xtc.Common.Services;
using Microsoft.Xtc.Common.Services.Logging;

namespace Microsoft.Xtc.TestCloud.Services
{
    /// <summary>
    /// Service that verifies whether the system has all required .NET Core dependencies.
    /// </summary>
    public class NativeDependenciesVerifier : INativeDependenciesVerifier
    {
        private readonly ILogger _logger;
        private readonly OSPlatform _currentPlatform;

        private const string OsXNoOpenSslErrorMessage = @"In order to use this command, you first need to install the latest version of OpenSSL.
The easiest way to get this is by using one of package managers.
If your are using Homebrew (http://brew.sh), execute this command:
    brew install openssl

If you are using MacPorts (https://www.macports.org/), execute this command:
    sudo port install openssl
";

        public NativeDependenciesVerifier(ILoggerService loggerService, IPlatformService platformService)
        {
            if (loggerService == null)
                throw new ArgumentNullException(nameof(loggerService));
            if (platformService == null)
                throw new ArgumentNullException(nameof(platformService));
            
            _logger = loggerService.CreateLogger<NativeDependenciesVerifier>();
            _currentPlatform = platformService.CurrentPlatform;
        }

        public void Verify(string commandName)
        {
            if (commandName == null)
                throw new ArgumentNullException(nameof(commandName));

            if (_currentPlatform == OSPlatform.OSX)
            {
                VerifyOsXDependencies(commandName);
            }
        }

        private void VerifyOsXDependencies(string commandName)
        {
            try
            {
                _logger.LogDebug($"Verifying whether OpenSSL is installed");
                var typeName = (new HttpClient()).GetType().Name;
                _logger.LogDebug($"OpenSSL is installed and type {typeName} can be instantiated");
            }
            catch (TypeInitializationException)
            {
                _logger.LogDebug($"OpenSSL is not installed");
                throw new CommandException(commandName, OsXNoOpenSslErrorMessage);
            }
        }
    }
}