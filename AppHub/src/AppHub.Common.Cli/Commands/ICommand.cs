using System.Threading.Tasks;

namespace Microsoft.AppHub.Cli
{
    /// <summary> 
    /// Interface for all command line commands.
    /// </summary>
    public interface ICommand
    {
        Task ExecuteAsync();        
    }
}