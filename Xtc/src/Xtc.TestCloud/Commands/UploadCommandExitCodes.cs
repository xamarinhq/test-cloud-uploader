namespace Microsoft.Xtc.TestCloud.Commands
{
    public enum UploadCommandExitCodes
    {
        Success = 0,
        UnknownError = 3,
        InvalidAppFile = 4,
        InvalidWorkspace = 5,
        InvalidDSymDirectory = 6,
        InvalidOptions = 7,
        InvalidTestCloudEndpoint = 8,
        ServerError = 9
    }
}