using System;
using System.Threading.Tasks;

namespace Microsoft.AppHub.Common.Services
{
    /// <summary>
    /// Represents service that runs an external process. 
    /// </summary>
    public interface IProcessService
    {
        /// <summary>
        /// Runs the external command.
        /// </summary>
        /// <param name="command">Command to execute.</param>
        /// <param name="arguments">(Optional) Command arguments.</param>
        /// <param name="standardOutputCallback">(Optional) An action that will be called with each standard output line
        /// wrote by the executed command.</param>
        /// <param name="standardErrorCallback">(Optional) An action that will be called with each standard error line 
        /// wrote by the executed command.</param>
        /// <remarks>
        /// The returned ProcessResult contains merged standard output and standard error, but they are not available 
        /// until the launched process finishes execution. The optional callbacks allow the caller to be notifed about 
        /// new output while the process is running. As an alternative design, I considered using IObserver for 
        /// notifications. However, the Reactive Extensions are not yet available for .NET Core. Also,
        /// that design would be more complicated and require more dependencies. 
        /// </remarks>
        Task<ProcessResult> RunAsync(
            string command, 
            string arguments = null, 
            Action<string> standardOutputCallback = null,
            Action<string> standardErrorCallback = null);
    }
}