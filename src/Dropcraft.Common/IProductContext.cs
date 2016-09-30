namespace Dropcraft.Common
{
    /// <summary>
    /// IProductContext defines properties required for product deployment and execution
    /// </summary>
    public interface IProductContext
    {
        /// <summary>
        /// Product installation path
        /// </summary>
        string ProductPath { get; }
    }
}