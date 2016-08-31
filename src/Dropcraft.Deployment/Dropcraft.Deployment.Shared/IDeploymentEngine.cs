using Dropcraft.Common;
using Dropcraft.Deployment.Options;

namespace Dropcraft.Deployment
{
    public interface IDeploymentEngine
    {
        DeploymentContext DeploymentContext { get; }

        void InstallPackages(InstallOptions options);
        void UpdatePackages(UpdateOptions options);
        void UninstallPackages(UninstallOptions options);
    }
}
