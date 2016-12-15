using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dropcraft.Common
{
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
        /// <param name="allowDowngrades">Instructs to allow packages downgrades.</param>
        /// <param name="updatePackages">When true, packages will be always updated from the remote source, even if they can be resolved from the installed path.</param>
        /// <returns>Task</returns>
        Task InstallPackages(ICollection<PackageId> packages, bool allowDowngrades, bool updatePackages);

        /// <summary>
        /// Uninstalls provided packages
        /// </summary>
        /// <param name="packages">Packages to uninstall</param>
        /// <param name="removeDependencies">Remove dependent packages if they are not referenced elsewhere</param>
        /// <returns>Task</returns>
        Task UninstallPackages(ICollection<PackageId> packages, bool removeDependencies);
    }
}
