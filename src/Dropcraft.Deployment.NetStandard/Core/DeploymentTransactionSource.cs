using Dropcraft.Common.Deployment;

namespace Dropcraft.Deployment.Core
{
    public class DeploymentTransactionSource : IDeploymentTransactionSource
    {
        public IDeploymentTransaction NewTransaction(DeploymentContext deploymentContext)
        {
            return new DeploymentTransaction();
        }
    }
}