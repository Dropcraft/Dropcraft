using System.Collections.Generic;
using Dropcraft.Common.Package;

namespace Dropcraft.Common
{
    /// <summary>
    /// Provides configurations for the product
    /// </summary>
    public interface IProductConfigurationProvider
    {
        /// <summary>
        /// Indicates existence of the configured product in the product path
        /// </summary>
        bool IsProductConfigured { get; }

        /// <summary>
        /// Returns all configured packages
        /// </summary>
        /// <returns>Package graph</returns>
        IPackageGraph GetPackages();

        /// <summary>
        /// Returns configuration for the selected package
        /// </summary>
        /// <param name="packageId">Selected Package ID</param>
        /// <returns>Configuration for the selected package</returns>
        IPackageConfiguration GetPackageConfiguration(PackageId packageId);

        /// <summary>
        /// Reconfigures product configuration by replacing it with the provided configuration
        /// </summary>
        /// <param name="packages">New list of the packages</param>
        /// <param name="packageGraph">Package dependencies</param>
        /// <param name="files">Package files</param>
        void Reconfigure(IEnumerable<IPackageConfiguration> packages, IPackageGraph packageGraph,
            IDictionary<PackageId, IEnumerable<string>> files);

        /// <summary>
        /// Removes package from a list of the configured packages
        /// </summary>
        /// <param name="packageId">Package to remove</param>
        void RemovePackage(PackageId packageId);

        IEnumerable<string> GetInstalledFiles(PackageId packageId, bool nonSharedFilesOnly);

        /// <summary>
        /// Saves configuration to file
        /// </summary>
        void Save();
    }
}