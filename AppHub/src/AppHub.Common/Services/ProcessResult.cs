namespace Microsoft.AppHub.Common
{
    public class ProcessResult
    {
        public ProcessResult(int exitCode, string standardOutput, string standardError)
        {
            this.ExitCode = exitCode;
            this.StandardOutput = standardOutput ?? string.Empty;
            this.StandardError = standardError ?? string.Empty;
        }

        public int ExitCode { get; }

        public string StandardOutput { get; }

        public string StandardError { get; }
    }
}