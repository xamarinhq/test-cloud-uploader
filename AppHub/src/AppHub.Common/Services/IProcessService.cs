namespace Microsoft.AppHub.Common
{
    public interface IProcessService
    {
        ProcessResult Run(string command, string arguments = null);
    }
}