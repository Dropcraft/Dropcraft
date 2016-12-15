using System.Collections.Generic;
using Dropcraft.Common;
using NuGet.DependencyResolver;

namespace Dropcraft.Deployment.Workflow
{
    public class DeploymentPackageInfo
    {
        public PackageId Id { get; set; }
        public RemoteMatch Match { get; set; }
        public string PackagePath { get; set; }
        public List<DeploymentPackageInfo> Dependencies { get; } = new List<DeploymentPackageInfo>();
    }
}