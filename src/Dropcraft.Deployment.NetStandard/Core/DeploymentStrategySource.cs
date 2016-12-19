using Dropcraft.Common;
using Dropcraft.Common.Deployment;

namespace Dropcraft.Deployment.Core
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