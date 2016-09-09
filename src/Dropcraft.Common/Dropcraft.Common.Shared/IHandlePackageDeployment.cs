using Dropcraft.Common.Configuration;
using Dropcraft.Common.Package;

namespace Dropcraft.Common
{
    public interface IHandlePackageDeployment
    {
        void AfterPackageDeployed(PackageInfo packageInfo, DeploymentContext context);
    }
}