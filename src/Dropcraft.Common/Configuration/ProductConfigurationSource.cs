using System.Collections.Generic;

namespace Dropcraft.Common.Configuration
{
    /// <summary>
    /// ProductConfigurationSource is a source of configuration for installed product and deployed packages
    /// </summary>
    public abstract class ProductConfigurationSource
    {
        protected RuntimeContext RuntimeContext { get; private set; }

        protected ProductConfigurationSource(RuntimeContext runtimeContext)
        {
            RuntimeContext = runtimeContext;
        }

        public bool IsProductConfigured => OnIsProductConfigured();

        public IEnumerable<PackageInfo> GetPackages()
        {
            return OnGetPackages();
        }

        public PackageConfiguration GetPackageConfiguration(PackageInfo packageInfo)
        {
            return OnGetPackageConfiguration(packageInfo);
        }

        public void SetPackageConfiguration(PackageInfo packageInfo)
        {
            OnSetPackageConfiguration(packageInfo);
        }

        protected abstract IEnumerable<PackageInfo> OnGetPackages();
        protected abstract PackageConfiguration OnGetPackageConfiguration(PackageInfo packageInfo);
        protected abstract void OnSetPackageConfiguration(PackageInfo packageInfo);
        protected abstract bool OnIsProductConfigured();
    }
}
