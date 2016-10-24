using Dropcraft.Common;
using Dropcraft.Common.Configuration;

namespace Dropcraft.Deployment.Configuration
{
    public class DeploymentStrategySource : IDeploymentStrategySource
    {
        /// <summary>
        /// Defines default conflict resolution strategy
        /// </summary>
        public FileConflictResolution DefaultConflictResolution { get; set; } = FileConflictResolution.Override;

        public IDeploymentStartegyProvider GetStartegyProvider(DeploymentContext deploymentContext)
        {
            return new DeploymentStarategyProvider(deploymentContext)
            {
                DefaultConflictResolution = DefaultConflictResolution
            };
        }
    }
}