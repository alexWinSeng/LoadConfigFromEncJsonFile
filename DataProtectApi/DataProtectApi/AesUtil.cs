using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace DataProtectApi
{
    public class AesUtil
    {
        /// <summary>
        /// generate a fix-length cryptographic key from a password
        /// </summary>
        /// <returns>base64 key</returns>
        public static string GenerateKey(string password)
        {
            var saltLength = 8;
            var salt = password.PadLeft(saltLength, ' ');
            var saltBytes = Encoding.UTF8.GetBytes(salt);
            var keyBase64 = "";
            using (var derivedBytes = new Rfc2898DeriveBytes(password, saltBytes, iterations: 50000, HashAlgorithmName.SHA256))
            {
                var keyLength = 256;
                byte[] key = derivedBytes.GetBytes(keyLength / 8); // 256 bits key
                keyBase64 = Convert.ToBase64String(key);
            }
            return keyBase64;
        }

        /// <summary>
        /// encrypt plain text using key
        /// </summary>
        /// <param name="keyBase64">base64 key</param>
        /// <param name="plainText">plain text</param>
        /// <returns></returns>
        public static string EncryptAsBase64(string keyBase64, string plainText)
        {
            var (key, iv) = GetKeyIv(keyBase64);
            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;

            byte[] dataByteArray = Encoding.UTF8.GetBytes(plainText);
            using MemoryStream ms = new MemoryStream();
            using CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(dataByteArray, 0, dataByteArray.Length);
            cs.FlushFinalBlock();
            return Convert.ToBase64String(ms.ToArray());
        }

        public static string EncryptAsBase64(string keyBase64, byte[] dataBytes)
        {
            var (key, iv) = GetKeyIv(keyBase64);
            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;

            using MemoryStream ms = new MemoryStream();
            using CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(dataBytes, 0, dataBytes.Length);
            cs.FlushFinalBlock();
            return Convert.ToBase64String(ms.ToArray());
        }

        /// <summary>
        /// decrypt base64 encrypted text using key
        /// </summary>
        /// <param name="keyBase64">base64 key</param>
        /// <param name="encryptedBase64">base64 encrypted text</param>
        /// <returns></returns>
        public static string Decrypt(string keyBase64, string encryptedBase64)
        {
            var decryptedBytes = DecryptAsBytes(keyBase64, encryptedBase64);
            return Encoding.UTF8.GetString(decryptedBytes);
        }

        /// <summary>
        /// decrypt base64 encrypted text using key
        /// </summary>
        /// <param name="keyBase64">base64 key</param>
        /// <param name="encryptedBase64">base64 encrypted text</param>
        /// <returns></returns>
        public static byte[] DecryptAsBytes(string keyBase64, string encryptedBase64)
        {
            var (key, iv) = GetKeyIv(keyBase64);
            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;

            byte[] encryptedBytes = Convert.FromBase64String(encryptedBase64);
            using MemoryStream ms = new MemoryStream();
            using CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(encryptedBytes, 0, encryptedBytes.Length);
            cs.FlushFinalBlock();
            return ms.ToArray();
        }

        /// <summary>
        /// get key and iv from base64 key
        /// </summary>
        /// <param name="keyBase64">base64 key</param>
        /// <returns></returns>
        private static (byte[] key, byte[] iv) GetKeyIv(string keyBase64)
        {
            var keyBytes = Convert.FromBase64String(keyBase64);
            var key = keyBytes.Take(32).ToArray();
            var iv = keyBytes.Take(16).ToArray();
            return (key, iv);
        }

    }
}
