namespace Microsoft.AppHub.Common
{
    /// <summary>
    /// External process execution results.
    /// </summary>
    public class ProcessResult
    {
        public ProcessResult(int exitCode, string standardOutput, string standardError)
        {
            this.ExitCode = exitCode;
            this.StandardOutput = standardOutput ?? string.Empty;
            this.StandardError = standardError ?? string.Empty;
        }

        /// <summary>
        /// Process exit code.
        /// </summary>
        public int ExitCode { get; }

        /// <summary>
        /// Process standard output.
        /// </summary>
        public string StandardOutput { get; }

        /// <summary>
        /// Process standard error.
        /// </summary>
        public string StandardError { get; }
    }
}