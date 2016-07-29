using System;
using System.Threading.Tasks;

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
        Task<ProcessResult> RunAsync(string command, string arguments = null, Action<string> standardOutputCallback = null, Action<string> standardErrorCallback = null);
    }
}