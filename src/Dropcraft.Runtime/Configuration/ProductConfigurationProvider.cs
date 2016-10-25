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
        private readonly string _configPath;
        public bool IsProductConfigured { get; private set; }

        public ProductConfigurationProvider(IProductContext context, string productConfigurationFileName)
            : this(Path.Combine(context.ProductPath, productConfigurationFileName))
        {
        }

        public ProductConfigurationProvider(string configPath)
        {
            _configPath = configPath;
            TryParseConfiguration();
        }

        private void TryParseConfiguration()
        {
            if (!File.Exists(_configPath))
                return;

            using (var stream = File.OpenText(_configPath))
            {
                var reader = new JsonTextReader(stream);
                var jObject = JObject.Load(reader);

                //TODO
            }


            IsProductConfigured = true;
        }

        public IEnumerable<PackageId> GetPackages()
        {
            var packages = new List<PackageId>();
            if (!IsProductConfigured)
                return packages;
            
            return packages;
        }

        public IPackageConfiguration GetPackageConfiguration(PackageId packageId)
        {
            if (!IsProductConfigured)
                return null;

            throw new System.NotImplementedException();
        }

        public void SetPackageConfiguration(PackageId packageId, IPackageConfiguration packageConfiguration)
        {
        }

        public void RemovePackageConfiguration(PackageId packageId)
        {
            if (!IsProductConfigured)
                return;
        }

        public void Save()
        {
            throw new System.NotImplementedException();
        }
    }
}