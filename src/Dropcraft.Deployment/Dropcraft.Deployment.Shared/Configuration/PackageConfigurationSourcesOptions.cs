using Dropcraft.Common.Configuration;

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
        public DeploymentConfiguration AddDefault()
        {
            //PackageConfigurationSources.Add(new PackageManifestParser()); TODO
            return _configuration;
        }

        /// <summary>
        /// Allows to define custom package configuration source
        /// </summary>
        /// <param name="source">Custom configuration source</param>
        /// <returns>Configuration object</returns>
        public DeploymentConfiguration Add(PackageConfigurationSource source)
        {
            _configuration.PackageConfigurationSources.Add(source);
            return _configuration;
        }
    }
}