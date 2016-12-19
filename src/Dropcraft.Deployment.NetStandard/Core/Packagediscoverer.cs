using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dropcraft.Common;
using Dropcraft.Common.Package;
using Dropcraft.Deployment.NuGet;

namespace Dropcraft.Deployment.Core
{
    public class PackageDiscoverer : IPackageDiscoverer
    {
        public INuGetEngine NuGetEngine { get; set; }

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