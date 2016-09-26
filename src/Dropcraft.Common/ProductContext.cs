namespace Dropcraft.Common
{
    /// <summary>
    /// ProductContext defines properties required for product deployment and execution
    /// </summary>
    public abstract class ProductContext
    {
        /// <summary>
        /// Product installation path
        /// </summary>
        public string ProductPath { get; private set; }

        protected ProductContext(string productPath)
        {
            ProductPath = productPath;
        }
    }
}