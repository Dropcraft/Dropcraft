namespace Dropcraft.Common
{
    public abstract class DeploymentContext : ProductContext
    {
        /// <summary>
        /// Path to install/uninstall packages 
        /// </summary>
        public string PackagesFolderPath { get; }

        protected DeploymentContext(string productPath, string packagesFolderPath)
            : base(productPath)
        {
            PackagesFolderPath = packagesFolderPath;
        }
    }
}