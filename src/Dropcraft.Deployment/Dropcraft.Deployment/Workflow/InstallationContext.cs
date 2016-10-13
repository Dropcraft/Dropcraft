using System;
using System.Collections.Generic;
using Dropcraft.Common;
using NuGet.DependencyResolver;

namespace Dropcraft.Deployment.Workflow
{
    public class InstallationContext
    {
        public List<PackageId> InputPackages { get; set; }

        public List<PackageId> InputProductPackages { get; set; }

        public List<ActionablePackage> ResultingProductPackages { get; } = new List<ActionablePackage>();
        public List<ActionablePackage> PackagesForInstallation { get; } = new List<ActionablePackage>();
        public List<PackageId> ProductPackagesForDeletion { get; } = new List<PackageId>();

        public Queue<Tuple<GraphNode<RemoteResolveResult>, ActionablePackage>> FlatteningCache { get; }

        public InstallationContext(IEnumerable<PackageId> newPackages, IEnumerable<PackageId> productPackages)
        {
            InputPackages = new List<PackageId>(newPackages);
            InputProductPackages = new List<PackageId>(productPackages);
            FlatteningCache = new Queue<Tuple<GraphNode<RemoteResolveResult>, ActionablePackage>>();
        }
    }
}