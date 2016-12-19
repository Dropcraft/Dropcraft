using System.Collections.Generic;
using System.Threading.Tasks;
using Dropcraft.Common;
using Dropcraft.Common.Package;
using NuGet.DependencyResolver;
using NuGet.Versioning;

namespace Dropcraft.Deployment.NuGet
{
    public interface INuGetEngine
    {
        bool AllowDowngrades { get; set; }
        bool UpdatePackages { get; set; }

        void AnalysePackages(GraphNode<RemoteResolveResult> resolvedPackages);
        string GetPackageTargetPath(string packageId, NuGetVersion version, string path);
        Task InstallPackage(RemoteMatch match, string path);
        Task<GraphNode<RemoteResolveResult>> ResolvePackages(ICollection<PackageId> packages);
        Task<PackageId> ResolvePackageVersion(PackageId packageId);
    }
}