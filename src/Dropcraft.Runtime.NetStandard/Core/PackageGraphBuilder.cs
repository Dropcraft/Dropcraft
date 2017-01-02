using System.Collections.Generic;
using System.Linq;
using Dropcraft.Common;

namespace Dropcraft.Runtime.Core
{
    /// <summary>
    /// Builds dependency graphs using provided packages and information about the relations
    /// </summary>
    public class PackageGraphBuilder
    {
        private readonly Dictionary<PackageId, List<PackageId>> _packages = new Dictionary<PackageId, List<PackageId>>();

        /// <summary>
        /// Includes the provided information in the graph
        /// </summary>
        /// <param name="packageId">The package identifier.</param>
        /// <param name="dependencies">The package's dependencies.</param>
        /// <returns><see cref="PackageGraphBuilder"/></returns>
        public PackageGraphBuilder Append(PackageId packageId, IEnumerable<PackageId> dependencies)
        {
            if (!_packages.ContainsKey(packageId))
            {
                _packages.Add(packageId, new List<PackageId>(dependencies));
            }

            return this;
        }

        /// <summary>
        /// Builds an instance of PackageGraph using provided information.
        /// </summary>
        /// <returns><see cref="PackageGraph"/>.</returns>
        public PackageGraph Build()
        {
            var nodes = new List<PackageGraphNode>();

            foreach (var package in _packages)
            {
                var packageId = package.Key;

                var packageNode = nodes.FirstOrDefault(x=>x.Package.IsSamePackage(packageId));
                if (packageNode == null)
                {
                    packageNode = new PackageGraphNode(packageId);
                    nodes.Add(packageNode);
                }

                foreach (var dependencyId in package.Value)
                {
                    var depPackageNode = nodes.FirstOrDefault(x => x.Package.IsSamePackage(dependencyId)); 
                    if (depPackageNode == null)
                    {
                        depPackageNode = new PackageGraphNode(dependencyId);
                        nodes.Add(depPackageNode);
                    }

                    packageNode.AddDependency(depPackageNode);
                }
            }

            return new PackageGraph(nodes.Where(x => x.Dependents.Count == 0).ToList(), nodes.Count);
        }
    }
}