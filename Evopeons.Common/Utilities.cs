using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.IO.Compression;

namespace Evopeons.Common
{
    public static class Utilities
    {
        private static SHA256 sha256Hash = SHA256.Create();

        public static string CalculateHash(Stream stream)
        {
            return ByteArrayToString(sha256Hash.ComputeHash(stream));
        }

        public static string CalculateHash(byte[] content)
        {
            return ByteArrayToString(sha256Hash.ComputeHash(content));
        }

        public static string ByteArrayToString(byte[] ba)
        {
            var hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        public static void CompressGZip(Stream stream)
        {
            using (GZipStream gZipStream = new GZipStream(stream, CompressionLevel.Optimal))
            {
                
            }
        }
    }
}
