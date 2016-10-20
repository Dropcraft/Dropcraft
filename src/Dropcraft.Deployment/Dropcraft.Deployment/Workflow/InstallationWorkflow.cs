using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Dropcraft.Common;
using Dropcraft.Common.Logging;
using Dropcraft.Deployment.NuGet;
using NuGet.DependencyResolver;
using NuGet.LibraryModel;
using NuGet.Versioning;

namespace Dropcraft.Deployment.Workflow
{
    public class InstallationWorkflow
    {
        private readonly NuGetEngine _nuGetEngine;
        private static readonly ILog Logger = LogProvider.For<DeploymentEngine>();

        public InstallationWorkflow(NuGetEngine nuGetEngine)
        {
            _nuGetEngine = nuGetEngine;
        }

        /// <summary>
        /// Ensures that all packages are versioned and versions are valid
        /// </summary>
        /// <returns>Async Task</returns>
        public async Task EnsureAllPackagesAreVersioned(InstallationContext context)
        {
            var tasks = new List<Task<PackageId>>();

            foreach (var packageId in context.InputPackages)
            {
                if (string.IsNullOrWhiteSpace(packageId.Id))
                    throw new ArgumentException("Package Id cannot be empty");

                tasks.Add(ResolvePackageVersion(packageId));
            }

            context.InputPackages = (await Task.WhenAll(tasks)).ToList();
        }

        /// <summary>
        /// Resolves dependencies for all packages
        /// </summary>
        /// <returns>Async Task</returns>
        public async Task ResolvePackages(InstallationContext context)
        {
            MergeWithProductPackages(context);

            var resolvedPackages = await _nuGetEngine.ResolvePackages(context.InputPackages);
            _nuGetEngine.AnalysePackages(resolvedPackages);

            foreach (var innerNode in resolvedPackages.InnerNodes)
            {
                FlattenPackageNode(innerNode, null, context);
            }

            while (context.FlatteningCache.Any())
            {
                var nextNode = context.FlatteningCache.Dequeue();
                FlattenPackageNode(nextNode.Item1, nextNode.Item2, context);
            }

            PrepareInstallationAndDeletionLists(context);
        }

        /// <summary>
        /// Download and unpack packages to the destination folder
        /// </summary>
        /// <param name="context">Installation context</param>
        /// <param name="path">Destination path</param>
        /// <returns>Async Task</returns>
        public void DownloadPackages(InstallationContext context, string path)
        {
            foreach (var package in context.PackagesForInstallation)
            {
                _nuGetEngine.InstallPackage(package.Match, path).GetAwaiter().GetResult();
                package.TargetPath = _nuGetEngine.GetPackageTargetPath(package.Match.Library.Name,
                    package.Match.Library.Version, path);
            }
        }

        protected void PrepareInstallationAndDeletionLists(InstallationContext context)
        {
            foreach (var package in context.ResultingProductPackages)
            {
                var packageMatch = context.InputProductPackages.FirstOrDefault(x => x.Id == package.Id.Id);

                if (packageMatch != null)
                {
                    if (packageMatch.VersionRange != package.Id.VersionRange)
                    {
                        context.ProductPackagesForDeletion.Add(packageMatch);
                        context.PackagesForInstallation.Add(package);
                    }
                }
                else
                {
                    context.PackagesForInstallation.Add(package);
                }
            }

            context.ResultingProductPackages.Reverse();
            context.PackagesForInstallation.Reverse();
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

            context.ResultingProductPackages.Add(package);
            parent?.Dependencies.Add(package);

            foreach (var innerNode in node.InnerNodes)
            {
                context.FlatteningCache.Enqueue(new Tuple<GraphNode<RemoteResolveResult>, ActionablePackage>(innerNode, package)); 
            }
        }

        protected void MergeWithProductPackages(InstallationContext context)
        {
            var listForMerge = new List<PackageId>();

            foreach (var productPackage in context.InputProductPackages)
            {
                var addPackage = true;
                foreach (var newPackage in context.InputPackages)
                {
                    if (newPackage.Id == productPackage.Id)
                        addPackage = false;
                }

                if (addPackage)
                    listForMerge.Add(productPackage);
            }

            context.InputPackages.AddRange(listForMerge);
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