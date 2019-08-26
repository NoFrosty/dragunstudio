using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Evopeons.API
{
    public class Helper
    {
        public static string GeneratePassword(int length) //length of salt    
        {
            const string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789!?#@[]";
            var randNum = new Random();
            var chars = new char[length];
            var allowedCharCount = allowedChars.Length;
            for (var i = 0; i <= length - 1; i++)
            {
                chars[i] = allowedChars[Convert.ToInt32((allowedChars.Length) * randNum.NextDouble())];
            }
            return new string(chars);
        }

        public static string GenerateSalt(int length)
        {
            Random random = new Random();
            byte[] saltBytes = new byte[length];

            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetNonZeroBytes(saltBytes);

            return ConvertToString(saltBytes);
        }
        public static string EncodePassword(string pass, string salt) //encrypt password    
        {
            string saltedPass = String.Concat(pass, salt);
            SHA256 sha256 = SHA256Managed.Create();
            byte[] hashedPass = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPass));

            return ConvertToString(hashedPass);
        }

        public static string ConvertToString(byte[] bytes)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }
}
