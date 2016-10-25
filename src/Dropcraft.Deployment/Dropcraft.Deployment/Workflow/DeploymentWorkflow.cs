using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dropcraft.Common;
using Dropcraft.Common.Configuration;
using Dropcraft.Common.Logging;
using Dropcraft.Deployment.NuGet;
using NuGet.DependencyResolver;
using NuGet.LibraryModel;
using NuGet.Versioning;

namespace Dropcraft.Deployment.Workflow
{
    /// <summary>
    /// Low level deployment API 
    /// </summary>
    public class DeploymentWorkflow
    {
        private readonly NuGetEngine _nuGetEngine;
        private static readonly ILog Logger = LogProvider.For<DeploymentWorkflow>();

        public DeploymentWorkflow(NuGetEngine nuGetEngine)
        {
            _nuGetEngine = nuGetEngine;
        }

        /// <summary>
        /// Ensures that all packages are versioned and versions are valid
        /// </summary>
        /// <returns>Task</returns>
        public async Task EnsureAllPackagesAreVersioned(WorkflowContext context)
        {
            await OnEnsureAllPackagesAreVersioned(context);
        }

        protected virtual async Task OnEnsureAllPackagesAreVersioned(WorkflowContext context)
        {
            var tasks = new List<Task<PackageId>>();

            foreach (var packageId in context.InputPackages)
            {
                if (string.IsNullOrWhiteSpace(packageId.Id))
                    throw LogException(new ArgumentException("Package Id cannot be empty"));

                tasks.Add(ResolvePackageVersion(packageId));
            }

            context.InputPackages = (await Task.WhenAll(tasks)).ToList();
        }

        /// <summary>
        /// Resolves dependencies for all packages
        /// </summary>
        /// <returns>Task</returns>
        public async Task ResolvePackages(WorkflowContext context)
        {
            await OnResolvePackages(context);
        }

        protected virtual async Task OnResolvePackages(WorkflowContext context)
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
        /// Downloads and unpacks packages to the destination folder
        /// </summary>
        /// <param name="context">Installation context</param>
        /// <param name="path">Destination path</param>
        /// <returns>Task</returns>
        public void DownloadPackages(WorkflowContext context, string path)
        {
            OnDownloadPackages(context, path);
        }

        protected virtual void OnDownloadPackages(WorkflowContext context, string path)
        {
            foreach (var package in context.PackagesForInstallation)
            {
                _nuGetEngine.InstallPackage(package.Match, path).GetAwaiter().GetResult();
                package.PackagePath = _nuGetEngine.GetPackageTargetPath(package.Match.Library.Name,
                    package.Match.Library.Version, path);
            }
        }

        /// <summary>
        /// Deletes provided packages
        /// </summary>
        /// <param name="fileTransaction">Transaction to track file system changes</param>
        /// <param name="packages">Packages to delete</param>
        /// <param name="productConfig">Current product configuration</param>
        public void DeletePackages(FileTransaction fileTransaction, IEnumerable<PackageId> packages, IProductConfigurationProvider productConfig)
        {
            OnDeletePackages(fileTransaction, packages, productConfig);
        }

        protected virtual void OnDeletePackages(FileTransaction fileTransaction, IEnumerable<PackageId> packages,
            IProductConfigurationProvider productConfig)
        {
            foreach (var packageId in packages)
            {
                var packageConfig = productConfig.GetPackageConfiguration(packageId);
                var files = packageConfig.GetInstalledFiles(true);
                foreach (var file in files)
                {
                    fileTransaction.DeleteFile(file);
                }

                productConfig.RemovePackageConfiguration(packageId);
            }
        }

        /// <summary>
        /// Installs provided packages
        /// </summary>
        /// <param name="fileTransaction">File transaction to use</param>
        /// <param name="packages">Packages to install</param>
        /// <param name="productConfigProvider">Product configuration</param>
        /// <param name="packageConfigProvider">Packages configuration</param>
        /// <param name="deploymentStartegy">Deployment strategy to use</param>
        public void InstallPackages(FileTransaction fileTransaction, IEnumerable<PackageInfo> packages,
            IProductConfigurationProvider productConfigProvider,
            IPackageConfigurationProvider packageConfigProvider,
            IDeploymentStartegyProvider deploymentStartegy)
        {
            OnInstallPackages(fileTransaction, packages, productConfigProvider, packageConfigProvider,
                deploymentStartegy);
        }

        protected virtual void OnInstallPackages(FileTransaction fileTransaction, IEnumerable<PackageInfo> packages,
            IProductConfigurationProvider productConfigProvider,
            IPackageConfigurationProvider packageConfigProvider,
            IDeploymentStartegyProvider deploymentStartegy)
        {
            foreach (var package in packages)
            {
                var files = deploymentStartegy.GetPackageFiles(package.Id, package.PackagePath);
                foreach (var file in files)
                {
                    fileTransaction.InstallFile(file);
                }

                productConfigProvider.SetPackageConfiguration(package.Id,
                    packageConfigProvider.GetPackageConfiguration(package.Id, package.PackagePath));
            }
        }

        protected void PrepareInstallationAndDeletionLists(WorkflowContext context)
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

        protected void FlattenPackageNode(GraphNode<RemoteResolveResult> node, PackageInfo parent, WorkflowContext context)
        {
            if (node.Key.TypeConstraint != LibraryDependencyTarget.Package &&
                node.Key.TypeConstraint != LibraryDependencyTarget.PackageProjectExternal)
            {
                var exception = new ArgumentException($"Package {node.Key.Name} cannot be resolved from the sources");
                throw LogException(exception);
            }

            var package = new PackageInfo
            {
                Id = new PackageId(node.Item.Key.Name, node.Item.Key.Version.ToNormalizedString(),
                        node.Item.Key.Version.IsPrerelease),
                Match = node.Item.Data.Match
            };

            context.ResultingProductPackages.Add(package);
            parent?.Dependencies.Add(package);

            foreach (var innerNode in node.InnerNodes)
            {
                context.FlatteningCache.Enqueue(new Tuple<GraphNode<RemoteResolveResult>, PackageInfo>(innerNode, package)); 
            }
        }

        protected void MergeWithProductPackages(WorkflowContext context)
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