namespace Dropcraft.Common.Configuration
{
    /// <summary>
    /// IPackageConfigurationSource is a source of configuration for packages during deployment.
    /// </summary>
    public interface IPackageConfigurationSource
    {
        IPackageConfigurationProvider GetPackageConfigurationProvider();
    }
}