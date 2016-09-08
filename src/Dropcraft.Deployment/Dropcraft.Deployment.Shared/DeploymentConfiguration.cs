using System.Collections.Generic;
using Dropcraft.Common;

namespace Dropcraft.Deployment
{
    public class DeploymentConfiguration
    {
        public bool UpdatePackages { get; private set; }

        public DeploymentContext DeploymentContext { get; }

        /// <summary>
        /// List of the package sources
        /// </summary>
        public List<string> PackageSources { get; } = new List<string>();

        public DeploymentConfiguration(string installPath, string packagesFolderPath)
            : this(new DefaultDeploymentContext(installPath, packagesFolderPath))
        {
        }

        public DeploymentConfiguration(DeploymentContext deploymentContext)
        {
            DeploymentContext = deploymentContext;
        }

        public DeploymentConfiguration AddPackageSource(string packageSource)
        {
            PackageSources.Add(packageSource);
            return this;
        }

        public DeploymentConfiguration UpdatePackagesFromSource(bool update)
        {
            UpdatePackages = update;
            return this;
        }

        public IDeploymentEngine CreatEngine()
        {
            return new DeploymentEngine(this);
        }
    }
}
