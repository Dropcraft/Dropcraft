using Dropcraft.Common;
using Dropcraft.Deployment.Options;

namespace Dropcraft.Deployment
{
    class DeploymentEngine : IDeploymentEngine
    {
        public DeploymentContext DeploymentContext { get; }


        public void InstallPackages(InstallOptions options)
        {
            throw new System.NotImplementedException();
        }

        public void UpdatePackages(UpdateOptions options)
        {
            throw new System.NotImplementedException();
        }

        public void UninstallPackages(UninstallOptions options)
        {
            throw new System.NotImplementedException();
        }
    }
}
