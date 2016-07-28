namespace Microsoft.AppHub.Common
{
    /// <summary>
    /// Represents service that runs a command. 
    /// </summary>
    public interface IProcessService
    {
        /// <summary>
        /// Runs a command. 
        /// </summary>
        ProcessResult Run(string command, string arguments = null);
    }
}