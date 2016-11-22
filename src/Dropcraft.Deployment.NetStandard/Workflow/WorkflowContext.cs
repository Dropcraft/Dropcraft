using System;
using System.Collections.Generic;
using Dropcraft.Common;
using NuGet.DependencyResolver;

namespace Dropcraft.Deployment.Workflow
{
    /// <summary>
    /// Contains input, output and temporary information for the deployment workflow execution
    /// </summary>
    public class WorkflowContext
    {
        /// <summary>
        /// New package to be installed/updated
        /// </summary>
        public List<PackageId> InputPackages { get; set; }

        /// <summary>
        /// Packages already installed registered with the product
        /// </summary>
        public List<PackageId> InputProductPackages { get; set; }

        /// <summary>
        /// List of the top level packages (no other packages depend on them)
        /// </summary>
        public List<PackageId> TopLevelProductPackages { get; set; }

        /// <summary>
        /// Expected list of registered packages after the deployment
        /// </summary>
        public List<PackageInfo> ResultingProductPackages { get; } = new List<PackageInfo>();

        /// <summary>
        /// Packages list to install
        /// </summary>
        public List<PackageInfo> PackagesForInstallation { get; } = new List<PackageInfo>();

        /// <summary>
        /// List with the IDs of the packages to update
        /// </summary>
        public List<string> PackagesAffectedByUpdate { get; } = new List<string>();

        /// <summary>
        /// Packages list to remove
        /// </summary>
        public List<PackageId> ProductPackagesForDeletion { get; } = new List<PackageId>();

        /// <summary>
        /// Temp cache
        /// </summary>
        public Queue<Tuple<GraphNode<RemoteResolveResult>, PackageInfo>> FlatteningCache { get; }

        public WorkflowContext(IEnumerable<PackageId> newPackages, IEnumerable<PackageId> productPackages, IEnumerable<PackageId> topLevelProductPackages)
        {
            InputPackages = new List<PackageId>(newPackages);
            InputProductPackages = new List<PackageId>(productPackages);
            TopLevelProductPackages = new List<PackageId>(topLevelProductPackages);
            FlatteningCache = new Queue<Tuple<GraphNode<RemoteResolveResult>, PackageInfo>>();
        }
    }
}