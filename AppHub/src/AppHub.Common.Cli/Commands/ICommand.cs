using System.Threading.Tasks;

namespace Microsoft.AppHub.Cli
{
    /// <summary> 
    /// Interface for CLI command.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Executes the command.
        /// </summary>
        Task ExecuteAsync();        
    }
}