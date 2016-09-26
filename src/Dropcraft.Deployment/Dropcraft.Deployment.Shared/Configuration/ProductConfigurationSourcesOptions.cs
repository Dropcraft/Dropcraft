using Dropcraft.Common.Configuration;
using Dropcraft.Runtime.Configuration;

namespace Dropcraft.Deployment
{
    public class ProductConfigurationSourcesOptions
    {
        readonly DeploymentConfiguration _configuration;

        public ProductConfigurationSourcesOptions(DeploymentConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Adds default product configuration source
        /// </summary>
        /// <returns>Configuration object</returns>
        public DeploymentConfiguration AddDefault()
        {
            _configuration.ProductConfigurationSource.Add(new DefaultProductConfigurationSource(_configuration.DeploymentContext));
            return _configuration;
        }

        /// <summary>
        /// Allows to define custom product configuration source
        /// </summary>
        /// <param name="source">Custom configuration source</param>
        /// <returns>Configuration object</returns>
        public DeploymentConfiguration Add(ProductConfigurationSource source)
        {
            _configuration.ProductConfigurationSources.Add(source);
            return _configuration;
        }
    }
}