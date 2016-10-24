using System.Collections.Generic;
using System.IO;
using Dropcraft.Common;
using Dropcraft.Common.Configuration;

namespace Dropcraft.Deployment.Configuration
{
    public class DeploymentStarategyProvider : IDeploymentStartegyProvider
    {
        private readonly DeploymentContext _deploymentContext;
        public FileConflictResolution DefaultConflictResolution { get; set; } = FileConflictResolution.Override;

        public DeploymentStarategyProvider(DeploymentContext deploymentContext)
        {
            _deploymentContext = deploymentContext;
        }

        public IEnumerable<PackageFileInfo> GetPackageFiles(PackageId packageId, string packagePath)
        {
            var files = new List<PackageFileInfo>();
            var toolsFolder = Path.Combine(packagePath, "tools");
            var contentFolder = Path.Combine(packagePath, "content");
            var libFolder = Path.Combine(packagePath, "lib");

            if (Directory.Exists(toolsFolder))
            {
                //Directory.EnumerateFiles(toolsFolder);
                //Directory.EnumerateDirectories(toolsFolder);
            }

            return files;
        }
    }
}