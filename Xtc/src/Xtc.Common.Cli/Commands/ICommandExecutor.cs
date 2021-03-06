using System.Threading.Tasks;

namespace Microsoft.Xtc.Common.Cli.Commands
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