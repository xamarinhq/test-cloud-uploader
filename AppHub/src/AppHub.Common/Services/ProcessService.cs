using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.AppHub.Common
{
    /// <summary>
    /// Default implementation of IProcessService. 
    /// </summary>
    public class ProcessService: IProcessService
    {
        /// <summary>
        /// Runs the external command.
        /// </summary>
        /// <param name="command">Command to execute.</param>
        /// <param name="arguments">(Optional) Command arguments.</param>
        /// <param name="standardOutputCallback">(Optional) An action that will be called with each standard output line wrote by the executed command.</param>
        /// <param name="standardErrorCallback">(Optional) An action that will be called with each standard error line wrote by the executed command.</param>
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

            // We use new task to wait for the process to avoid blocking the current thread.
            return Task.Run(async () => 
            {
                var standardOutputAndError = await Task.WhenAll(
                    ReadOutput(process.StandardOutput, standardOutputCallback), 
                    ReadOutput(process.StandardError, standardErrorCallback));
                    
                process.WaitForExit();

                return new ProcessResult(process.ExitCode, standardOutputAndError[0], standardOutputAndError[1]);
            });
        }

        private async Task<string> ReadOutput(StreamReader streamReader, Action<string> callback)
        {
            var result = new StringBuilder();
            string line;
            while ((line = await streamReader.ReadLineAsync()) != null)
            {
                if (callback != null)
                    callback(line);
                
                result.AppendLine(line);
            }

            return result.ToString();
        }
    }
}