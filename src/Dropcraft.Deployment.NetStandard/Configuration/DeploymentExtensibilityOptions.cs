using Dropcraft.Common.Package;

namespace Dropcraft.Deployment.Configuration
{
    /// <summary>
    /// Allows to configure extensibility options
    /// </summary>
    public class DeploymentExtensibilityOptions
    {
        private readonly DeploymentConfiguration _deploymentConfiguration;

        internal DeploymentExtensibilityOptions(DeploymentConfiguration deploymentConfiguration)
        {
            _deploymentConfiguration = deploymentConfiguration;
        }

        /// <summary>
        /// Allows to setup a custom activator for the package entities
        /// </summary>
        /// <param name="entityActivator">Custom activator</param>
        /// <returns>Configuration object</returns>
        public DeploymentConfiguration UsePackageEntityActivator(IEntityActivator entityActivator)
        {
            _deploymentConfiguration.EntityActivator = entityActivator;
            return _deploymentConfiguration;
        }
    }
}