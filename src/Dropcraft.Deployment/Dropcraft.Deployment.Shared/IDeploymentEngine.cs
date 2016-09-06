using System.Collections.Generic;
using Dropcraft.Common;
using Dropcraft.Common.Configuration;

namespace Dropcraft.Deployment
{
    public interface IDeploymentEngine
    {
        DeploymentContext DeploymentContext { get; }

        void InstallPackages(IEnumerable<PackageId> packages);
        void UpdatePackages(IEnumerable<PackageId> packages);
        void UninstallPackages(IEnumerable<PackageId> packages);
    }
}
