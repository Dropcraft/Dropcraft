using Dropcraft.Common.Deployment;

namespace Dropcraft.Deployment.Core
{
    /// <summary>
    /// Class DeploymentTransactionSource.
    /// </summary>
    /// <seealso cref="Dropcraft.Deployment.Core.IDeploymentTransactionSource" />
    public class DeploymentTransactionSource : IDeploymentTransactionSource
    {
        /// <summary>
        /// Starts new transaction.
        /// </summary>
        /// <param name="deploymentContext">The deployment context.</param>
        /// <returns><see cref="IDeploymentTransaction"/></returns>
        public IDeploymentTransaction NewTransaction(DeploymentContext deploymentContext)
        {
            return new DeploymentTransaction();
        }
    }
}