using System.Collections.Generic;
using System.Threading.Tasks;
using Dropcraft.Common;
using Dropcraft.Deployment.NuGet;

namespace Dropcraft.Deployment.Core
{
    /// <summary>
    /// Generates a combined list of the installed packages and the new packages with the correct versions
    /// </summary>
    public interface IPackageDiscoverer
    {
        /// <summary>
        /// NuGet engine to use
        /// </summary>
        /// <value><see cref="INuGetEngine"/></value>
        INuGetEngine NuGetEngine { get; set; }

        /// <summary>
        /// Based on the provided product configuration and the new packages, returns a joint list of the packages  
        /// </summary>
        /// <param name="packageGraph">Existing product configuration</param>
        /// <param name="newPackages">New packages to install. Some packages may be defined only by name, without version or using NuGet versioning notation</param>
        /// <returns>Combined list of the versioned packages</returns>
        Task<ICollection<PackageId>> Discover(IPackageGraph packageGraph, ICollection<PackageId> newPackages);
    }
}