using System.Collections.Generic;
using System.Threading.Tasks;
using Dropcraft.Common;
using Dropcraft.Common.Deployment;
using Dropcraft.Deployment.NuGet;

namespace Dropcraft.Deployment.Core
{
    public class InstallationPlan
    {
        public int UpdateCount { get; set; }
        public int InstallCount { get; set; }
        public IPackageGraph TargetPackageGraph { get; set; }
        public ICollection<DeploymentAction> Actions { get; set; }
        public ICollection<PackageId> RemainingPackages { get; set; }

        public InstallationPlan()
        {
            Actions = new List<DeploymentAction>();
            RemainingPackages = new List<PackageId>();
        }
    }

    public class UninstallationPlan
    {
        public ICollection<DeploymentAction> Actions { get; set; }
    }

    public interface IPackageDeployer
    {
        IDeploymentStartegyProvider DeploymentStartegyProvider { get; set; }
        INuGetEngine NuGetEngine { get; set; }
        string PackagesFolderPath { get; set; }
        DeploymentContext DeploymentContext { get; set; }

        Task<InstallationPlan> PlanInstallation(IPackageGraph productPackages, ICollection<PackageId> packages);

        Task<UninstallationPlan> PlanUninstallation(IPackageGraph productPackages, ICollection<PackageId> packages,
            bool removeDependencies);
    }
}