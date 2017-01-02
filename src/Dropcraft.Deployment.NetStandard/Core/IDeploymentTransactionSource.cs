using Dropcraft.Common.Deployment;

namespace Dropcraft.Deployment.Core
{
    /// <summary>
    /// Source of the transactions
    /// </summary>
    public interface IDeploymentTransactionSource
    {
        /// <summary>
        /// Starts new transaction.
        /// </summary>
        /// <param name="deploymentContext">The deployment context.</param>
        /// <returns><see cref="IDeploymentTransaction"/></returns>
        IDeploymentTransaction NewTransaction(DeploymentContext deploymentContext);
    }
}