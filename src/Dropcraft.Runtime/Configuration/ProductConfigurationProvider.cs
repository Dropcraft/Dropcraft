using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dropcraft.Common;
using Dropcraft.Common.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Dropcraft.Runtime.Configuration
{
    public class ProductConfigurationProvider : IProductConfigurationProvider
    {
        private readonly string _configPath;
        private readonly List<IPackageConfiguration> _packages = new List<IPackageConfiguration>();

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

        public IEnumerable<IPackageConfiguration> GetPackageConfigurations()
        {
            return _packages;
        }

        public IPackageConfiguration GetPackageConfiguration(PackageId packageId)
        {
            return _packages.FirstOrDefault(x => x.Id.IsSamePackage(packageId));
        }

        public void SetPackageConfiguration(IPackageConfiguration packageConfiguration)
        {
            RemovePackageConfiguration(packageConfiguration.Id);
            _packages.Add(packageConfiguration);
        }

        public void RemovePackageConfiguration(PackageId packageId)
        {
            var configuration = GetPackageConfiguration(packageId);
            if (configuration != null)
            {
                _packages.Remove(configuration);
            }
        }

        public void Save()
        {
            throw new System.NotImplementedException();
        }
    }
}