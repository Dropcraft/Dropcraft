using Dropcraft.Common.Package;

namespace Dropcraft.Runtime.Core
{
    /// <summary>
    /// Configuration source for default JSON-based package configuration files
    /// </summary>
    public class PackageConfigurationSource : IPackageConfigurationSource
    {
        /// <summary>
        /// ManifestNameTemplate defines package manifest file name.
        /// Manifest can be changed from the default 'manifest.json' by providing a template.
        /// Example template: '{0}-manifest.json', where {0} will be replaced with the package ID
        /// </summary>
        public string ManifestNameTemplate { get; set; } = "manifest.json";

        /// <summary>
        /// Creates package configuration provider
        /// </summary>
        /// <returns><see cref="T:Dropcraft.Common.Package.IPackageConfigurationProvider" /></returns>
        public IPackageConfigurationProvider GetPackageConfigurationProvider()
        {
            return new PackageConfigurationProvider(ManifestNameTemplate);
        }
    }
}
