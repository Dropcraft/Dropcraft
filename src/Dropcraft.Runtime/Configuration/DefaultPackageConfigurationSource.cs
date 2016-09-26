using System.IO;
using System.Linq;
using Dropcraft.Common;
using Dropcraft.Common.Configuration;
using Newtonsoft.Json.Linq;

namespace Dropcraft.Runtime.Configuration
{
    /// <summary>
    /// Configuration source for default JSON-based package configuration files
    /// </summary>
    public class DefaultPackageConfigurationSource : PackageConfigurationSource
    {
        /// <summary>
        /// ManifestNameTemplate defines package manifest file name.
        /// Manifest can be changed from the default 'manifest.json' by providing a template.
        /// Example template: '{0}-manifest.json', where {0} will be replaced with the package ID
        /// </summary>
        public string ManifestNameTemplate { get; set; } = "manifest.json";


        public DefaultPackageConfigurationSource(ProductContext context) 
            : base(context)
        {
        }

        protected override PackageConfiguration OnGetPackageConfiguration(PackageInfo packageInfo)
        {
            var manifestName = string.Format(ManifestNameTemplate, packageInfo.PackageId.Id);
            var manifestFile = packageInfo.Files.FirstOrDefault(x => x.EndsWith(manifestName));
            if (manifestFile != null)
            {
                var text = File.ReadAllText(manifestFile);
                return new DefaultPackageConfiguration(packageInfo, JObject.Parse(text));
            }

            return null;
        }
    }
}
