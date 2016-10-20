using System.Collections.Generic;
using Dropcraft.Common;
using Dropcraft.Common.Configuration;

namespace Dropcraft.Deployment.Configuration
{
    public class DeploymentStarategyProvider : IDeploymentStartegyProvider
    {
        private readonly DeploymentContext _deploymentContext;

        public DeploymentStarategyProvider(DeploymentContext deploymentContext)
        {
            _deploymentContext = deploymentContext;
        }

        public IEnumerable<PackageFileInfo> GetPackageFiles(PackageId packageId, string packagePath)
        {
            throw new System.NotImplementedException();
        }
    }
}