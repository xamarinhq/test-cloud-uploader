namespace Microsoft.AppHub.TestCloud.Commands
{
    public enum UploadCommandExitCodes
    {
        Success = 0,
        UnknownError = 1,
        AppFileNotFound = 2,
        WorkspaceNotFound = 3,
        DSymDirectoryNotFound = 4,
        InvalidOptions = 5
    }
}