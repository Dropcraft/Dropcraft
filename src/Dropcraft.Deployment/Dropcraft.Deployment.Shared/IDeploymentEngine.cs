using System.Collections.Generic;
using System.Threading.Tasks;
using Dropcraft.Common;
using Dropcraft.Common.Package;

namespace Dropcraft.Deployment
{
    public interface IDeploymentEngine
    {
        DeploymentContext DeploymentContext { get; }

        Task InstallPackages(IEnumerable<VersionedPackageInfo> packages);
        Task UpdatePackages(IEnumerable<VersionedPackageInfo> packages);
        Task UninstallPackages(IEnumerable<VersionedPackageInfo> packages);
    }
}
