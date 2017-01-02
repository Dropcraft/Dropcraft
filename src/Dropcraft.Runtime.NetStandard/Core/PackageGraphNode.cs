using System.Collections.Generic;
using Dropcraft.Common;

namespace Dropcraft.Runtime.Core
{
    /// <summary>
    /// Class PackageGraphNode.
    /// </summary>
    /// <seealso cref="Dropcraft.Common.IPackageGraphNode" />
    public class PackageGraphNode : IPackageGraphNode
    {
        private readonly List<PackageGraphNode> _dependencies = new List<PackageGraphNode>();
        private readonly List<PackageGraphNode> _dependents = new List<PackageGraphNode>();

        /// <summary>
        /// Package
        /// </summary>
        public PackageId Package { get; }

        /// <summary>
        /// Package dependencies
        /// </summary>
        /// <value><see cref="IPackageGraphNode"/></value>
        public IReadOnlyCollection<IPackageGraphNode> Dependencies => _dependencies.AsReadOnly();
        /// <summary>
        /// Package dependents
        /// </summary>
        /// <value><see cref="IPackageGraphNode"/></value>
        public IReadOnlyCollection<IPackageGraphNode> Dependents => _dependents.AsReadOnly();

        /// <summary>
        /// Initializes a new instance of the <see cref="PackageGraphNode"/> class.
        /// </summary>
        /// <param name="packageId">The package identifier.</param>
        public PackageGraphNode(PackageId packageId)
        {
            Package = packageId;
        }

        /// <summary>
        /// Adds a dependency for the node.
        /// </summary>
        /// <param name="dependency">New dependency.</param>
        public void AddDependency(PackageGraphNode dependency)
        {
            _dependencies.Add(dependency);
            dependency._dependents.Add(this);
        }
    }
}