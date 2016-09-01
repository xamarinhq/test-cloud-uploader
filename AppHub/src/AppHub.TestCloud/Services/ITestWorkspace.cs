using System.Collections.Generic;
using System.Security.Cryptography;

namespace Microsoft.AppHub.TestCloud
{
    /// <summary>
    /// Interface for classes that represent workspace folder for supported test frameworks.
    /// </summary>
    public interface ITestWorkspace
    {
        string WorkspacePath { get; }

        bool IsValid();

        void Validate();

        IList<UploadFileInfo> GetUploadFiles(HashAlgorithm hashAlgorithm);
    }
}