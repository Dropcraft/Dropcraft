using Dropcraft.Common;

namespace Dropcraft.Runtime.Configuration
{
    /// <summary>
    /// Allows to define package configuration sources
    /// </summary>
    public class ProductConfigurationSourceOptions
    {
        private readonly RuntimeConfiguration _runtimeConfiguration;

        internal ProductConfigurationSourceOptions(RuntimeConfiguration runtimeConfiguration)
        {
            _runtimeConfiguration = runtimeConfiguration;
        }

        /// <summary>
        /// Instruct to use a custom package configuration source.
        /// </summary>
        /// <param name="configurationSource">Custom configuration source</param>
        /// <returns>Configuration object</returns>
        public RuntimeConfiguration UseCustomConfigurationSource(IProductConfigurationSource configurationSource)
        {
            _runtimeConfiguration.ProductConfigurationSource = configurationSource;
            return _runtimeConfiguration;
        }
    }
}