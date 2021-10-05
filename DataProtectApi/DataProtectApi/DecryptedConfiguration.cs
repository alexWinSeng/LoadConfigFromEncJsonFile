using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DataProtectApi
{
    public class DecryptedConfigurationProvider : JsonConfigurationProvider
    {
        public DecryptedConfigurationProvider(DecryptedConfigurationSource source) : base(source)
        {
                
        }

        public override void Load(Stream stream)
        {
            base.Load(stream);


            //var aesKeyBase64 = Environment.GetEnvironmentVariable("Keys__AesKey", EnvironmentVariableTarget.User);
            //var aesHelper = new AesHelper(aesKeyBase64);

            //var encryptedFileContentBase64 = Data["Content"];
            //var decryptedFileContent = aesHelper.Decrypt(encryptedFileContentBase64);
            //var decryptedFileContentBytes = aesHelper.DecryptAsBytes(encryptedFileContentBase64);

            //using var memoryStream = new MemoryStream(decryptedFileContentBytes);
            //base.Load(memoryStream);



            var keyBase64 = AesUtil.GenerateKey(Environment.MachineName);

            var encryptedFileContentBase64 = Data["Content"];
            var decryptedFileContent = AesUtil.Decrypt(keyBase64, encryptedFileContentBase64);
            var decryptedFileContentBytes = AesUtil.DecryptAsBytes(keyBase64, encryptedFileContentBase64);

            using var memoryStream = new MemoryStream(decryptedFileContentBytes);
            base.Load(memoryStream);
        }

        //public override void Load(Stream stream)
        //{
        //    base.Load(stream);





        //    // Symmetric 
        //    //var aesKeyBase64 = Environment.GetEnvironmentVariable("Keys__AesKey");

        //    //var encryptedStr = Data["ConnectionStrings:DefaultConnection"];

        //    //var aesHelper = new AesHelper(aesKeyBase64);
        //    //var decryptedStr = aesHelper.Decrypt(encryptedStr);
        //    //Data["ConnectionStrings:DefaultConnection"] = decryptedStr;

        //    // ASymmetric
        //    //var privateKeyBase64 = Environment.GetEnvironmentVariable("Keys__PrivateKey", EnvironmentVariableTarget.User);
        //    //var decryptedStr = RSAUtil.DecryptByPrivateKey(privateKeyBase64, encryptedStr);
        //    //Data["ConnectionStrings:DefaultConnection"] = decryptedStr;
        //}
    }

    public class DecryptedConfigurationSource : JsonConfigurationSource
    {
        public override IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            EnsureDefaults(builder);
            return new DecryptedConfigurationProvider(this);
        }
    }

    public static class JsonConfigurationExtensions2
    {
        public static IConfigurationBuilder AddJsonFile2(this IConfigurationBuilder builder, string path, bool optional,
            bool reloadOnChange)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("File path must be a non-empty string.");
            }

            var source = new DecryptedConfigurationSource
            {
                FileProvider = null,
                Path = path,
                Optional = optional,
                ReloadOnChange = reloadOnChange
            };

            source.ResolveFileProvider();
            builder.Add(source);
            return builder;
        }
    }
}
