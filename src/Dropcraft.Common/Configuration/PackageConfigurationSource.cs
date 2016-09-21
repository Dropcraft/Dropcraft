using Dropcraft.Common.Package;

namespace Dropcraft.Common.Configuration
{
    /// <summary>
    /// PackageConfigurationSource is a source of configuration for packages during deployment.
    /// </summary>
    public abstract class PackageConfigurationSource
    {
        protected DeploymentContext DeploymentContext { get; private set; }

        protected PackageConfigurationSource(DeploymentContext deploymentContext)
        {
            DeploymentContext = deploymentContext;
        }

        public PackageConfiguration GetPackageConfiguration(InstallablePackageInfo packageInfo)
        {
            return OnGetPackageConfiguration(packageInfo);
        }

        protected abstract PackageConfiguration OnGetPackageConfiguration(InstallablePackageInfo packageInfo);
    }
}