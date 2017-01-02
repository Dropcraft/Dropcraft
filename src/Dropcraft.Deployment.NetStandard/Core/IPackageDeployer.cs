using System.Collections.Generic;
using System.Threading.Tasks;
using Dropcraft.Common;
using Dropcraft.Common.Deployment;
using Dropcraft.Deployment.NuGet;

namespace Dropcraft.Deployment.Core
{
    /// <summary>
    /// Describes the packages involved in the installation operation and the actions for these packages 
    /// </summary>
    public class InstallationPlan
    {
        /// <summary>
        /// Number of the packages to be updated
        /// </summary>
        public int UpdateCount { get; set; }

        /// <summary>
        /// Number of the packages to be installed
        /// </summary>
        public int InstallCount { get; set; }

        /// <summary>
        /// Final product representation
        /// </summary>
        public IPackageGraph TargetPackageGraph { get; set; }

        /// <summary>
        /// List of the actions for each package
        /// </summary>
        public ICollection<DeploymentAction> Actions { get; set; }

        /// <summary>
        /// List of the remaining packages
        /// </summary>
        public ICollection<PackageId> RemainingPackages { get; set; }

        /// <summary>
        /// Constructs an instance of <see cref="InstallationPlan"/>
        /// </summary>
        public InstallationPlan()
        {
            Actions = new List<DeploymentAction>();
            RemainingPackages = new List<PackageId>();
        }
    }

    /// <summary>
    /// Describes uninstallation actions
    /// </summary>
    public class UninstallationPlan
    {
        /// <summary>
        /// List of the uninstallations actions
        /// </summary>
        public ICollection<DeploymentAction> Actions { get; set; }
    }

    /// <summary>
    /// Generates installation/uninstallation plans for the product
    /// </summary>
    public interface IPackageDeployer
    {
        /// <summary>
        /// An instance of <see cref="IDeploymentStrategyProvider"/> to use
        /// </summary>
        IDeploymentStrategyProvider DeploymentStrategyProvider { get; set; }

        /// <summary>
        /// An instance of <see cref="INuGetEngine"/> to use
        /// </summary>
        INuGetEngine NuGetEngine { get; set; }

        /// <summary>
        /// Packages installation path
        /// </summary>
        string PackagesFolderPath { get; set; }

        /// <summary>
        /// Current deployment context. An instance of <see cref="DeploymentContext"/>
        /// </summary>
        DeploymentContext DeploymentContext { get; set; }

        /// <summary>
        /// Generates an installation plan
        /// </summary>
        /// <param name="productPackages">Current product</param>
        /// <param name="packages">Packages to install</param>
        /// <returns><see cref="InstallationPlan"/></returns>
        Task<InstallationPlan> PlanInstallation(IPackageGraph productPackages, ICollection<PackageId> packages);

        /// <summary>
        /// Generates an uninstallation plan
        /// </summary>
        /// <param name="productPackages">Current product</param>
        /// <param name="packages">Packages to uninstall</param>
        /// <param name="removeDependencies">Defines whether the package dependencies shall be removed as well. When false, only the requested packages will be deleted</param>
        /// <returns><see cref="UninstallationPlan"/></returns>
        Task<UninstallationPlan> PlanUninstallation(IPackageGraph productPackages, ICollection<PackageId> packages,
            bool removeDependencies);
    }
}