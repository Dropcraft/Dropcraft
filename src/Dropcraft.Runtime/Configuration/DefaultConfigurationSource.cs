using System.IO;
using System.Linq;
using Dropcraft.Common;
using Dropcraft.Common.Configuration;
using Dropcraft.Common.Package;

namespace Dropcraft.Runtime.Configuration
{
    public class DefaultConfigurationSource : IConfigurationSource
    {
        /// <summary>
        /// ManifestNameTemplate defines package manifest file name.
        /// Manifest can be changed from the default 'manifest.json' by providing a template.
        /// Example template: '{0}-manifest.json', where {0} will be replaced with the package ID
        /// </summary>
        public string ManifestNameTemplate { get; set; } = "manifest.json";

        /// <summary>
        /// Defines application configuration file name to store information about installed packages, dependencies, etc.
        /// </summary>
        public string ApplicationConfigurationFileName { get; set; } = "dropcraft.json";

        public IInstallablePackageConfiguration GetPackageConfiguration(InstallablePackageInfo packageInfo,
            RuntimeContext runtimeContext)
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

        public IInstaledPackageConfiguration GetPackageConfiguration(PackageInfo packageInfo, DeploymentContext deploymentContext)
        {
            throw new System.NotImplementedException();
        }

        public IApplicationConfiguration GetApplicationConfiguration(RuntimeContext runtimeContext)
        {
            throw new System.NotImplementedException();
        }
    }
}
