using Dropcraft.Common;
using NuGet.DependencyResolver;

namespace Dropcraft.Deployment.Core
{
    public class DeploymentPackageInfo
    {
        public PackageId Id { get; set; }
        public RemoteMatch Match { get; set; }
        public string PackagePath { get; set; }
    }
}