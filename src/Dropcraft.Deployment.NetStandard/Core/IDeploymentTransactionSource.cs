using Dropcraft.Common.Deployment;

namespace Dropcraft.Deployment.Core
{
    public interface IDeploymentTransactionSource
    {
        IDeploymentTransaction NewTransaction(DeploymentContext deploymentContext);
    }
}