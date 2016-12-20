using System.Collections.Generic;
using Dropcraft.Common;

namespace Dropcraft.Runtime.Core
{
    public class PackageGraphNode : IPackageGraphNode
    {
        private readonly List<PackageGraphNode> _dependencies = new List<PackageGraphNode>();
        private readonly List<PackageGraphNode> _dependents = new List<PackageGraphNode>();

        public PackageId Package { get; }

        public IReadOnlyCollection<IPackageGraphNode> Dependencies => _dependencies.AsReadOnly();
        public IReadOnlyCollection<IPackageGraphNode> Dependents => _dependents.AsReadOnly();

        public PackageGraphNode(PackageId packageId)
        {
            Package = packageId;
        }

        public void AddDependency(PackageGraphNode dependency)
        {
            _dependencies.Add(dependency);
            dependency._dependents.Add(this);
        }
    }
}