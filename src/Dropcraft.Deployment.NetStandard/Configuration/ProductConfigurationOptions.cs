using Dropcraft.Common;

namespace Dropcraft.Deployment.Configuration
{
    /// <summary>
    /// Class ProductConfigurationOptions.
    /// </summary>
    public class ProductConfigurationOptions
    {
        readonly DeploymentConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductConfigurationOptions"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
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