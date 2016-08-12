using System.Threading.Tasks;

namespace Microsoft.AppHub.Cli
{
    /// <summary> 
    /// Interface for CLI command executors.
    /// </summary>
    public interface ICommandExecutor
    {
        /// <summary>
        /// Executes the command.
        /// </summary>
        Task ExecuteAsync();        
    }
}