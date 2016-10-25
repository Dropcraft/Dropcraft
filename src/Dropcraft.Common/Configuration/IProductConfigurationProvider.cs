using System.Collections.Generic;

namespace Dropcraft.Common.Configuration
{
    /// <summary>
    /// Provides configurations for the product
    /// </summary>
    public interface IProductConfigurationProvider
    {
        /// <summary>
        /// Indicates existance of the configured product in the product path
        /// </summary>
        bool IsProductConfigured { get; }

        IEnumerable<PackageId> GetPackages();

        IPackageConfiguration GetPackageConfiguration(PackageId packageId);

        void SetPackageConfiguration(PackageId packageId, IPackageConfiguration packageConfiguration);

        void RemovePackageConfiguration(PackageId packageId);

        void Save();
    }
}