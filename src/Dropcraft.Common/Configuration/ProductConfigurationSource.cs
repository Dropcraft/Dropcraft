using System.Collections.Generic;
using Dropcraft.Common.Package;

namespace Dropcraft.Common.Configuration
{
    /// <summary>
    /// ProductConfigurationSource is a source of configuration for installed product and deployed packages
    /// </summary>
    public abstract class ProductConfigurationSource
    {
        protected string ProductPath { get; private set; }
        protected RuntimeContext RuntimeContext { get; private set; }

        protected ProductConfigurationSource(string productPath, RuntimeContext runtimeContext)
        {
            ProductPath = productPath;
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

        public void SetPackageConfiguration(InstallablePackageInfo packageInfo)
        {
            OnSetPackageConfiguration(packageInfo);
        }

        protected abstract IEnumerable<PackageInfo> OnGetPackages();
        protected abstract PackageConfiguration OnGetPackageConfiguration(PackageInfo packageInfo);
        protected abstract void OnSetPackageConfiguration(InstallablePackageInfo packageInfo);
        protected abstract bool OnIsProductConfigured();
    }
}
