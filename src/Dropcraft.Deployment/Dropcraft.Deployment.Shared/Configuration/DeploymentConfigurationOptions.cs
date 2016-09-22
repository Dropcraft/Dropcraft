using Dropcraft.Common.Package;

namespace Dropcraft.Deployment
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