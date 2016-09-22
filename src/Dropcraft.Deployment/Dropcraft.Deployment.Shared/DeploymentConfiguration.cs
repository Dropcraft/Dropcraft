using System.Collections.Generic;
using Dropcraft.Common;
using Dropcraft.Common.Configuration;
using Dropcraft.Common.Package;

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
        /// Defines default conflict resolution strategy
        /// </summary>
        internal FileConflictResolution DefaultConflictResolution { get; set; }

        /// <summary>
        /// Package configuration sources
        /// </summary>
        internal List<PackageConfigurationSource> PackageConfigurationSources { get; } = new List<PackageConfigurationSource>();

        /// <summary>
        /// Product configuration sources
        /// </summary>
        internal List<ProductConfigurationSource> ProductConfigurationSources { get; } = new List<ProductConfigurationSource>();

        /// <summary>
        /// Deployment filters
        /// </summary>
        internal List<IDeploymentFilter> DeploymentFilters { get; } = new List<IDeploymentFilter>();

        /// <summary>
        /// List of the package sources
        /// </summary>
        internal List<string> PackageSources { get; } = new List<string>();

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
        }

        /// <summary>
        /// Adds a new package source to the available package sources list 
        /// </summary>
        /// <param name="packageSource">Package source  URI or path</param>
        /// <returns>Configuration object</returns>
        public DeploymentConfiguration AddPackageSource(string packageSource)
        {
            PackageSources.Add(packageSource);
            return this;
        }

        /// <summary>
        /// Adds <see cref="IDeploymentFilter"/> filter to use during deployment
        /// </summary>
        /// <param name="filter">Filter instance</param>
        /// <returns></returns>
        public DeploymentConfiguration AddDeploymentFilter(IDeploymentFilter filter)
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
        public PackageConfigurationSourcesOptions PackageConfigurationSource => new PackageConfigurationSourcesOptions(this);

        /// <summary>
        /// Allows to setup the configuration sources for product
        /// </summary>
        public ProductConfigurationSourcesOptions ProductConfigurationSource => new ProductConfigurationSourcesOptions(this);

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
