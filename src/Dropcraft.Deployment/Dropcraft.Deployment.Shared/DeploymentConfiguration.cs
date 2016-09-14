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
        /// Package configuration parsers
        /// </summary>
        internal List<IDeploymentPackageConfigParser> PackageConfigurationParsers { get; } = new List<IDeploymentPackageConfigParser>();

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
        /// Adds default package configuration parser
        /// </summary>
        /// <returns>Configuration object</returns>
        public DeploymentConfiguration AddDefaultConfigurationParser()
        {
            //PackageConfigurationParsers.Add(new PackageManifestParser()); TODO
            return this;
        }

        /// <summary>
        /// Allows to define custom package configuration parsers
        /// </summary>
        /// <param name="parser">Custom configuration parser</param>
        /// <returns>Configuration object</returns>
        public DeploymentConfiguration AddPackageConfigurationParser(IDeploymentPackageConfigParser parser)
        {
            PackageConfigurationParsers.Add(parser);
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
        /// <returns>Configuration object</returns>
        public DeploymentConfigurationOptions ConfigureTo()
        {
            return new DeploymentConfigurationOptions(this);
        }

        /// <summary>
        /// Creates <see cref="IDeploymentEngine"/> instances.
        /// </summary>
        /// <returns><see cref="IDeploymentEngine"/></returns>
        public IDeploymentEngine CreatEngine()
        {
            return new DeploymentEngine(this);
        }
    }

    /// <summary>
    /// Allows to setup additional options
    /// </summary>
    public class DeploymentConfigurationOptions
    {
        readonly DeploymentConfiguration _configuration;

        public DeploymentConfigurationOptions(DeploymentConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Instructs to always update packages from the remote sources, default value is false
        /// </summary>
        /// <param name="update">When true, packages will be always updated from the remote source, even if they can be resolved from the installed path</param>
        /// <returns>Configuration object</returns>
        public DeploymentConfiguration UpdatePackagesFromSource(bool update)
        {
            _configuration.UpdatePackages = update;
            return _configuration;
        }

        /// <summary>
        /// Defines a default method of the file conflict resolution
        /// </summary>
        /// <param name="resolutionStrategy">Resolution strategy</param>
        /// <returns>Configuration object</returns>
        public DeploymentConfiguration ResolveFileConflictsUsing(FileConflictResolution resolutionStrategy)
        {
            _configuration.DefaultConflictResolution = resolutionStrategy;
            return _configuration;
        }
    }

}
