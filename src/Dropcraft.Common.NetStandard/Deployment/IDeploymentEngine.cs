using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dropcraft.Common.Deployment
{
    /// <summary>
    /// High level API for installing/uninstalling packages
    /// </summary>
    public interface IDeploymentEngine
    {
        /// <summary>
        /// Current deployment context
        /// </summary>
        DeploymentContext DeploymentContext { get; }

        /// <summary>
        /// Installs provided packages
        /// </summary>
        /// <param name="packages">Packages to install</param>
        /// <param name="options">Additional installation options</param>
        /// <returns>Task</returns>
        Task InstallPackages(ICollection<PackageId> packages, InstallationOptions options);

        /// <summary>
        /// Uninstalls provided packages
        /// </summary>
        /// <param name="packages">Packages to uninstall</param>
        /// <param name="options">Additional uninstallation options</param>
        /// <returns>Task</returns>
        Task UninstallPackages(ICollection<PackageId> packages, UninstallationOptions options);
    }
}
