using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dropcraft.Common;
using Dropcraft.Common.Logging;
using Dropcraft.Deployment.NuGet;
using NuGet.DependencyResolver;
using NuGet.LibraryModel;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Versioning;

namespace Dropcraft.Deployment.Workflow
{
    public class InstallationWorkflow
    {
        private readonly NuGetEngine _nuGetEngine;
        private static readonly ILog Logger = LogProvider.For<DeploymentEngine>();

        public InstallationContext Context { get; set; }

        public InstallationWorkflow(InstallationContext context, NuGetEngine nuGetEngine)
        {
            Context = context;
            _nuGetEngine = nuGetEngine;
        }

        /// <summary>
        /// Ensures that all packages are versioned and versions are valid
        /// </summary>
        /// <returns>Async Task</returns>
        public async Task EnsureAllPackagesAreVersioned()
        {
            var tasks = new List<Task<PackageId>>();

            foreach (var packageId in Context.InputPackages)
            {
                if (string.IsNullOrWhiteSpace(packageId.Id))
                    throw new ArgumentException("Package Id cannot be empty");

                tasks.Add(ResolvePackageVersion(packageId));
            }

            Context.InputPackages = (await Task.WhenAll(tasks)).ToList();
        }

        /// <summary>
        /// Resolves dependencies for all packages
        /// </summary>
        /// <returns>Async Task</returns>
        public async Task ResolvePackages()
        {
            MergeWithProductPackages();

            var resolvedPackages = await _nuGetEngine.ResolvePackages(Context.InputPackages);
            _nuGetEngine.AnalysePackages(resolvedPackages);

            foreach (var innerNode in resolvedPackages.InnerNodes)
            {
                FlattenPackageNode(innerNode, null, Context);
            }

            while (Context.FlatteningCache.Any())
            {
                var nextNode = Context.FlatteningCache.Dequeue();
                FlattenPackageNode(nextNode.Item1, nextNode.Item2, Context);
            }

            Context.PackagesForInstallation.Reverse();
            PrepareListForDeletion();
        }

        /// <summary>
        /// Installs packages to the destination folder
        /// </summary>
        /// <param name="path">Destination path</param>
        /// <returns>Async Task</returns>
        public void InstallPackages(string path)
        {
            foreach (var package in Context.PackagesForInstallation)
            {
                _nuGetEngine.InstallPackage(package.Match, path).GetAwaiter().GetResult();
            }
        }

        protected void PrepareListForDeletion()
        {
            foreach (var package in Context.ProductPackages)
            {
                if (Context.PackagesForInstallation.Any(
                        x => x.Id.Id == package.Id && x.Id.VersionRange != package.VersionRange))
                {
                    Context.ProductPackagesForDeletion.Add(package);
                }
            }
        }

        protected void FlattenPackageNode(GraphNode<RemoteResolveResult> node, ActionablePackage parent, InstallationContext context)
        {
            if (node.Key.TypeConstraint != LibraryDependencyTarget.Package &&
                node.Key.TypeConstraint != LibraryDependencyTarget.PackageProjectExternal)
            {
                var exception = new ArgumentException($"Package {node.Key.Name} cannot be resolved from the sources");
                throw exception;
            }

            var package = new ActionablePackage
            {
                Id = new PackageId(node.Item.Key.Name, node.Item.Key.Version.ToNormalizedString(),
                        node.Item.Key.Version.IsPrerelease),
                Match = node.Item.Data.Match
            };

            context.PackagesForInstallation.Add(package);
            parent?.Dependencies.Add(package);

            foreach (var innerNode in node.InnerNodes)
            {
                context.FlatteningCache.Enqueue(new Tuple<GraphNode<RemoteResolveResult>, ActionablePackage>(innerNode, package)); 
            }
        }

        protected void MergeWithProductPackages()
        {
            var listForMerge = new List<PackageId>();

            foreach (var productPackage in Context.ProductPackages)
            {
                var addPackage = true;
                foreach (var newPackage in Context.InputPackages)
                {
                    if (newPackage.Id == productPackage.Id)
                        addPackage = false;
                }

                if (addPackage)
                    listForMerge.Add(productPackage);
            }

            Context.InputPackages.AddRange(listForMerge);
        }

        protected async Task<PackageId> ResolvePackageVersion(PackageId packageId)
        {
            PackageId package = null;
            if (!string.IsNullOrWhiteSpace(packageId.VersionRange))
            {
                var resolvedVer = VersionRange.Parse(packageId.VersionRange);
                if (resolvedVer == null)
                {
                    throw LogException(new ArgumentException($"Provided package version is incorrect: {packageId.VersionRange}"));
                }

                package = packageId;
            }
            else
            {
                var version = await _nuGetEngine.ResolveNuGetVersion(packageId);
                if (version != null)
                {
                    package = new PackageId(packageId.Id, version, packageId.AllowPrereleaseVersions);
                }
            }

            if (package == null)
            {
                throw LogException(new ArgumentException($"Version for package {packageId.Id} cannot be resolved"));
            }

            Logger.Trace($"Package {packageId.Id} resolved as {packageId.Id},{packageId.VersionRange}");
            return package;
        }

        protected Exception LogException(Exception exception)
        {
            Logger.Trace(exception.Message);
            return exception;
        }
    }
}