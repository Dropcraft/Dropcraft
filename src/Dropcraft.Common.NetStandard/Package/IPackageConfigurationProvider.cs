namespace Dropcraft.Common.Package
{
    /// <summary>
    /// Provides configurations for the packages
    /// </summary>
    public interface IPackageConfigurationProvider
    {
        /// <summary>
        /// Returns package configuration
        /// </summary>
        /// <param name="packageId">Package</param>
        /// <param name="packagePath">Package location</param>
        /// <returns><see cref="IPackageConfiguration"/></returns>
        IPackageConfiguration GetPackageConfiguration(PackageId packageId, string packagePath);
    }
}