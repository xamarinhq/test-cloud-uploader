using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.AppHub.Common
{
    /// <summary>
    /// Default implementation of IProcessService. 
    /// </summary>
    public class ProcessService: IProcessService
    {
        public Task<ProcessResult> RunAsync(string command, string arguments = null, Action<string> standardOutputCallback = null, Action<string> standardErrorCallback = null)
        {
            if (string.IsNullOrWhiteSpace(command))
                throw new ArgumentNullException(nameof(command));

            var startInfo = new ProcessStartInfo
            {
                FileName = command,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                Arguments = arguments ?? string.Empty
            };

            var process = new Process() { StartInfo = startInfo };
            process.Start();

            return Task.Run(async () => 
            {
                var standardOutputAndError = await Task.WhenAll(ReadStandardOutput(process, standardOutputCallback), ReadStandardError(process, standardErrorCallback));
                process.WaitForExit();

                return new ProcessResult(process.ExitCode, standardOutputAndError[0], standardOutputAndError[1]);
            });
        }

        private async Task<string> ReadStandardOutput(Process process, Action<string> standardOutputCallback)
        {
            var result = new StringBuilder();
            string line;
            while ((line = await process.StandardOutput.ReadLineAsync()) != null)
            {
                if (standardOutputCallback != null)
                    standardOutputCallback(line);
                
                result.AppendLine(line);
            }

            return result.ToString();
        }

        private async Task<string> ReadStandardError(Process process, Action<string> standardErrorCallback)
        {
            var result = new StringBuilder();
            string line;
            while ((line = await process.StandardError.ReadLineAsync()) != null)
            {
                if (standardErrorCallback != null)
                    standardErrorCallback(line);
                
                result.AppendLine(line);
            }

            return result.ToString();
        }
    }
}