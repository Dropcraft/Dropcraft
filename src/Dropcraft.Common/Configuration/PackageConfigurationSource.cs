namespace Dropcraft.Common.Configuration
{
    /// <summary>
    /// PackageConfigurationSource is a source of configuration for packages during deployment.
    /// </summary>
    public abstract class PackageConfigurationSource
    {
        protected ProductContext Context { get; private set; }

        protected PackageConfigurationSource(ProductContext context)
        {
            Context = context;
        }

        public PackageConfiguration GetPackageConfiguration(PackageInfo packageInfo)
        {
            return OnGetPackageConfiguration(packageInfo);
        }

        protected abstract PackageConfiguration OnGetPackageConfiguration(PackageInfo packageInfo);
    }
}