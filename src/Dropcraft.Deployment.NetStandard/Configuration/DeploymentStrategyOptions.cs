using Dropcraft.Common.Deployment;
using Dropcraft.Deployment.Core;

namespace Dropcraft.Deployment.Configuration
{
    public class DeploymentStrategyOptions
    {
        readonly DeploymentConfiguration _configuration;

        public DeploymentStrategyOptions(DeploymentConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Allows to define custom deployment strategy source
        /// </summary>
        /// <param name="strategySource">Custom deployment strategy source</param>
        /// <returns>Configuration object</returns>
        public DeploymentConfiguration UseStrategy(IDeploymentStrategySource strategySource)
        {
            _configuration.DeploymentStrategySource = strategySource;
            return _configuration;
        }

        /// <summary>
        /// Allows to define custom transaction source
        /// </summary>
        /// <param name="transactionSource">Transaction source</param>
        /// <returns>Configuration object</returns>
        public DeploymentConfiguration UseTransactionSource(IDeploymentTransactionSource transactionSource)
        {
            _configuration.TransactionSource = transactionSource;
            return _configuration;
        }

        /// <summary>
        /// Allows to define custom package deployer
        /// </summary>
        /// <param name="packageDeployer">Package deployer</param>
        /// <returns>Configuration object</returns>
        public DeploymentConfiguration UsePackageDeployer(IPackageDeployer packageDeployer)
        {
            _configuration.PackageDeployer = packageDeployer;
            return _configuration;
        }

        /// <summary>
        /// Allows to define custom package discoverer
        /// </summary>
        /// <param name="packageDiscoverer">Package discoverer</param>
        /// <returns>Configuration object</returns>
        public DeploymentConfiguration UsePackageDiscoverer(IPackageDiscoverer packageDiscoverer)
        {
            _configuration.PackageDiscoverer = packageDiscoverer;
            return _configuration;
        }
    }
}