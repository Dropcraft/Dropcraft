namespace Dropcraft.Common
{
    public abstract class DeploymentContext
    {
        /// <summary>
        /// Path to install/uninstall product 
        /// </summary>
        public string InstallPath { get; }

        /// <summary>
        /// Path to install/uninstall packages 
        /// </summary>
        public string PackagesFolderPath { get; set; }

        protected DeploymentContext(string installPath, string packagesFolderPath)
        {
            InstallPath = installPath;
            PackagesFolderPath = packagesFolderPath;
        }
    }
}