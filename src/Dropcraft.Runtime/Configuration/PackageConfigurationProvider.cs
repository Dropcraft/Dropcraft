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

        public IPackageConfiguration GetPackageConfiguration(PackageInfo packageInfo)
        {
            var manifestName = string.Format(_manifestNameTemplate, packageInfo.PackageId.Id);
            var manifestFile = packageInfo.Files.FirstOrDefault(x => x.EndsWith(manifestName));
            if (manifestFile != null)
            {
                var text = File.ReadAllText(manifestFile);
                return new PackageConfiguration(packageInfo, JObject.Parse(text));
            }

            return null;
        }
    }
}