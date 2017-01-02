using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dropcraft.Common;
using Dropcraft.Common.Package;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Dropcraft.Runtime.Core
{
    /// <summary>
    /// Class ProductConfigurationProvider.
    /// </summary>
    /// <seealso cref="Dropcraft.Common.IProductConfigurationProvider" />
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

        /// <summary>
        /// Gets the installed packages.
        /// </summary>
        /// <value>The installed packages.</value>
        protected IPackageGraph Packages { get; private set; }
        /// <summary>
        /// Gets the installed packages' information.
        /// </summary>
        /// <value>The installed packages' information.</value>
        protected List<ProductPackageInfo> ProductPackages { get; }

        private JObject _jsonObject;

        /// <summary>
        /// Indicates existence of the configured product in the product path
        /// </summary>
        /// <value><c>true</c> if the product is configured; otherwise, <c>false</c>.</value>
        public bool IsProductConfigured { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductConfigurationProvider"/> class.
        /// </summary>
        /// <param name="productPath">The product path.</param>
        /// <param name="productConfigurationFileName">Name of the product configuration file.</param>
        public ProductConfigurationProvider(string productPath, string productConfigurationFileName)
            : this(Path.Combine(productPath, productConfigurationFileName))
        {
            _productPath = productPath;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductConfigurationProvider"/> class.
        /// </summary>
        /// <param name="configPath">The configuration path.</param>
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
                                info.Files.AddRange(array.Select(x => new PackageFileInfo(x.ToString())));
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

        /// <summary>
        /// Returns all configured packages
        /// </summary>
        /// <returns><see cref="IPackageGraph"/></returns>
        public IPackageGraph GetPackages()
        {
            return Packages;
        }

        /// <summary>
        /// Returns configuration for the selected package
        /// </summary>
        /// <param name="packageId">Selected Package ID</param>
        /// <returns>Configuration for the selected package. <see cref="T:Dropcraft.Common.Package.IPackageConfiguration" /></returns>
        public IPackageConfiguration GetPackageConfiguration(PackageId packageId)
        {
            return ProductPackages.FirstOrDefault(x => x.Configuration.Id.IsSamePackage(packageId))?.Configuration;
        }

        /// <summary>
        /// Reconfigures product configuration by replacing it with the provided configuration
        /// </summary>
        /// <param name="packages">New list of the packages</param>
        /// <param name="packageGraph">Package dependencies</param>
        /// <param name="files">Package files</param>
        public void Reconfigure(IEnumerable<IPackageConfiguration> packages, IPackageGraph packageGraph,
            IDictionary<PackageId, IEnumerable<IPackageFile>> files)
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

        /// <summary>
        /// Removes package from a list of the configured packages
        /// </summary>
        /// <param name="packageId">Package to remove</param>
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

        /// <summary>
        /// Returns package's installed files
        /// </summary>
        /// <param name="packageId">Package ID</param>
        /// <param name="nonSharedFilesOnly">When true, instructs function to return only the files which are unique for the package</param>
        /// <returns>Files list</returns>
        public IReadOnlyCollection<IPackageFile> GetInstalledFiles(PackageId packageId, bool nonSharedFilesOnly)
        {
            var packageInfo = ProductPackages.FirstOrDefault(x => x.Configuration.Id.IsSamePackage(packageId));
            if (packageInfo == null)
                return new List<PackageFileInfo>(new PackageFileInfo[] {});

            var files = !nonSharedFilesOnly
                ? packageInfo.Files
                : packageInfo.Files.Where(file => !ProductPackages.Any(x => x != packageInfo && x.Files.Any(file.IsSameFile)))
                    .ToList();

            if (string.IsNullOrWhiteSpace(_productPath))
                return files;

            return files.Select(
                    x => Path.IsPathRooted(x.FileName) ? x : new PackageFileInfo(Path.Combine(_productPath, x.FileName)))
                .ToList();
        }

        /// <summary>
        /// Saves configuration to file
        /// </summary>
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
                var filesArray = new JArray(packageInfo.Files.Select(x=>x.FileName));
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