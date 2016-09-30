using System.Collections.Generic;

namespace Dropcraft.Common.Configuration
{
    /// <summary>
    /// Provides configurations for the product
    /// </summary>
    public interface IProductConfigurationProvider
    {
        bool IsProductConfigured { get; }

        IEnumerable<PackageInfo> GetPackages();

        IPackageConfiguration GetPackageConfiguration(PackageId packageId);
        void SetPackageConfiguration(PackageId packageId, IPackageConfiguration packageConfiguration);
        void RemovePackageConfiguration(PackageId packageId);
    }
}