using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Microsoft.AppHub.TestCloud.Utilities
{
    public static class HashHelper
    {
        public static string GetFileHash(this HashAlgorithm hashAlgorithm, string filePath)
        {
            if (hashAlgorithm == null)
                throw new ArgumentNullException(nameof(hashAlgorithm));
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("Argument is null or empty", nameof(filePath));

            var fileInfo = new FileInfo(filePath);
            using (var stream = fileInfo.OpenRead())
            {
                return GetHash(hashAlgorithm, stream);
            }
        }

        public static string GetStringHash(this HashAlgorithm hashAlgorithm, string text)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            return GetBufferHash(hashAlgorithm, Encoding.UTF8.GetBytes(text));
        }

        public static string GetBufferHash(this HashAlgorithm hashAlgorithm, byte[] buffer)
        {
            if (hashAlgorithm == null)
                throw new ArgumentNullException(nameof(hashAlgorithm));
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            return ByteHashToString(hashAlgorithm.ComputeHash(buffer));
        }

        public static string GetHash(this HashAlgorithm hashAlgorithm, Stream stream)
        {
            if (hashAlgorithm == null)
                throw new ArgumentNullException(nameof(hashAlgorithm));
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            return ByteHashToString(hashAlgorithm.ComputeHash(stream));
        }

        private static string ByteHashToString(byte[] byteHash)
        {
            return BitConverter.ToString(byteHash).Replace("-", string.Empty).ToLowerInvariant();
        }
    }
}