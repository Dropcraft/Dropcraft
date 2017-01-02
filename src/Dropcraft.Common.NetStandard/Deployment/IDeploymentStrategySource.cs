namespace Dropcraft.Common.Deployment
{
    /// <summary>
    /// Source of <see cref="IDeploymentStrategyProvider"/>
    /// </summary>
    public interface IDeploymentStrategySource
    {
        /// <summary>
        /// Returns strategy provider for the DeploymentContext
        /// </summary>
        /// <param name="deploymentContext">Deployment context</param>
        /// <returns>Deployment strategy</returns>
        IDeploymentStrategyProvider GetStrategyProvider(DeploymentContext deploymentContext);
    }
}