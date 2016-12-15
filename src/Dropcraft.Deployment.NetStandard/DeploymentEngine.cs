using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dropcraft.Common;
using Dropcraft.Common.Configuration;
using Dropcraft.Common.Events;
using Dropcraft.Common.Logging;
using Dropcraft.Deployment.NuGet;
using Dropcraft.Deployment.Workflow;
using Dropcraft.Runtime.Configuration;
using NuGet.DependencyResolver;
using NuGet.LibraryModel;

namespace Dropcraft.Deployment
{
    /// <summary>
    /// Deployment engine responsible for the packages installation/uninstallation
    /// </summary>
    public class DeploymentEngine : IDeploymentEngine
    {
        private static readonly ILog Logger = LogProvider.For<DeploymentEngine>();
        private static readonly Task SuccessTask = Task.FromResult<object>(null);

        protected NuGetEngine NuGetEngine { get; }
        protected IDeploymentStartegyProvider DeploymentStartegyProvider { get; }
        protected string PackagesFolderPath { get; }
        protected bool DontCachePackages { get; }

        /// <summary>
        /// Assigned deployment context
        /// </summary>
        public DeploymentContext DeploymentContext { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="deploymentContext">Deployment context to use</param>
        /// <param name="deploymentStartegyProvider">Deployment strategy to use</param>
        /// <param name="packagesFolderPath">Path to cache packages</param>
        /// <param name="remotePackagesSources">Package sources</param>
        public DeploymentEngine(DeploymentContext deploymentContext,
            IDeploymentStartegyProvider deploymentStartegyProvider, string packagesFolderPath,
            List<string> remotePackagesSources)
        {
            DeploymentContext = deploymentContext;
            DeploymentStartegyProvider = deploymentStartegyProvider;

            if (string.IsNullOrWhiteSpace(packagesFolderPath))
            {
                PackagesFolderPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                DontCachePackages = true;
            }
            else
            {
                PackagesFolderPath = packagesFolderPath;
            }


            NuGetEngine = new NuGetEngine(deploymentContext, PackagesFolderPath, remotePackagesSources);
        }

        /// <summary>
        /// Installs provided packages
        /// </summary>
        /// <param name="packages">Packages to install</param>
        /// <param name="allowDowngrades">Instructs to allow packages downgrades</param>
        /// <param name="updatePackages">Instructs to always try to update packages from the remote sources</param>
        /// <returns>Task</returns>
        public async Task InstallPackages(ICollection<PackageId> packages, bool allowDowngrades, bool updatePackages)
        {
            packages = await OnInstallingPackages(packages, allowDowngrades, updatePackages);

            var resolvedPackages = new Dictionary<PackageId, DeploymentPackageInfo>();
            var resolvedPackagesGraph = await OnResolvingPackages(packages, resolvedPackages);

            DeploymentContext.RaiseDeploymentEvent(new BeforeMaintenanceEvent());

            using (var transaction = new DeploymentTransaction())
            {
                var actions = OnPreparingInstallationActions(resolvedPackagesGraph, resolvedPackages);

                foreach (var deploymentAction in actions)
                {
                    deploymentAction.Execute(transaction);
                }

                if (OnReconfiguringPackages(transaction, resolvedPackagesGraph))
                    transaction.Commit();
            }

            DeploymentContext.RaiseDeploymentEvent(new AfterMaintenanceEvent());

            OnFinilizingPackagesInstallation();
        }

        protected virtual bool OnReconfiguringPackages(DeploymentTransaction transaction, IPackageGraph resolvedPackagesGraph)
        {
            var packages = new List<IPackageConfiguration>();
            var packageFiles = new Dictionary<PackageId, IEnumerable<string>>();

            foreach (var packageInfo in transaction.InstalledPackages)
            {
                packages.Add(packageInfo.Configuration);
                packageFiles.Add(packageInfo.Configuration.Id, packageInfo.Files);
            }

            DeploymentContext.ProductConfigurationProvider.Reconfigure(packages, resolvedPackagesGraph, packageFiles);
            DeploymentContext.ProductConfigurationProvider.Save();
            return true;
        }

        protected virtual async Task<ICollection<PackageId>> OnInstallingPackages(ICollection<PackageId> packages,
            bool allowDowngrades, bool updatePackages)
        {
            Logger.Info("Installing packages...");
            var productPackages = DeploymentContext.ProductConfigurationProvider.GetPackages();
            Logger.Info(
                $"{productPackages.Count} product package(s) discovered and {packages.Count} new package(s) requested");

            NuGetEngine.AllowDowngrades = allowDowngrades;
            NuGetEngine.UpdatePackages = updatePackages;

            var tasks = new List<Task<PackageId>>();

            foreach (var packageId in packages)
            {
                if (string.IsNullOrWhiteSpace(packageId.Id))
                    throw LogException(new ArgumentException("Package Id cannot be empty"));

                tasks.Add(NuGetEngine.ResolvePackageVersion(packageId));
            }

            var results = await Task.WhenAll(tasks);
            Logger.Info("Versions are verified for all the packages");
            return results.ToList();
        }

        protected virtual async Task<IPackageGraph> OnResolvingPackages(ICollection<PackageId> packages,
            Dictionary<PackageId, DeploymentPackageInfo> resolvedPackagesDictionary)
        {
            var mergedPackages = new List<PackageId>(packages);
            var productPackages = DeploymentContext.ProductConfigurationProvider.GetPackages().Packages;
            foreach (var productPackage in productPackages)
            {
                var addPackage = !packages.Any(x => string.Equals(x.Id, productPackage.Package.Id,
                    StringComparison.CurrentCultureIgnoreCase));

                if (addPackage)
                    mergedPackages.Add(productPackage.Package);
            }

            var resolvedPackages = await NuGetEngine.ResolvePackages(mergedPackages);
            NuGetEngine.AnalysePackages(resolvedPackages);

            var graphBuilder = new PackageGraphBuilder();
            foreach (var innerNode in resolvedPackages.InnerNodes)
            {
                TranslateNuGetGraphNode(innerNode, graphBuilder, resolvedPackagesDictionary);
            }

            Logger.Info($"{resolvedPackagesDictionary.Count} package(s) are resolved");

            return graphBuilder.Build();
        }

        protected PackageId TranslateNuGetGraphNode(GraphNode<RemoteResolveResult> node, PackageGraphBuilder graphBuilder,
            Dictionary<PackageId, DeploymentPackageInfo> resolvedPackagesDictionary)
        {
            if (node.Key.TypeConstraint != LibraryDependencyTarget.Package &&
                node.Key.TypeConstraint != LibraryDependencyTarget.PackageProjectExternal)
            {
                var exception = new ArgumentException($"Package {node.Key.Name} cannot be resolved from the sources");
                throw LogException(exception);
            }

            var package = new DeploymentPackageInfo
            {
                Id = new PackageId(node.Item.Key.Name, node.Item.Key.Version.ToNormalizedString(),
                        node.Item.Key.Version.IsPrerelease),
                Match = node.Item.Data.Match
            };

            graphBuilder.Append(package.Id,
                node.InnerNodes.Select(x => TranslateNuGetGraphNode(x, graphBuilder, resolvedPackagesDictionary)));
            
            if (!resolvedPackagesDictionary.ContainsKey(package.Id))
                resolvedPackagesDictionary.Add(package.Id, package);

            return package.Id;
        }

        protected virtual List<DeploymentAction> OnPreparingInstallationActions(IPackageGraph resolvedPackagesGraph,
            Dictionary<PackageId, DeploymentPackageInfo> resolvedPackages)
        {
            var deploymentActions = new List<DeploymentAction>();
            var productPackagesList = DeploymentContext.ProductConfigurationProvider.GetPackages().FlattenMostDependentFirst();
            var resolvedPackagesList = resolvedPackagesGraph.FlattenMostDependentFirst();

            var updateCount = 0;
            var installCount = 0;

            foreach (var packageId in resolvedPackagesList)
            {
                var packageMatch = productPackagesList.FirstOrDefault(x => x.Id == packageId.Id);
                var isUpdate = (packageMatch != null) && (packageMatch.Version != packageId.Version);
                var install = (packageMatch == null) || isUpdate;

                if (isUpdate)
                {
                    deploymentActions.Add(new DeletePackageAction(DeploymentContext, true, packageMatch));
                    updateCount++;
                }

                if (install)
                {
                    var packageInfo = resolvedPackages[packageId];
                    deploymentActions.Add(new DownloadPackageAction(DeploymentContext, isUpdate,
                        packageInfo, NuGetEngine, PackagesFolderPath));
                    deploymentActions.Add(new InstallPackageAction(DeploymentContext, isUpdate, packageInfo,
                        DeploymentStartegyProvider));

                    if (!isUpdate)
                        installCount++;
                }
            }

            Logger.Info($"\t{installCount} package(s) to install");
            Logger.Info($"\t{updateCount} package(s) to update");

            return deploymentActions.Where(x => x is DownloadPackageAction)
                .Concat(deploymentActions.Where(x => x is DeletePackageAction))
                .Concat(deploymentActions.Where(x => x is InstallPackageAction))
                .ToList();
        }

        protected virtual void OnFinilizingPackagesInstallation()
        {
            Logger.Info("Packages installation complete");
        }

        /// <summary>
        /// Uninstalls provided packages
        /// </summary>
        /// <param name="packages">Packages to uninstall</param>
        /// <param name="removeDependencies">Remove dependent packages if they are not referenced elsewhere</param>
        /// <returns>Task</returns>
        public async Task UninstallPackages(ICollection<PackageId> packages, bool removeDependencies)
        {
            packages = OnUninstallingPackages(packages, removeDependencies);
            var actions = OnPreparingUnInstallationActions(packages);

            DeploymentContext.RaiseDeploymentEvent(new BeforeMaintenanceEvent());

            using (var transaction = new DeploymentTransaction())
            {
                foreach (var deploymentAction in actions)
                {
                    deploymentAction.Execute(transaction);
                }

                DeploymentContext.ProductConfigurationProvider.Save();
                transaction.Commit();
            }

            DeploymentContext.RaiseDeploymentEvent(new AfterMaintenanceEvent());
            OnFinilizingPackagesUninstallation();
            await SuccessTask;
        }

        protected virtual List<DeploymentAction> OnPreparingUnInstallationActions(ICollection<PackageId> packages)
        {
            return packages.Select(package => new DeletePackageAction(DeploymentContext, false, package))
                    .Cast<DeploymentAction>()
                    .ToList();
        }

        protected virtual ICollection<PackageId> OnUninstallingPackages(ICollection<PackageId> packages, bool removeDependencies)
        {
            Logger.Info("Uninstalling packages");

            if (removeDependencies)
            {
                return DeploymentContext.ProductConfigurationProvider.GetPackages()
                    .SliceWithDependencies(packages, true)
                    .FlattenMostDependentFirst();
            }

            return packages;
        }

        protected virtual void OnFinilizingPackagesUninstallation()
        {
            Logger.Info("Packages uninstalled");
        }

        protected Exception LogException(Exception exception)
        {
            Logger.Trace(exception.Message);
            return exception;
        }
    }
}