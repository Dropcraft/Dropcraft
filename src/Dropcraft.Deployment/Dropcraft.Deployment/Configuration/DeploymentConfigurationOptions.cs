using Dropcraft.Common;

namespace Dropcraft.Deployment.Configuration
{
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
        /// Instructs to use the provided .NET framework for installation
        /// </summary>
        /// <param name="frameworkId">Target framework</param>
        /// <returns>Configuration object</returns>
        public DeploymentConfiguration UseFramework(string frameworkId)
        {
            _configuration.TargetFramework = frameworkId;
            return _configuration;
        }

        /// <summary>
        /// Instructs to allow package downgrades
        /// </summary>
        /// <param name="allowDowngrades">Are downgrades allowed</param>
        /// <returns>Configuration object</returns>
        public DeploymentConfiguration AllowDowngrades(bool allowDowngrades)
        {
            _configuration.AllowDowngrades = allowDowngrades;
            return _configuration;
        }
    }
}