using Dropcraft.Common.Package;

namespace Dropcraft.Deployment.Configuration
{
    /// <summary>
    /// Class PackagesConfigurationOptions.
    /// </summary>
    public class PackagesConfigurationOptions
    {
        readonly DeploymentConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="PackagesConfigurationOptions"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public PackagesConfigurationOptions(DeploymentConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Allows to define custom package configuration source
        /// </summary>
        /// <param name="source">Custom configuration source</param>
        /// <returns>Configuration object</returns>
        public DeploymentConfiguration AddConfigurationSource(IPackageConfigurationSource source)
        {
            _configuration.PackageConfigurationSource = source;
            return _configuration;
        }
    }
}