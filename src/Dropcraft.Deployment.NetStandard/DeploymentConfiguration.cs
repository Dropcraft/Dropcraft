using System.Collections.Generic;
using Dropcraft.Common;
using Dropcraft.Common.Deployment;
using Dropcraft.Common.Package;
using Dropcraft.Deployment.Configuration;
using Dropcraft.Deployment.Core;
using Dropcraft.Runtime.Core;

namespace Dropcraft.Deployment
{
    /// <summary>
    /// Configuration object for creating <see cref="IDeploymentEngine"/> instances.
    /// </summary>
    public class DeploymentConfiguration
    {
        /// <summary>
        /// Path to install/uninstall packages 
        /// </summary>
        internal string PackagesFolderPath { get; set; }

        /// <summary>
        /// Package configuration source
        /// </summary>
        internal IPackageConfigurationSource PackageConfigurationSource { get; set; }

        /// <summary>
        /// Product configuration source
        /// </summary>
        internal IProductConfigurationSource ProductConfigurationSource { get; set; }

        /// <summary>
        /// Deployment filters
        /// </summary>
        internal IDeploymentStrategySource DeploymentStrategySource { get; set; }

        /// <summary>
        /// Deployment transaction source
        /// </summary>
        internal IDeploymentTransactionSource TransactionSource { get; set; }

        /// <summary>
        /// Package deployment functionality
        /// </summary>
        internal IPackageDeployer PackageDeployer { get; set; }

        /// <summary>
        /// Package discovery functionality
        /// </summary>
        internal IPackageDiscoverer PackageDiscoverer { get; set; }

        /// <summary>
        /// List of remote package sources
        /// </summary>
        internal List<string> RemotePackagesSources { get; } = new List<string>();

        /// <summary>
        /// List of local package sources
        /// </summary>
        internal List<string> LocalPackagesSources { get; } = new List<string>();

        /// <summary>
        /// Constructor
        /// </summary>
        public DeploymentConfiguration()
        {
            ProductConfigurationSource = new ProductConfigurationSource();
            PackageConfigurationSource = new PackageConfigurationSource();
            DeploymentStrategySource = new DeploymentStrategySource();
            TransactionSource = new DeploymentTransactionSource();
            PackageDeployer = new PackageDeployer();
            PackageDiscoverer = new PackageDiscoverer();
        }

        /// <summary>
        /// Allows to setup the configuration sources for packages
        /// </summary>
        public PackagesConfigurationOptions ForPackagesConfiguration => new PackagesConfigurationOptions(this);

        /// <summary>
        /// Allows to setup the configuration sources for product
        /// </summary>
        public ProductConfigurationOptions ForProductConfiguration => new ProductConfigurationOptions(this);

        /// <summary>
        /// Allows to setup the deployment strategy for product
        /// </summary>
        public DeploymentStrategyOptions ForDeployment => new DeploymentStrategyOptions(this);

        /// <summary>
        /// Allows to setup local and remote package sources
        /// </summary>
        public PackageSourceOptions ForPackages => new PackageSourceOptions(this);

        /// <summary>
        /// Creates <see cref="IDeploymentEngine"/> instances.
        /// </summary>
        /// <param name="deploymentContext">Custom deployment context</param>
        /// <returns><see cref="IDeploymentEngine"/></returns>
        public IDeploymentEngine CreateEngine(DeploymentContext deploymentContext)
        {
            var deploymentStrategyProvider = DeploymentStrategySource.GetStrategyProvider(deploymentContext);

            return new DeploymentEngine(deploymentContext, deploymentStrategyProvider, PackageDiscoverer,
                PackageDeployer, TransactionSource,
                PackagesFolderPath,
                RemotePackagesSources, 
                LocalPackagesSources);
        }

        /// <summary>
        /// Creates <see cref="IDeploymentEngine"/> instances.
        /// </summary>
        /// <param name="productPath">The product path.</param>
        /// <param name="framework">The .NET framework.</param>
        /// <returns><see cref="IDeploymentEngine"/></returns>
        public IDeploymentEngine CreateEngine(string productPath, string framework)
        {
            var packageConfigurationProvider = PackageConfigurationSource.GetPackageConfigurationProvider();
            var productConfigurationProvider = ProductConfigurationSource.GetProductConfigurationProvider(productPath);
            var deploymentContext = new DefaultDeploymentContext(productPath, framework, packageConfigurationProvider,
                productConfigurationProvider);

            return CreateEngine(deploymentContext);
        }
    }
}
