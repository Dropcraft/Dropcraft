using System.Collections.Generic;
using System.Threading.Tasks;
using Dropcraft.Common;
using Dropcraft.Common.Package;
using Dropcraft.Deployment.NuGet;

namespace Dropcraft.Deployment.Core
{
    public interface IPackageDiscoverer
    {
        INuGetEngine NuGetEngine { get; set; }

        Task<ICollection<PackageId>> Discover(IPackageGraph packageGraph, ICollection<PackageId> newPackages);
    }
}