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
        private static string _versionTag = "version";
        private static string _packagesTag = "packages";
        private static string _packageTag = "package";
        private static string _filesTag = "files";
        private static string _dependenciesTag = "dependencies";

        private readonly string _version = "1.0";
        private readonly string _configPath;

        //TODO: package metadata

        protected List<IPackageConfiguration> Packages { get; }
        protected Dictionary<string, ProductPackageInfo> ProductPackagesInfo { get; }

        private JObject _jsonObject;

        public bool IsProductConfigured { get; private set; }

        public ProductConfigurationProvider(string productPath, string productConfigurationFileName)
            : this(Path.Combine(productPath, productConfigurationFileName))
        {
        }

        public ProductConfigurationProvider(string configPath)
        {
            _configPath = configPath;
            Packages = new List<IPackageConfiguration>();
            ProductPackagesInfo = new Dictionary<string, ProductPackageInfo>();

            TryParseConfiguration();
        }

        private void TryParseConfiguration()
        {
            if (!File.Exists(_configPath))
                return;

            using (var stream = File.OpenText(_configPath))
            {
                var reader = new JsonTextReader(stream);
                _jsonObject = JObject.Load(reader);

                JToken token;
                if (_jsonObject.TryGetValue(_packagesTag, out token))
                {
                    foreach (var package in token.Children())
                    {
                        var packageProp = (JProperty) package;
                        var packageObject = (JObject) packageProp.Value;

                        var packageId = new PackageId(packageProp.Name);
                        var info = new ProductPackageInfo();

                        if (packageObject.TryGetValue(_packageTag, out token))
                        {
                            var packageConfigObject = (JObject) token;
                            Packages.Add(new PackageConfiguration(packageId, packageConfigObject));

                            if (packageObject.TryGetValue(_filesTag, out token) && token.HasValues)
                            {
                                var array = (JArray)token;
                                info.Files.AddRange(array.Select(x => x.ToString()));
                            }

                            if (packageObject.TryGetValue(_dependenciesTag, out token) && token.HasValues)
                            {
                                var array = (JArray)token;
                                info.Dependencies.AddRange(array.Select(x => x.ToString()));
                            }

                            ProductPackagesInfo.Add(packageId.ToString(), info);
                        }
                    }

                    IsProductConfigured = true;
                }
            }
        }

        public IEnumerable<IPackageConfiguration> GetPackageConfigurations(DependencyOrdering dependencyOrdering)
        {
            if (dependencyOrdering == DependencyOrdering.TopPackagesOnly)
            {
                var dependencies = new List<string>();
                foreach (var info in ProductPackagesInfo)
                {
                    dependencies.AddRange(info.Value.Dependencies);
                }

                return Packages.Where(x => !dependencies.Contains(x.Id.ToString()));
            }

            if (dependencyOrdering == DependencyOrdering.TopToBottom)
            {
                var packages = new List<IPackageConfiguration>(Packages);
                packages.Reverse();
                return packages;
            }

            return Packages;
        }

        public IPackageConfiguration GetPackageConfiguration(PackageId packageId)
        {
            return Packages.FirstOrDefault(x => x.Id.IsSamePackage(packageId));
        }

        public void SetPackageConfiguration(IPackageConfiguration packageConfiguration, IEnumerable<string> files,
            IEnumerable<string> dependencies)
        {
            RemovePackageConfiguration(packageConfiguration.Id);

            Packages.Add(packageConfiguration);

            var info = new ProductPackageInfo();
            info.Files.AddRange(files);
            info.Dependencies.AddRange(dependencies);
            ProductPackagesInfo.Add(packageConfiguration.Id.ToString(), info);
        }

        public void RemovePackageConfiguration(PackageId packageId)
        {
            var configuration = GetPackageConfiguration(packageId);
            if (configuration != null)
            {
                Packages.Remove(configuration);
            }

            ProductPackagesInfo.Remove(packageId.ToString());
        }

        public IEnumerable<string> GetInstalledFiles(PackageId packageId, bool deletableFilesOnly)
        {
            var id = packageId.ToString();
            var packageInfo = ProductPackagesInfo[id];

            if (!deletableFilesOnly)
                return packageInfo.Files;

            return packageInfo.Files.Where(
                    file => !ProductPackagesInfo.Any(x => x.Key != id && x.Value.Files.Contains(file))).ToList();
        }

        public IEnumerable<string> GetPackageDependencies(PackageId packageId)
        {
            var id = packageId.ToString();
            var packageInfo = ProductPackagesInfo[id];

            return packageInfo.Dependencies;
        }

        public void Save()
        {
            var packagesObject = new JObject();
            var jsonObject = new JObject
            {
                {_versionTag, _version},
                {_packagesTag, packagesObject}
            };

            foreach (var package in Packages)
            {
                var packageInfo = ProductPackagesInfo[package.Id.ToString()];
                var filesArray = new JArray(packageInfo.Files);
                var dependenciesArray = new JArray(packageInfo.Dependencies);
                
                var packageObject = new JObject
                {
                    {_packageTag, JObject.Parse(package.AsJson())},
                    {_filesTag, filesArray},
                    {_dependenciesTag, dependenciesArray}
                };

                packagesObject.Add(package.Id.ToString(), packageObject);
            }

            var str = jsonObject.ToString();
            File.WriteAllText(_configPath, str);
        }
    }
}