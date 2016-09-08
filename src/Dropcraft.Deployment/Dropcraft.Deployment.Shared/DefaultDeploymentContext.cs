using Dropcraft.Common;

namespace Dropcraft.Deployment
{
    public class DefaultDeploymentContext : DeploymentContext
    {
        public DefaultDeploymentContext(string installPath, string packagesFolderPath) 
            : base(installPath, packagesFolderPath)
        {
        }
    }
}