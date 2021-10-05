using System;
using System.Security.Cryptography;
using System.Text;

namespace DataProtectApi
{
    public class RSAUtil
    {
        public static string EncryptByPublicKey(string pubKeyBase64, string plainText)
        {
            byte[] pubKeybytes = Convert.FromBase64String(pubKeyBase64);

            var encryptedContentBase64 = "";
            using (var cryptoServiceProvider = RSA.Create())
            {
                // load PKCS#1 format public key
                cryptoServiceProvider.ImportRSAPublicKey(pubKeybytes, out _);
                byte[] bytesEncrypted = cryptoServiceProvider.Encrypt(Encoding.UTF8.GetBytes(plainText), RSAEncryptionPadding.OaepSHA1);
                encryptedContentBase64 = Convert.ToBase64String(bytesEncrypted);
            }
            return encryptedContentBase64;
        }

        public static string DecryptByPrivateKey(string privateKeyBase64, string encryptedContentBase64)
        {
            byte[] privateKeybytes = Convert.FromBase64String(privateKeyBase64);
            var decryptedContent = "";
            using (var cryptoServiceProvider = RSA.Create())
            {
                // load PKCS#1 format private key
                cryptoServiceProvider.ImportRSAPrivateKey(privateKeybytes, out _);
                var bytesDecrypted = cryptoServiceProvider.Decrypt(Convert.FromBase64String(encryptedContentBase64), RSAEncryptionPadding.OaepSHA1);
                decryptedContent = Encoding.UTF8.GetString(bytesDecrypted);
            }
            return decryptedContent;
        }

        public static (string pubKeyBase64, string privateKeyBase64) GenerateRSAKeyPair()
        {
            var keySize = 2048;
            using var cryptoServiceProvider = RSA.Create(keySize);
            var pubKeyBase64 = Convert.ToBase64String(cryptoServiceProvider.ExportRSAPublicKey());
            var privateKeyBase64 = Convert.ToBase64String(cryptoServiceProvider.ExportRSAPrivateKey());

            return (pubKeyBase64, privateKeyBase64);
        }
    }
}
