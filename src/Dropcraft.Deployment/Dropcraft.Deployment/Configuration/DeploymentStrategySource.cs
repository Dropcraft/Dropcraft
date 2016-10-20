using Dropcraft.Common;
using Dropcraft.Common.Configuration;

namespace Dropcraft.Deployment.Configuration
{
    public class DeploymentStrategySource : IDeploymentStrategySource
    {
        public IDeploymentStartegyProvider GetStartegyProvider(DeploymentContext deploymentContext)
        {
            return new DeploymentStarategyProvider(deploymentContext);
        }
    }
}