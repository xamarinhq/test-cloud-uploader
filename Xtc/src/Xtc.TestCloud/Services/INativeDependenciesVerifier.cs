namespace Microsoft.Xtc.TestCloud.Services
{
    /// <summary>
    /// Represents service that verifies whether the system has all required .NET Core dependencies.
    /// </summary>
    public interface INativeDependenciesVerifier
    {
        void Verify(string commandName);
    }
}