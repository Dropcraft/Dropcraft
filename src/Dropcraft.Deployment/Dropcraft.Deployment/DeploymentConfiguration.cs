using System.Collections.Generic;
using Dropcraft.Common;
using Dropcraft.Common.Configuration;
using Dropcraft.Deployment.Configuration;
using Dropcraft.Runtime.Configuration;

namespace Dropcraft.Deployment
{
    /// <summary>
    /// Configuration object for creating <see cref="IDeploymentEngine"/> instances.
    /// </summary>
    public class DeploymentConfiguration
    {
        /// <summary>
        /// Assigned deployment context
        /// </summary>
        public DeploymentContext DeploymentContext { get; }

        /// <summary>
        /// Instructs to always try to update packages from the remote sources
        /// </summary>
        internal bool UpdatePackages { get; set; }

        /// <summary>
        /// Instructs to allow packages downgrades
        /// </summary>
        internal bool AllowDowngrades { get; set; }

        /// <summary>
        /// Defines target .NET framework
        /// </summary>
        internal string TargetFramework { get; set; }

        /// <summary>
        /// Package configuration sources
        /// </summary>
        internal List<IPackageConfigurationSource> PackageConfigurationSources { get; } = new List<IPackageConfigurationSource>();

        /// <summary>
        /// Product configuration sources
        /// </summary>
        internal IProductConfigurationSource ProductConfigurationSource { get; set; }

        /// <summary>
        /// Deployment filters
        /// </summary>
        internal IDeploymentStrategySource DeploymentStrategySource { get; set; }

        /// <summary>
        /// List of remote package sources
        /// </summary>
        internal List<string> RemotePackagesSources { get; } = new List<string>();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="installPath">Path to deploy composed application</param>
        /// <param name="packagesFolderPath">Path to install packages</param>
        public DeploymentConfiguration(string installPath, string packagesFolderPath)
            : this(new DefaultDeploymentContext(installPath, packagesFolderPath))
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="deploymentContext">Configured custom deployment context to use</param>
        public DeploymentConfiguration(DeploymentContext deploymentContext)
        {
            DeploymentContext = deploymentContext;

            ProductConfigurationSource = new ProductConfigurationSource();
            DeploymentStrategySource = new DeploymentStrategySource();
        }

        /// <summary>
        /// Allows to setup additional options
        /// </summary>
        public DeploymentConfigurationOptions ConfigureTo => new DeploymentConfigurationOptions(this);

        /// <summary>
        /// Allows to setup the configuration sources for packages
        /// </summary>
        public PackagesConfigurationOptions ForPackagesConfiguration => new PackagesConfigurationOptions(this);

        /// <summary>
        /// Allows to setup the configuration sources for product
        /// </summary>
        public ProductConfigurationOptions ForProductConfiguration => new ProductConfigurationOptions(this);

        /// <summary>
        /// Allows to setup the deployment startegy for product
        /// </summary>
        public DeploymentConfigurationOptions ForDeployment => new DeploymentConfigurationOptions(this);

        /// <summary>
        /// Allows to setup local and remote package sources
        /// </summary>
        public PackageSourceOptions ForPackages => new PackageSourceOptions(this);

        /// <summary>
        /// Creates <see cref="IDeploymentEngine"/> instances.
        /// </summary>
        /// <returns><see cref="IDeploymentEngine"/></returns>
        public IDeploymentEngine CreatEngine()
        {
            return new DeploymentEngine(this);
        }
    }
}
