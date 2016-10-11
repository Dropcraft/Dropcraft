using System.Collections.Generic;
using Dropcraft.Common;
using Dropcraft.Common.Configuration;
using Dropcraft.Common.Handler;
using Dropcraft.Deployment.Configuration;

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
        public IDeploymentContext DeploymentContext { get; }

        /// <summary>
        /// Instructs to always try to update packages from the remote sources
        /// </summary>
        internal bool UpdatePackages { get; set; }

        /// <summary>
        /// Instructs to allow packages downgrades
        /// </summary>
        internal bool AllowDowngrades { get; set; }

        /// <summary>
        /// Defines default conflict resolution strategy
        /// </summary>
        internal FileConflictResolution DefaultConflictResolution { get; set; }

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
        internal List<IPackageFileFilteringHandler> DeploymentFilters { get; } = new List<IPackageFileFilteringHandler>();

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
            : this(new DeploymentContext(installPath, packagesFolderPath))
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="deploymentContext">Configured custom deployment context to use</param>
        public DeploymentConfiguration(IDeploymentContext deploymentContext)
        {
            DeploymentContext = deploymentContext;
        }

        /// <summary>
        /// Adds <see cref="IPackageFileFilteringHandler"/> filter to use during deployment
        /// </summary>
        /// <param name="filter">Filter instance</param>
        /// <returns></returns>
        public DeploymentConfiguration AddDeploymentFilter(IPackageFileFilteringHandler filter)
        {
            DeploymentFilters.Add(filter);
            return this;
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
