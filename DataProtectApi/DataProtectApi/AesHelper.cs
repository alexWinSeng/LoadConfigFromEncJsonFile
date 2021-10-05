using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DataProtectApi
{
    public class AesHelper
    {
        private readonly AesKey _aesKey;
        public AesHelper(string base64SecretKey)
        {
            var keyBytes = Convert.FromBase64String(base64SecretKey);

            _aesKey = new AesKey
            {
                Key = keyBytes.Take(32).ToArray(),
                IV = keyBytes.Skip(32).Take(16).ToArray()
            };
        }

        public string Decrypt(string encryptedText)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = _aesKey.Key;
                aes.IV = _aesKey.IV;

                byte[] dataByteArray = Convert.FromBase64String(encryptedText);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(dataByteArray, 0, dataByteArray.Length);
                        cs.FlushFinalBlock();
                        return Encoding.UTF8.GetString(ms.ToArray());
                    }
                }
            }
        }

        public byte[] DecryptAsBytes(string encryptedText)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = _aesKey.Key;
                aes.IV = _aesKey.IV;

                byte[] dataByteArray = Convert.FromBase64String(encryptedText);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(dataByteArray, 0, dataByteArray.Length);
                        cs.FlushFinalBlock();
                        return ms.ToArray();
                    }
                }
            }
        }

        public string EncryptAsBase64(string plainText)
        {
            using (var aes = Aes.Create())
            {
                Debug.Assert(aes != null, "aes != null");

                aes.Key = _aesKey.Key;
                aes.IV = _aesKey.IV;

                byte[] dataByteArray = Encoding.UTF8.GetBytes(plainText);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(dataByteArray, 0, dataByteArray.Length);
                        cs.FlushFinalBlock();
                        return Convert.ToBase64String(ms.ToArray());
                    }
                }
            }
        }

        public string EncryptAsBase64(byte[] dataByteArray)
        {
            using (var aes = Aes.Create())
            {
                Debug.Assert(aes != null, "aes != null");

                aes.Key = _aesKey.Key;
                aes.IV = _aesKey.IV;

                //byte[] dataByteArray = Encoding.UTF8.GetBytes(plainText);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(dataByteArray, 0, dataByteArray.Length);
                        cs.FlushFinalBlock();
                        return Convert.ToBase64String(ms.ToArray());
                    }
                }
            }
        }




        public static string GenerateAesKey()
        {
            using (Aes myAes = Aes.Create())
            {
                byte[] key = myAes.Key;
                byte[] iv = myAes.IV;

                byte[] passowrd = new byte[key.Length + iv.Length];
                key.CopyTo(passowrd, 0);
                iv.CopyTo(passowrd, key.Length);

                string base64Password = Convert.ToBase64String(passowrd);
                return base64Password;
            }
        }
    }
}
