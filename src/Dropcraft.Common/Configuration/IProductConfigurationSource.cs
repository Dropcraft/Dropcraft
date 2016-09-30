namespace Dropcraft.Common.Configuration
{
    /// <summary>
    /// IProductConfigurationSource is a source of configuration for installed product and deployed packages
    /// </summary>
    public interface IProductConfigurationSource
    {
        IProductConfigurationProvider GetProductConfigurationProvider(IProductContext context);
    }
}
