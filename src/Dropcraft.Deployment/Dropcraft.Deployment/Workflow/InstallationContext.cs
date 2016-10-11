using System;
using System.Collections.Generic;
using Dropcraft.Common;
using NuGet.DependencyResolver;

namespace Dropcraft.Deployment.Workflow
{
    public class InstallationContext
    {
        public List<PackageId> InputPackages { get; set; }

        public List<PackageId> ProductPackages { get; set; }

        public List<ActionablePackage> PackagesForInstallation { get; } = new List<ActionablePackage>();
        public List<PackageId> ProductPackagesForDeletion { get; } = new List<PackageId>();

        public Queue<Tuple<GraphNode<RemoteResolveResult>, ActionablePackage>> FlatteningCache { get; }

        public InstallationContext(IEnumerable<PackageId> newPackages, IEnumerable<PackageId> productPackages)
        {
            InputPackages = new List<PackageId>(newPackages);
            ProductPackages = new List<PackageId>(productPackages);
            FlatteningCache = new Queue<Tuple<GraphNode<RemoteResolveResult>, ActionablePackage>>();
        }
    }
}