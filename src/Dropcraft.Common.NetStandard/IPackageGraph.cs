using System.Collections.Generic;

namespace Dropcraft.Common
{
    /// <summary>
    /// Represents hierarchy of the packages and dependencies
    /// </summary>
    public interface IPackageGraph
    {
        /// <summary>
        /// Top level packages
        /// </summary>
        IReadOnlyCollection<IPackageGraphNode> Packages { get; }

        /// <summary>
        /// Number of the packages in the graph
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Flattens the graph returning a list with the dependent packages fist following the dependencies 
        /// </summary>
        /// <returns>Flattened graph</returns>
        ICollection<PackageId> FlattenMostDependentFirst();

        /// <summary>
        /// Flattens the graph returning a list with the dependencies first following the dependent packages
        /// </summary>
        /// <returns>Flattened graph</returns>
        ICollection<PackageId> FlattenLeastDependentFirst();

        /// <summary>
        /// Creates a subgraph including the provided packages and the packages they depend on
        /// </summary>
        /// <param name="packages">Packages to use for slicing</param>
        /// <param name="excludeNonExclusiveDependencies">When true, all dependencies shared with the packages outside of the subgraph will not be included</param>
        /// <returns>New graph which includes the requested packages and the dependencies</returns>
        IPackageGraph SliceWithDependencies(ICollection<PackageId> packages, bool excludeNonExclusiveDependencies);

        /// <summary>
        /// Creates a subgraph including the provided packages and the packages which depend on them
        /// </summary>
        /// <param name="packages">Packages to use for slicing</param>
        /// <returns>New graph which includes the requested packages and the dependents</returns>
        IPackageGraph SliceWithDependents(ICollection<PackageId> packages);

        /// <summary>
        /// Return nodes for the provided package IDs
        /// </summary>
        /// <param name="packages">Package IDs</param>
        /// <returns>Nodes from the graph</returns>
        IReadOnlyCollection<IPackageGraphNode> GetNodes(ICollection<PackageId> packages);
    }
}