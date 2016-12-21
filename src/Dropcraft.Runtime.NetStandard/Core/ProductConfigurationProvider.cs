using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dropcraft.Common;
using Dropcraft.Common.Package;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Dropcraft.Runtime.Core
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
        private readonly string _productPath;

        protected IPackageGraph Packages { get; private set; }
        protected List<ProductPackageInfo> ProductPackages { get; }

        private JObject _jsonObject;

        public bool IsProductConfigured { get; private set; }

        public ProductConfigurationProvider(string productPath, string productConfigurationFileName)
            : this(Path.Combine(productPath, productConfigurationFileName))
        {
            _productPath = productPath;
        }

        public ProductConfigurationProvider(string configPath)
        {
            _configPath = configPath;
            Packages = PackageGraph.Empty();
            ProductPackages = new List<ProductPackageInfo>();

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
                    var builder = new PackageGraphBuilder();
                    foreach (var package in token.Children())
                    {
                        var packageProp = (JProperty) package;
                        var packageObject = (JObject) packageProp.Value;

                        var packageId = new PackageId(packageProp.Name);
                        var info = new ProductPackageInfo();

                        if (packageObject.TryGetValue(_packageTag, out token))
                        {
                            var packageConfigObject = (JObject) token;
                            info.Configuration = new PackageConfiguration(packageId, packageConfigObject);

                            if (packageObject.TryGetValue(_filesTag, out token) && token.HasValues)
                            {
                                var array = (JArray) token;
                                info.Files.AddRange(array.Select(x => x.ToString()));
                            }
                            
                            var dependencies = new List<PackageId>();
                            if (packageObject.TryGetValue(_dependenciesTag, out token) && token.HasValues)
                            {
                                var array = (JArray) token;
                                dependencies.AddRange(array.Select(x => new PackageId(x.ToString())));
                            }

                            builder.Append(packageId, dependencies);
                            ProductPackages.Add(info);
                        }
                    }

                    Packages = builder.Build();
                    IsProductConfigured = true;
                }
            }
        }

        public IPackageGraph GetPackages()
        {
            return Packages;
        }

        public IPackageConfiguration GetPackageConfiguration(PackageId packageId)
        {
            return ProductPackages.FirstOrDefault(x => x.Configuration.Id.IsSamePackage(packageId))?.Configuration;
        }

        public void Reconfigure(IEnumerable<IPackageConfiguration> packages, IPackageGraph packageGraph,
            IDictionary<PackageId, IEnumerable<string>> files)
        {
            Packages = packageGraph;

            ProductPackages.Clear();
            foreach (var package in packages)
            {
                var info = new ProductPackageInfo
                {
                    Configuration = package
                };

                var packageFiles = files.FirstOrDefault(f => f.Key.IsSamePackage(package.Id)).Value;
                if (packageFiles != null)
                    info.Files.AddRange(packageFiles);

                ProductPackages.Add(info);
            }
        }

        public void RemovePackage(PackageId packageId)
        {
            var packageInfo = ProductPackages.FirstOrDefault(x=>x.Configuration.Id.IsSamePackage(packageId));
            if (packageInfo != null)
            {
                ProductPackages.Remove(packageInfo);
            }

            var allPackages = Packages.GetNodes(new PackageId[] {});
            var builder = new PackageGraphBuilder();

            foreach (var packageNode in allPackages)
            {
                if (packageNode.Package.IsSamePackage(packageId))
                    continue;

                builder.Append(packageNode.Package,
                    packageNode.Dependencies.Where(x => !x.Package.IsSamePackage(packageId)).Select(p => p.Package));
            }

            Packages = builder.Build();
        }

        public IEnumerable<string> GetInstalledFiles(PackageId packageId, bool nonSharedFilesOnly)
        {
            var packageInfo = ProductPackages.FirstOrDefault(x => x.Configuration.Id.IsSamePackage(packageId));
            if (packageInfo == null)
                return new string[] {};

            var files = !nonSharedFilesOnly
                ? packageInfo.Files
                : packageInfo.Files.Where(file => !ProductPackages.Any(x => x != packageInfo && x.Files.Contains(file)))
                    .ToList();

            return string.IsNullOrWhiteSpace(_productPath)
                ? files
                : files.Select(path => Path.IsPathRooted(path) ? path : Path.Combine(_productPath, path));
        }

        public void Save()
        {
            var packagesObject = new JObject();
            var jsonObject = new JObject
            {
                {_versionTag, _version},
                {_packagesTag, packagesObject}
            };

            var allPackages = Packages.GetNodes(new PackageId[] { });

            foreach (var packageInfo in ProductPackages)
            {
                var packageId = packageInfo.Configuration.Id;
                var filesArray = new JArray(packageInfo.Files);

                var dependencies =
                    allPackages.FirstOrDefault(x => x.Package.IsSamePackage(packageId))?
                        .Dependencies.Select(d => d.Package.ToString()) ?? new string[] {};
                var dependenciesArray = new JArray(dependencies);
                
                var packageObject = new JObject
                {
                    {_packageTag, JObject.Parse(packageInfo.Configuration.AsJson())},
                    {_filesTag, filesArray},
                    {_dependenciesTag, dependenciesArray}
                };

                packagesObject.Add(packageId.ToString(), packageObject);
            }

            var str = jsonObject.ToString();
            File.WriteAllText(_configPath, str);
        }
    }
}