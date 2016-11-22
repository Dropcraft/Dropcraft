using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dropcraft.Common;
using Dropcraft.Common.Configuration;
using Dropcraft.Common.Handler;
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
        public NuGetEngine NuGetEngine { get; protected set; }
        public DeploymentContext DeploymentContext { get; protected set; }
        public WorkflowContext WorkflowContext { get; protected set; }

        private static readonly ILog Logger = LogProvider.For<DeploymentWorkflow>();

        public DeploymentWorkflow(DeploymentContext deploymentContext, WorkflowContext workflowContext, NuGetEngine nuGetEngine)
        {
            DeploymentContext = deploymentContext;
            WorkflowContext = workflowContext;
            NuGetEngine = nuGetEngine;
        }

        /// <summary>
        /// Ensures that all packages are versioned and versions are valid
        /// </summary>
        /// <returns>Task</returns>
        public async Task EnsureAllPackagesAreVersioned()
        {
            await OnEnsureAllPackagesAreVersioned();
        }

        protected virtual async Task OnEnsureAllPackagesAreVersioned()
        {
            var tasks = new List<Task<PackageId>>();

            foreach (var packageId in WorkflowContext.InputPackages)
            {
                if (string.IsNullOrWhiteSpace(packageId.Id))
                    throw LogException(new ArgumentException("Package Id cannot be empty"));

                tasks.Add(ResolvePackageVersion(packageId));
            }

            WorkflowContext.InputPackages = (await Task.WhenAll(tasks)).ToList();
        }

        /// <summary>
        /// Resolves dependencies for all packages
        /// </summary>
        /// <returns>Task</returns>
        public async Task ResolvePackages()
        {
            await OnResolvePackages();
        }

        protected virtual async Task OnResolvePackages()
        {
            MergeWithProductPackages();

            var resolvedPackages = await NuGetEngine.ResolvePackages(WorkflowContext.InputPackages);
            NuGetEngine.AnalysePackages(resolvedPackages);

            foreach (var innerNode in resolvedPackages.InnerNodes)
            {
                FlattenPackageNode(innerNode, null);
            }

            while (WorkflowContext.FlatteningCache.Any())
            {
                var nextNode = WorkflowContext.FlatteningCache.Dequeue();
                FlattenPackageNode(nextNode.Item1, nextNode.Item2);
            }

            PrepareInstallationAndDeletionLists();
        }

        /// <summary>
        /// Downloads and unpacks packages to the destination folder
        /// </summary>
        /// <param name="path">Destination path</param>
        /// <returns>Task</returns>
        public void DownloadPackages(string path)
        {
            OnDownloadPackages(path);
        }

        protected virtual void OnDownloadPackages(string path)
        {
            foreach (var package in WorkflowContext.PackagesForInstallation)
            {
                NuGetEngine.InstallPackage(package.Match, path).GetAwaiter().GetResult();
                package.PackagePath = NuGetEngine.GetPackageTargetPath(package.Match.Library.Name,
                    package.Match.Library.Version, path);

                RaiseDeploymentEvent(new AfterPackageDownloadedEvent {PackagePath = package.PackagePath}, package.Id);
            }
        }

        private bool IsUpdateInProgressForPackage(PackageId id)
        {
            return WorkflowContext.PackagesAffectedByUpdate.Any(
                    x => string.Equals(x, id.Id, StringComparison.CurrentCultureIgnoreCase));
        }

        private void RaiseDeploymentEvent(PackageDeploymentEvent e, PackageId id)
        {
            e.Id = id;
            e.IsUpdateInProgress = IsUpdateInProgressForPackage(id);
            DeploymentContext.RaiseDeploymentEvent(e);
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
                var files = productConfig.GetInstalledFiles(packageId, true);

                var deleteEvent = new BeforePackageUninstalledEvent();
                deleteEvent.FilesToDelete.AddRange(files);
                RaiseDeploymentEvent(deleteEvent, packageId);

                foreach (var file in deleteEvent.FilesToDelete)
                {
                    fileTransaction.DeleteFile(file);
                }

                productConfig.RemovePackageConfiguration(packageId);

                RaiseDeploymentEvent(new AfterPackageUninstalledEvent(), packageId);
                Logger.Info($"Package {packageId} uninstalled");
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
                var files = deploymentStartegy.GetPackageFiles(package.Id, package.PackagePath).ToList();
                var installedFiles = new List<string>();

                var e = new BeforePackageInstalledEvent();
                e.FilesToInstall.AddRange(files);
                RaiseDeploymentEvent(e, package.Id);

                foreach (var file in e.FilesToInstall)
                {
                    if (file.Action == FileAction.Copy)
                    {
                        fileTransaction.InstallFile(file);
                        installedFiles.Add(file.TargetFileName);
                    }
                    else if (file.Action == FileAction.Delete)
                    {
                        fileTransaction.DeleteFile(file.TargetFileName);
                    }
                }

                var cfg = packageConfigProvider.GetPackageConfiguration(package.Id, package.PackagePath);
                productConfigProvider.SetPackageConfiguration(cfg, installedFiles, package.Dependencies.Select(x => x.Id.ToString()));

                RaiseDeploymentEvent(new AfterPackageInstalledEvent(), package.Id);
                Logger.Info($"Package {package.Id} installed");
            }
        }

        protected void PrepareInstallationAndDeletionLists()
        {
            foreach (var package in WorkflowContext.ResultingProductPackages)
            {
                var packageMatch = WorkflowContext.InputProductPackages.FirstOrDefault(x => x.Id == package.Id.Id);

                if (packageMatch != null)
                {
                    if (packageMatch.Version != package.Id.Version)
                    {
                        WorkflowContext.ProductPackagesForDeletion.Add(packageMatch);
                        WorkflowContext.PackagesForInstallation.Add(package);
                        WorkflowContext.PackagesAffectedByUpdate.Add(package.Id.Id);
                    }
                }
                else
                {
                    WorkflowContext.PackagesForInstallation.Add(package);
                }
            }

            WorkflowContext.ResultingProductPackages.Reverse();
            WorkflowContext.PackagesForInstallation.Reverse();
        }

        protected void FlattenPackageNode(GraphNode<RemoteResolveResult> node, PackageInfo parent)
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

            WorkflowContext.ResultingProductPackages.Add(package);
            parent?.Dependencies.Add(package);

            foreach (var innerNode in node.InnerNodes)
            {
                WorkflowContext.FlatteningCache.Enqueue(new Tuple<GraphNode<RemoteResolveResult>, PackageInfo>(innerNode, package)); 
            }
        }

        protected void MergeWithProductPackages()
        {
            var listForMerge = new List<PackageId>();

            foreach (var productPackage in WorkflowContext.TopLevelProductPackages)
            {
                var addPackage = true;
                foreach (var newPackage in WorkflowContext.InputPackages)
                {
                    if (string.Equals(newPackage.Id, productPackage.Id, StringComparison.CurrentCultureIgnoreCase))
                        addPackage = false;
                }

                if (addPackage)
                    listForMerge.Add(productPackage);
            }

            WorkflowContext.InputPackages.AddRange(listForMerge);
        }

        protected async Task<PackageId> ResolvePackageVersion(PackageId packageId)
        {
            PackageId package = null;
            if (!string.IsNullOrWhiteSpace(packageId.Version))
            {
                var resolvedVer = VersionRange.Parse(packageId.Version);
                if (resolvedVer == null)
                {
                    throw LogException(new ArgumentException($"Provided package version is incorrect: {packageId.Version}"));
                }

                package = packageId;
            }
            else
            {
                var version = await NuGetEngine.ResolveNuGetVersion(packageId);
                if (version != null)
                {
                    package = new PackageId(packageId.Id, version, packageId.AllowPrereleaseVersions);
                }
            }

            if (package == null)
            {
                throw LogException(new ArgumentException($"Version for package {packageId.Id} cannot be resolved"));
            }

            Logger.Trace($"Package {packageId.Id} resolved as {package.Id}/{package.Version}");
            return package;
        }

        protected Exception LogException(Exception exception)
        {
            Logger.Trace(exception.Message);
            return exception;
        }
    }
}