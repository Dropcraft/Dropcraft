using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dropcraft.Common;
using Dropcraft.Deployment.NuGet;

namespace Dropcraft.Deployment.Core
{
    /// <summary>
    /// Class PackageDiscoverer.
    /// </summary>
    /// <seealso cref="Dropcraft.Deployment.Core.IPackageDiscoverer" />
    public class PackageDiscoverer : IPackageDiscoverer
    {
        /// <summary>
        /// NuGet engine to use
        /// </summary>
        /// <value><see cref="INuGetEngine" /></value>
        public INuGetEngine NuGetEngine { get; set; }

        /// <summary>
        /// Based on the provided product configuration and the new packages, returns a joint list of the packages
        /// </summary>
        /// <param name="packageGraph">Existing product configuration</param>
        /// <param name="newPackages">New packages to install. Some packages may be defined only by name, without version or using NuGet versioning notation</param>
        /// <returns>Combined list of the versioned packages</returns>
        /// <exception cref="System.ArgumentException">Package Id cannot be empty</exception>
        public async Task<ICollection<PackageId>> Discover(IPackageGraph packageGraph, ICollection<PackageId> newPackages)
        {
            var tasks = new List<Task<PackageId>>();

            foreach (var packageId in newPackages)
            {
                if (string.IsNullOrWhiteSpace(packageId.Id))
                    throw new ArgumentException("Package Id cannot be empty");

                tasks.Add(NuGetEngine.ResolvePackageVersion(packageId));
            }

            var versionedPackages = (await Task.WhenAll(tasks)).ToList();

            var productPackages = packageGraph.Packages.Select(x => x.Package);
            foreach (var productPackage in productPackages)
            {
                var addPackage = !versionedPackages.Any(x => string.Equals(x.Id, productPackage.Id,
                    StringComparison.CurrentCultureIgnoreCase));

                if (addPackage)
                    versionedPackages.Add(productPackage);
            }

            return versionedPackages;
        }
    }
}