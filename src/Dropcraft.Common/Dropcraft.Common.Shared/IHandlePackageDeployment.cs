using Dropcraft.Common.Configuration;

namespace Dropcraft.Common
{
    public interface IHandlePackageDeployment
    {
        void AfterPackageDeployed(PackageInfo packageInfo, DeploymentContext context);
    }
}