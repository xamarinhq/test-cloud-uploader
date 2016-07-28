using System;
using System.Diagnostics;

namespace Microsoft.AppHub.Common
{
    /// <summary>
    /// Default implementation of IProcessService. 
    /// </summary>
    public class ProcessService: IProcessService
    {
        public ProcessResult Run(string command, string arguments = null)
        {
            if (string.IsNullOrWhiteSpace(command))
                throw new ArgumentNullException(nameof(command));

            var startInfo = new ProcessStartInfo
            {
                FileName = command,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                CreateNoWindow = true,
                Arguments = arguments ?? string.Empty
            };

            var process = new Process() { StartInfo = startInfo };
            process.Start();
            process.WaitForExit();

            var standardOutput = process.StandardOutput.ReadToEnd();
            var standardError = process.StandardOutput.ReadToEnd();
            
            return new ProcessResult(process.ExitCode, standardOutput, standardError);
        }
    }
}