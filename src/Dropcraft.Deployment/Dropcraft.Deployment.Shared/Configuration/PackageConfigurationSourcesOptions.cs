using Dropcraft.Common.Configuration;
using Dropcraft.Runtime.Configuration;

namespace Dropcraft.Deployment
{
    public class PackageConfigurationSourcesOptions
    {
        readonly DeploymentConfiguration _configuration;

        public PackageConfigurationSourcesOptions(DeploymentConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Adds default package configuration source
        /// </summary>
        /// <returns>Configuration object</returns>
        public DeploymentConfiguration AddDefaultConfigurationSource()
        {
            _configuration.PackageConfigurationSources.Add(new PackageConfigurationSource());
            return _configuration;
        }

        /// <summary>
        /// Allows to define custom package configuration source
        /// </summary>
        /// <param name="source">Custom configuration source</param>
        /// <returns>Configuration object</returns>
        public DeploymentConfiguration AddDefaultSource(IPackageConfigurationSource source)
        {
            _configuration.PackageConfigurationSources.Add(source);
            return _configuration;
        }
    }
}