using Dropcraft.Common.Configuration;
using Dropcraft.Runtime.Configuration;

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
        /// Instructs to use the default product configuration source
        /// </summary>
        /// <returns>Configuration object</returns>
        public DeploymentConfiguration UseDefaultConfigurationSource()
        {
            _configuration.ProductConfigurationSource = new ProductConfigurationSource();
            return _configuration;
        }

        /// <summary>
        /// Allows to define custom product configuration source
        /// </summary>
        /// <param name="source">Custom configuration source</param>
        /// <returns>Configuration object</returns>
        public DeploymentConfiguration UseCustomConfigurationSource(IProductConfigurationSource source)
        {
            _configuration.ProductConfigurationSource = source;
            return _configuration;
        }
    }
}