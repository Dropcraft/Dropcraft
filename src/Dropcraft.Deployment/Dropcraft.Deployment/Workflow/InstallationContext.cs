using System;
using System.Collections.Generic;
using Dropcraft.Common;
using NuGet.DependencyResolver;

namespace Dropcraft.Deployment.Workflow
{
    public class InstallationContext
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
        /// Expected list of registered packages after the deployment
        /// </summary>
        public List<ActionablePackage> ResultingProductPackages { get; } = new List<ActionablePackage>();

        /// <summary>
        /// Packages list to install
        /// </summary>
        public List<ActionablePackage> PackagesForInstallation { get; } = new List<ActionablePackage>();

        /// <summary>
        /// Packages list to remove
        /// </summary>
        public List<PackageId> ProductPackagesForDeletion { get; } = new List<PackageId>();

        /// <summary>
        /// Temp cache
        /// </summary>
        public Queue<Tuple<GraphNode<RemoteResolveResult>, ActionablePackage>> FlatteningCache { get; }

        public InstallationContext(IEnumerable<PackageId> newPackages, IEnumerable<PackageId> productPackages)
        {
            InputPackages = new List<PackageId>(newPackages);
            InputProductPackages = new List<PackageId>(productPackages);
            FlatteningCache = new Queue<Tuple<GraphNode<RemoteResolveResult>, ActionablePackage>>();
        }
    }
}