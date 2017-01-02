namespace Dropcraft.Common.Package
{
    /// <summary>
    /// IPackageConfigurationSource is a source of configuration for packages during deployment.
    /// </summary>
    public interface IPackageConfigurationSource
    {
        /// <summary>
        /// Creates package configuration provider
        /// </summary>
        /// <returns><see cref="IPackageConfigurationProvider"/></returns>
        IPackageConfigurationProvider GetPackageConfigurationProvider();
    }
}