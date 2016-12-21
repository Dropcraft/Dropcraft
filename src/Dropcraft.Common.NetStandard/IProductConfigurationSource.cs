namespace Dropcraft.Common
{
    /// <summary>
    /// IProductConfigurationSource is a source of configuration for installed product and deployed packages
    /// </summary>
    public interface IProductConfigurationSource
    {
        /// <summary>
        /// Returns product configuration provider for the provided path
        /// </summary>
        /// <param name="productPath">Product path</param>
        /// <returns><see cref="IProductConfigurationProvider"/></returns>
        IProductConfigurationProvider GetProductConfigurationProvider(string productPath);
    }
}
