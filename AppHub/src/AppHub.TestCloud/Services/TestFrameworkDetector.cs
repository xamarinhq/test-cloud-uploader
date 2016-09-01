using System;

namespace Microsoft.AppHub.TestCloud
{
    /// <summary>
    /// The test framework detector.
    /// </summary>
    public class TestFrameworkDetector : ITestFrameworkDetector
    {
        private static readonly Lazy<TestFrameworkDetector> _instance =
            new Lazy<TestFrameworkDetector>(() => new TestFrameworkDetector());

        public static TestFrameworkDetector Instance => _instance.Value;

        public TestFramework DetectTestFramework(string workspacePath)
        {
            if (workspacePath == null)
                throw new ArgumentNullException(nameof(workspacePath));

            if (new AppiumWorkspace(workspacePath).IsValid())
                return TestFramework.Appium;
            else
                return TestFramework.Unknown;
        }
    }
}