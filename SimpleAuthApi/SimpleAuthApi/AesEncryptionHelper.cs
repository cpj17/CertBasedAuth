using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SimpleAuthApi
{
    public static class AesEncryptionHelper
    {
        private static readonly string Key = "1234567890123456"; // 16 chars
        private static readonly string IV = "6543210987654321"; // 16 chars

        public static string Encrypt(string plainText)
        {
            using Aes aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(Key);
            aes.IV = Encoding.UTF8.GetBytes(IV);

            using var encryptor = aes.CreateEncryptor();
            using var ms = new MemoryStream();
            using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
            using var sw = new StreamWriter(cs);

            sw.Write(plainText);
            sw.Close();

            return Convert.ToBase64String(ms.ToArray());
        }
    }
}
