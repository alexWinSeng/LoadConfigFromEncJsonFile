using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataProtectApi
{
    public class CustomConfigProvider : ConfigurationProvider, IConfigurationSource
    {
        public CustomConfigProvider()
        {
        }

        public override void Load()
        {
            Data = UnencryptMyConfiguration();
        }

        private IDictionary<string, string> UnencryptMyConfiguration()
        {
            // do whatever you need to do here, for example load the file and unencrypt key by key
            //Like:
            var configValues = new Dictionary<string, string>
            {
                 {"key1", "unencryptedValue1"},
                 {"key2", "unencryptedValue2"}
            };
            return configValues;
        }

        private IDictionary<string, string> CreateAndSaveDefaultValues(IDictionary<string, string> defaultDictionary)
        {
            var configValues = new Dictionary<string, string>
            {
                {"key1", "encryptedValue1"},
                {"key2", "encryptedValue2"}
            };
            return configValues;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new CustomConfigProvider();
        }
    }
}
