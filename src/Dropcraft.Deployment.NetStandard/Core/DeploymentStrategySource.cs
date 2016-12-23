using Dropcraft.Common.Deployment;

namespace Dropcraft.Deployment.Core
{
    /// <summary>
    /// Default implementation for <see cref="IDeploymentStrategySource"/>
    /// </summary>
    public class DeploymentStrategySource : IDeploymentStrategySource
    {
        /// <summary>
        /// Defines default conflict resolution strategy. It will be used for all detected conflicts.
        /// </summary>
        public FileConflictResolution DefaultConflictResolution { get; set; } = FileConflictResolution.KeepExisting;

        /// <summary>
        /// Returns strategy provider for the DeploymentContext
        /// </summary>
        /// <param name="deploymentContext">Deployment context</param>
        /// <returns>Deployment strategy</returns>
        public IDeploymentStartegyProvider GetStartegyProvider(DeploymentContext deploymentContext)
        {
            return new DeploymentStarategyProvider(deploymentContext)
            {
                DefaultConflictResolution = DefaultConflictResolution
            };
        }
    }
}