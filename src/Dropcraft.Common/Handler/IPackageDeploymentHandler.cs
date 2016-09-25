namespace Dropcraft.Common.Handler
{
    public interface IPackageDeploymentHandler
    {
        void AfterPackageDeployed(PackageInfo packageInfo, DeploymentContext context);
    }
}