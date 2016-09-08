using System.Collections.Generic;
using System.Threading.Tasks;
using Dropcraft.Common;
using Dropcraft.Common.Configuration;

namespace Dropcraft.Deployment
{
    public interface IDeploymentEngine
    {
        DeploymentContext DeploymentContext { get; }

        Task InstallPackages(IEnumerable<InstallablePackageInfo> packages);
        Task UpdatePackages(IEnumerable<InstallablePackageInfo> packages);
        Task UninstallPackages(IEnumerable<InstallablePackageInfo> packages);
    }
}
