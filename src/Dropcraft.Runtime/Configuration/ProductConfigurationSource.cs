using Dropcraft.Common;
using Dropcraft.Common.Configuration;

namespace Dropcraft.Runtime.Configuration
{
    public class ProductConfigurationSource : IProductConfigurationSource
    {
        /// <summary>
        /// Defines product configuration file name to store information about installed packages, dependencies, etc.
        /// </summary>
        public string ProductConfigurationFileName { get; set; } = "dropcraft.json";

        public IProductConfigurationProvider GetProductConfigurationProvider(IProductContext context)
        {
            return new ProductConfigurationProvider(context, ProductConfigurationFileName);
        }
    }
}