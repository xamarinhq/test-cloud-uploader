using System.Threading.Tasks;

namespace Microsoft.AppHub.Common
{
    /// <summary>
    /// Extensions and constants for Task.
    /// </summary>
    public static class Tasks
    {
        /// <summary>
        /// Completed task.
        /// </summary>
        public static readonly Task Completed = Task.FromResult(true);
    }
}