using System.Collections.Generic;

namespace Dropcraft.Common.Configuration
{
    /// <summary>
    /// Represents an individual package with dependencies
    /// </summary>
    public interface IPackageGraphNode
    {
        /// <summary>
        /// Package
        /// </summary>
        PackageId Package { get; }

        /// <summary>
        /// Package dependencies
        /// </summary>
        IReadOnlyCollection<IPackageGraphNode> Dependencies { get; }

        /// <summary>
        /// Package dependents
        /// </summary>
        IReadOnlyCollection<IPackageGraphNode> Dependents { get; }
    }
}