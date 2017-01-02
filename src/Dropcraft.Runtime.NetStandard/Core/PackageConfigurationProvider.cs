using System.IO;
using System.Linq;
using System.Xml.Linq;
using Dropcraft.Common;
using Dropcraft.Common.Package;
using Newtonsoft.Json.Linq;

namespace Dropcraft.Runtime.Core
{
    /// <summary>
    /// Provides JSON-based package configuration 
    /// </summary>
    public class PackageConfigurationProvider : IPackageConfigurationProvider
    {
        private readonly string _manifestNameTemplate;

        /// <summary>
        /// Constructs the provider
        /// </summary>
        /// <param name="manifestNameTemplate">Template is used to determine manifest name. In manifest-{0}.json {0} will be replaced with the package ID</param>
        public PackageConfigurationProvider(string manifestNameTemplate)
        {
            _manifestNameTemplate = manifestNameTemplate;
        }

        /// <summary>
        /// Returns parsed package configuration
        /// </summary>
        /// <param name="packageId">Package ID</param>
        /// <param name="packagePath">Path to the unpacked package</param>
        /// <returns></returns>
        public IPackageConfiguration GetPackageConfiguration(PackageId packageId, string packagePath)
        {
            JObject metadataInfo = null;
            var specFile = Directory.GetFiles(packagePath, "*.nuspec", SearchOption.TopDirectoryOnly).FirstOrDefault();
            if (specFile != null)
            {
                var xdoc = XDocument.Load(specFile);
                var element = xdoc.Descendants().First(x => x.Name.LocalName == "metadata");

                metadataInfo = new JObject
                {
                    {"title", element.Descendants().FirstOrDefault(x => x.Name.LocalName == "title")?.Value ?? string.Empty},
                    {"authors", element.Descendants().FirstOrDefault(x => x.Name.LocalName == "authors")?.Value ?? string.Empty},
                    {"description", element.Descendants().FirstOrDefault(x => x.Name.LocalName == "description")?.Value ?? string.Empty},
                    {"projectUrl", element.Descendants().FirstOrDefault(x => x.Name.LocalName == "projectUrl")?.Value ?? string.Empty},
                    {"iconUrl", element.Descendants().FirstOrDefault(x => x.Name.LocalName == "iconUrl")?.Value ?? string.Empty},
                    {"licenseUrl", element.Descendants().FirstOrDefault(x => x.Name.LocalName == "licenseUrl")?.Value ?? string.Empty},
                    {"copyright", element.Descendants().FirstOrDefault(x => x.Name.LocalName == "copyright")?.Value ?? string.Empty},
                };
            }

            var manifestName = string.Format(_manifestNameTemplate, packageId.Id);
            var manifestFile = Directory.GetFiles(packagePath, manifestName, SearchOption.AllDirectories).FirstOrDefault();
            if (manifestFile != null)
            {
                var text = File.ReadAllText(manifestFile);
                var parsedManifest = JObject.Parse(text);
                parsedManifest?.Add("metadata", metadataInfo);

                return new PackageConfiguration(packageId, parsedManifest);
            }

            return new PackageConfiguration(packageId, new JObject { { "metadata", metadataInfo } });
        }
    }
}