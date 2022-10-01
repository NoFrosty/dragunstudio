using System.Security.Cryptography;
using System.Text;

namespace DragunStudio.Managers.Package.Helpers
{
    public static class CommonHelper
    {
        private static readonly SHA256 sha256Hash = SHA256.Create();

        internal static string CalculateHash(Stream stream)
        {
            return ByteArrayToString(sha256Hash.ComputeHash(stream));
        }

        internal static string ByteArrayToString(byte[] ba)
        {
            var hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        internal static string MakeRelative(string absolutePath, string filePath)
        {
            if (absolutePath.StartsWith(filePath))
            {
                return absolutePath.Substring(filePath.Length);
            }
            throw new InvalidDataException("Unexpected error file path!");
        }
    }
}
