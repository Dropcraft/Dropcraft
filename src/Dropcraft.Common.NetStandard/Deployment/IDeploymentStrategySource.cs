namespace Dropcraft.Common.Deployment
{
    /// <summary>
    /// Source of <see cref="IDeploymentStartegyProvider"/>
    /// </summary>
    public interface IDeploymentStrategySource
    {
        /// <summary>
        /// Returns strategy provider for the DeploymentContext
        /// </summary>
        /// <param name="deploymentContext">Deployment context</param>
        /// <returns>Deployment strategy</returns>
        IDeploymentStartegyProvider GetStartegyProvider(DeploymentContext deploymentContext);
    }
}