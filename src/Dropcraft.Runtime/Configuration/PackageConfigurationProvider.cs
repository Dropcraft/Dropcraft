using System.IO;
using System.Linq;
using Dropcraft.Common;
using Dropcraft.Common.Configuration;
using Newtonsoft.Json.Linq;

namespace Dropcraft.Runtime.Configuration
{
    public class PackageConfigurationProvider : IPackageConfigurationProvider
    {
        private readonly string _manifestNameTemplate;

        public PackageConfigurationProvider(IProductContext context, string manifestNameTemplate)
        {
            _manifestNameTemplate = manifestNameTemplate;
        }

        public IPackageConfiguration GetPackageConfiguration(PackageId packageId, string packagePath)
        {
            var manifestName = string.Format(_manifestNameTemplate, packageId.Id);
            var manifestFile = Directory.GetFiles(packagePath, manifestName, SearchOption.AllDirectories).FirstOrDefault();

            if (manifestFile != null)
            {
                var text = File.ReadAllText(manifestFile);
                return new PackageConfiguration(packageId, JObject.Parse(text));
            }

            return null;
        }
    }
}