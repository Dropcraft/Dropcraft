using System.Collections.Generic;

namespace Dropcraft.Common.Configuration
{
    public enum DependencyOrdering
    {
        TopToBottom,
        BottomToTop,
        TopPackagesOnly
    }

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
        /// Returns configuration for all configured packages
        /// </summary>
        /// <returns>Package configurations list</returns>
        IEnumerable<IPackageConfiguration> GetPackageConfigurations(DependencyOrdering dependencyOrdering);

        /// <summary>
        /// Returns configuration for the selected package
        /// </summary>
        /// <param name="packageId">Selected Package ID</param>
        /// <returns>Configuration for the selected package</returns>
        IPackageConfiguration GetPackageConfiguration(PackageId packageId);

        void SetPackageConfiguration(IPackageConfiguration packageConfiguration, IEnumerable<string> files,
            IEnumerable<string> dependencies);

        void RemovePackageConfiguration(PackageId packageId);

        IEnumerable<string> GetInstalledFiles(PackageId packageId, bool deletableFilesOnly);

        IEnumerable<string> GetPackageDependencies(PackageId packageId);

        void Save();
    }
}