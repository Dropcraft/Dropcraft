using System.Collections.Generic;
using Dropcraft.Common;
using NuGet.DependencyResolver;

namespace Dropcraft.Deployment.Workflow
{
    public class PackageInfo
    {
        public PackageId Id { get; set; }
        public RemoteMatch Match { get; set; }
        public string PackagePath { get; set; }
        public List<PackageInfo> Dependencies { get; } = new List<PackageInfo>();
    }
}