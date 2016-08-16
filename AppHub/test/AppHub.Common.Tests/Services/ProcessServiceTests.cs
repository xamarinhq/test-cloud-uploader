using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.AppHub.Common.Tests
{
    public class ProcessServiceTests
    {
        [Fact]
        public async Task ProcessServiceLaunchesCommandAndReturnsExitCode()
        {
            Assert.Equal(1, (await LaunchProcess("1")).ExitCode);
            Assert.Equal(2, (await LaunchProcess("2")).ExitCode);
        }

        [Fact]
        public async Task ProcessServiceRedirectsStandardOutput()
        {
            var standardOutput = new StringBuilder();
            var result = await LaunchProcess("1", standardOutputCallback: line => standardOutput.AppendLine(line));

            var expected = $"Output 1{Environment.NewLine}Output 2{Environment.NewLine}";
            Assert.Equal(expected, standardOutput.ToString());
            Assert.Equal(expected, result.StandardOutput);
        }

        [Fact]
        public async Task ProcessServiceRedirectsStandardError()
        {
            var standardError = new StringBuilder();
            var result = await LaunchProcess("1", standardErrorCallback: line => standardError.AppendLine(line));

            var expected = $"Error 1{Environment.NewLine}";
            Assert.Equal(expected, standardError.ToString());
            Assert.Equal(expected, result.StandardError);
        }

        private Task<ProcessResult> LaunchProcess(
            string arguments, Action<string> standardOutputCallback = null, Action<string> standardErrorCallback = null) 
        {
            var platformService = new PlatformService();
            var processService = new ProcessService();

            if (platformService.CurrentPlatform == OSPlatform.Windows)
            {
                return processService.RunAsync(
                    "powershell.exe", "-f Services/app-test-process.ps1 " + arguments, 
                    standardOutputCallback, 
                    standardErrorCallback);
            }
            else
            {
                return processService.RunAsync("bash", "Services/app-test-process.sh " + arguments, 
                    standardOutputCallback, 
                    standardErrorCallback);
            }            
        }
    }
}