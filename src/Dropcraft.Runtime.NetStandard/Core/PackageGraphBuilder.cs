using System.Collections.Generic;
using System.Linq;
using Dropcraft.Common;

namespace Dropcraft.Runtime.Core
{
    public class PackageGraphBuilder
    {
        private readonly Dictionary<PackageId, List<PackageId>> _packages = new Dictionary<PackageId, List<PackageId>>();

        public PackageGraphBuilder Append(PackageId packageId, IEnumerable<PackageId> dependencies)
        {
            if (!_packages.ContainsKey(packageId))
            {
                _packages.Add(packageId, new List<PackageId>(dependencies));
            }

            return this;
        }

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