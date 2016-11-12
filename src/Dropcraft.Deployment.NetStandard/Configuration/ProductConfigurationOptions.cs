using Dropcraft.Common.Configuration;

namespace Dropcraft.Deployment.Configuration
{
    public class ProductConfigurationOptions
    {
        readonly DeploymentConfiguration _configuration;

        public ProductConfigurationOptions(DeploymentConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Allows to define custom product configuration source
        /// </summary>
        /// <param name="source">Custom configuration source</param>
        /// <returns>Configuration object</returns>
        public DeploymentConfiguration UseConfigurationSource(IProductConfigurationSource source)
        {
            _configuration.ProductConfigurationSource = source;
            return _configuration;
        }
    }
}