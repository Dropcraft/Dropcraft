using System.Collections.Generic;
using Dropcraft.Common;

namespace Dropcraft.Deployment
{
    public class DeploymentConfiguration
    {
        public DeploymentContext DeploymentContext { get; set; }

        /// <summary>
        /// Path to install/uninstall packages 
        /// </summary>
        public string InstallPath { get; set; }

        /// <summary>
        /// List of the package sources
        /// </summary>
        public List<string> PackageSources { get; } = new List<string>();

        public DeploymentConfiguration(string installPath)
        {
            InstallPath = installPath;
        }

        public DeploymentConfiguration AddPackageSource(string packageSource)
        {
            PackageSources.Add(packageSource);
            return this;
        }

        public IDeploymentEngine CreatEngine()
        {
            return new DeploymentEngine(this);
        }
    }
}
