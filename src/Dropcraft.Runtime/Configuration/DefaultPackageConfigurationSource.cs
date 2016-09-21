using System.IO;
using System.Linq;
using Dropcraft.Common;
using Dropcraft.Common.Configuration;
using Dropcraft.Common.Package;

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


        public DefaultPackageConfigurationSource(DeploymentContext deploymentContext) 
            : base(deploymentContext)
        {
        }

        protected override PackageConfiguration OnGetPackageConfiguration(InstallablePackageInfo packageInfo)
        {
            var manifestName = string.Format(ManifestNameTemplate, packageInfo.Id);
            var manifestFile = packageInfo.InstallableFiles.FirstOrDefault(x => x.FilePath.EndsWith(manifestName));
            if (manifestFile != null)
            {
                var text = File.ReadAllText(manifestFile.FilePath);
                //                return JObject.Parse(text);

            }

            return null;
        }
    }
}
