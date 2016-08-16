using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace Microsoft.AppHub.TestCloud
{
    public static class HashHelper
    {
        public static string GetHash(this HashAlgorithm hashAlgorithm, DirectoryInfo directoryInfo)
        {
            if (directoryInfo == null)
                throw new ArgumentNullException(nameof(directoryInfo));
            
            return GetHash(hashAlgorithm, directoryInfo.GetFiles("*", SearchOption.AllDirectories));
        }

        public static string GetHash(this HashAlgorithm hashAlgorithm, FileInfo file)
        {
            if (hashAlgorithm == null)
                throw new ArgumentNullException(nameof(hashAlgorithm));
            if (file == null)
                throw new ArgumentNullException(nameof(file));

            using (var stream = file.OpenRead())
            {
                return GetHash(hashAlgorithm, stream);
            }
        }

        public static string GetHash(this HashAlgorithm hashAlgorithm, IEnumerable<FileInfo> files)
        {
            if (hashAlgorithm == null)
                throw new ArgumentNullException(nameof(hashAlgorithm));
            if (files == null || !files.Any())
                throw new ArgumentException($"Argument {nameof(files)} cannot be null or empty collection", nameof(files));

            var byteHash = new List<byte>();

            foreach (var file in files)
            {
                using (var stream = file.OpenRead())
                {
                    byteHash.AddRange(hashAlgorithm.ComputeHash(stream));
                }
            }

            return ByteHashToString(hashAlgorithm.ComputeHash(byteHash.ToArray()));
        }

        public static string GetHash(this HashAlgorithm hashAlgorithm, string text)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            return GetHash(hashAlgorithm, Encoding.UTF8.GetBytes(text));
        }

        public static string GetHash(this HashAlgorithm hashAlgorithm, byte[] buffer)
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

        public static string GetHash(this HashAlgorithm hashAlgorithm, IEnumerable<string> items)
        {
            if (hashAlgorithm == null)
                throw new ArgumentNullException(nameof(hashAlgorithm));
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            var byteHash = items
                .Select(text => hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(text)))
                .SelectMany(bytes => bytes)
                .ToList();

            return ByteHashToString(hashAlgorithm.ComputeHash(byteHash.ToArray()));
        }

        private static string ByteHashToString(byte[] byteHash)
        {
            return BitConverter.ToString(byteHash).Replace("-", string.Empty);
        }
    } 
}