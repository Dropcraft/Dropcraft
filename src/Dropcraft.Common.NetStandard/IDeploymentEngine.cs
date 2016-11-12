using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dropcraft.Common
{
    public interface IDeploymentEngine
    {
        DeploymentContext DeploymentContext { get; }

        Task InstallPackages(IEnumerable<PackageId> packages);
        Task UninstallPackages(IEnumerable<PackageId> packages);
    }
}
