namespace Microsoft.AppHub.TestCloud
{
    /// <summary>
    /// Interface for test framework detector.
    /// </summary>
    public interface ITestFrameworkDetector
    {
        /// <summary>
        /// Detects framework used by tests from a given workspace.
        /// </summary>
        /// <param name="workspacePath">Path to the workspace.</param>
        /// <returns>
        /// Returns detected test framework or TestFramework.Unknown if the framework was not recognized.
        /// </returns>
        TestFramework DetectTestFramework(string workspacePath);
    }
}