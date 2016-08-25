using Dropcraft.Contracts.Configuration;

namespace Dropcraft.Contracts
{
    public interface IHandlePackageDeployment
    {
        void AfterPackageDeployed(PackageInfo packageInfo, IDeploymentContext context);
    }
}