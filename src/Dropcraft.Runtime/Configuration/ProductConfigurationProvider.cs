using System.Collections.Generic;
using System.IO;
using Dropcraft.Common;
using Dropcraft.Common.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Dropcraft.Runtime.Configuration
{
    public class ProductConfigurationProvider : IProductConfigurationProvider
    {
        public bool IsProductConfigured { get; private set; }

        public ProductConfigurationProvider(IProductContext context, string productConfigurationFileName)
        {
            TryParseConfiguration(context, productConfigurationFileName);
        }

        private void TryParseConfiguration(IProductContext context, string productConfigurationFileName)
        {
            var config = Path.Combine(context.ProductPath, productConfigurationFileName);

            if (!File.Exists(config))
                return;

            using (var stream = File.OpenText(config))
            {
                var reader = new JsonTextReader(stream);
                var jObject = JObject.Load(reader);

                //TODO
            }


            IsProductConfigured = true;
        }

        public IEnumerable<PackageInfo> GetPackages()
        {
            throw new System.NotImplementedException();
        }

        public IPackageConfiguration GetPackageConfiguration(PackageId packageId)
        {
            throw new System.NotImplementedException();
        }

        public void SetPackageConfiguration(PackageId packageId, IPackageConfiguration packageConfiguration)
        {
            throw new System.NotImplementedException();
        }

        public void RemovePackageConfiguration(PackageId packageId)
        {
            throw new System.NotImplementedException();
        }
    }
}