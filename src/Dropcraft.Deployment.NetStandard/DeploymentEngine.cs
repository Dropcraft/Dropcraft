using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dropcraft.Common;
using Dropcraft.Common.Deployment;
using Dropcraft.Common.Logging;
using Dropcraft.Common.Package;
using Dropcraft.Deployment.Core;
using Dropcraft.Deployment.NuGet;
using Dropcraft.Runtime.Core;

namespace Dropcraft.Deployment
{
    /// <summary>
    /// Deployment engine responsible for the packages installation/uninstallation
    /// </summary>
    public class DeploymentEngine : IDeploymentEngine
    {
        private static readonly ILog Logger = LogProvider.For<DeploymentEngine>();

        /// <summary>
        /// Associated NuGet engine (<see cref="INuGetEngine"/>). 
        /// </summary>
        public INuGetEngine NuGetEngine;

        /// <summary>
        /// Associated package discoverer (<see cref="IPackageDiscoverer"/>)
        /// </summary>
        public IPackageDiscoverer PackageDiscoverer;

        /// <summary>
        /// Associated package deployer (<see cref="IPackageDeployer"/>)
        /// </summary>
        public IPackageDeployer PackageDeployer;

        /// <summary>
        /// Associated transaction source (<see cref="IDeploymentTransactionSource"/>)
        /// </summary>
        public IDeploymentTransactionSource TransactionSource;

        /// <summary>
        /// Associated deployment strategy provider (<see cref="IDeploymentStrategyProvider"/>)
        /// </summary>
        public IDeploymentStrategyProvider DeploymentStrategyProvider { get; }

        /// <summary>
        /// Gets the packages folder path.
        /// </summary>
        /// <value>The packages folder path.</value>
        public string PackagesFolderPath { get; }

        /// <summary>
        /// Gets a value indicating whether the packages should be cached or not.
        /// </summary>
        public bool DontCachePackages { get; }

        /// <summary>
        /// Assigned deployment context
        /// </summary>
        public DeploymentContext DeploymentContext { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="deploymentContext">Deployment context to use</param>
        /// <param name="deploymentStrategyProvider">Deployment strategy to use</param>
        /// <param name="transactionSource">Transactions factory</param>
        /// <param name="packagesFolderPath">Path to cache packages</param>
        /// <param name="remotePackagesSources">Remote package sources</param>
        /// <param name="packageDiscoverer">Package version resolver</param>
        /// <param name="packageDeployer">Package installer</param>
        /// <param name="localPackagesSources">Local package sources</param>
        internal DeploymentEngine(DeploymentContext deploymentContext,
            IDeploymentStrategyProvider deploymentStrategyProvider,
            IPackageDiscoverer packageDiscoverer,
            IPackageDeployer packageDeployer,
            IDeploymentTransactionSource transactionSource,
            string packagesFolderPath,
            ICollection<string> remotePackagesSources,
            ICollection<string> localPackagesSources)
        {
            DeploymentContext = deploymentContext;
            DeploymentStrategyProvider = deploymentStrategyProvider;

            var extendedLocalPackageSources = localPackagesSources;
            if (string.IsNullOrWhiteSpace(packagesFolderPath))
            {
                PackagesFolderPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                DontCachePackages = true;
            }
            else
            {
                PackagesFolderPath = packagesFolderPath;
                extendedLocalPackageSources = localPackagesSources.Union(new[] { PackagesFolderPath }).ToList();
            }

            NuGetEngine = new NuGetEngine(deploymentContext, PackagesFolderPath, remotePackagesSources, extendedLocalPackageSources);

            PackageDiscoverer = packageDiscoverer;
            PackageDiscoverer.NuGetEngine = NuGetEngine;

            PackageDeployer = packageDeployer;
            PackageDeployer.NuGetEngine = NuGetEngine;
            PackageDeployer.DeploymentContext = DeploymentContext;
            PackageDeployer.DeploymentStrategyProvider = DeploymentStrategyProvider;
            PackageDeployer.PackagesFolderPath = PackagesFolderPath;

            TransactionSource = transactionSource;
        }

        /// <summary>
        /// Installs provided packages
        /// </summary>
        /// <param name="packages">Packages to install</param>
        /// <param name="options">Additional options</param>
        /// <returns>Task</returns>
        public async Task InstallPackages(ICollection<PackageId> packages, InstallationOptions options)
        {
            Logger.Info("Installing packages...");
            DeploymentContext.RaiseDeploymentEvent(new BeforeMaintenanceEvent());

            NuGetEngine.AllowDowngrades = options.AllowDowngrades;
            NuGetEngine.UpdatePackages = options.UpdatePackages;

            try
            {
                var productPackages = DeploymentContext.ProductConfigurationProvider.GetPackages();
                Logger.Info(
                    $"{productPackages.Count} product package(s) found and {packages.Count} new package(s) requested");

                var versionedPackages = await PackageDiscoverer.Discover(productPackages, packages);
                Logger.Info($"Versions are confirmed");

                var installationPlan = await PackageDeployer.PlanInstallation(productPackages, versionedPackages);
                Logger.Info($"{installationPlan.TargetPackageGraph.Count} package(s) are resolved");
                Logger.Info($"\t{installationPlan.InstallCount} package(s) to install");
                Logger.Info($"\t{installationPlan.UpdateCount} package(s) to update");

                var remainingPackages = new List<ProductPackageInfo>();
                foreach (var packageId in installationPlan.RemainingPackages)
                {
                    var info = new ProductPackageInfo();
                    info.Files.AddRange(DeploymentContext.ProductConfigurationProvider.GetInstalledFiles(packageId, false));
                    info.Configuration = DeploymentContext.ProductConfigurationProvider.GetPackageConfiguration(packageId);
                    remainingPackages.Add(info);
                }

                using (var transaction = TransactionSource.NewTransaction(DeploymentContext))
                {
                    foreach (var deploymentAction in installationPlan.Actions)
                    {
                        deploymentAction.Execute(transaction);
                    }

                    var newPackages = transaction.InstalledPackages.Union(remainingPackages).ToList();
                    if (ReconfiguringPackages(newPackages, installationPlan.TargetPackageGraph))
                        transaction.Commit();
                }
            }
            catch (Exception e)
            {
                Logger.TraceException(e.Message, e);
                throw;
            }

            if (DontCachePackages)
            {
                try
                {
                    Directory.Delete(PackagesFolderPath, true);
                }
                catch
                {
                    Logger.Warn($"Failed to delete packages folder: {PackagesFolderPath}");
                }
            }

            DeploymentContext.RaiseDeploymentEvent(new AfterMaintenanceEvent());
            Logger.Info("Installation complete.");
        }

        /// <summary>
        /// Uninstalls provided packages
        /// </summary>
        /// <param name="packages">Packages to uninstall</param>
        /// <param name="options">Additional options</param>
        /// <returns>Task</returns>
        public async Task UninstallPackages(ICollection<PackageId> packages, UninstallationOptions options)
        {
            Logger.Info("Uninstalling packages...");
            DeploymentContext.RaiseDeploymentEvent(new BeforeMaintenanceEvent());

            try
            {
                var uninstallationPlan = await PackageDeployer.PlanUninstallation(
                    DeploymentContext.ProductConfigurationProvider.GetPackages(),
                    packages, options.RemoveDependencies);

                using (var transaction = TransactionSource.NewTransaction(DeploymentContext))
                {
                    foreach (var action in uninstallationPlan.Actions)
                    {
                        action.Execute(transaction);
                    }

                    DeploymentContext.ProductConfigurationProvider.Save();
                    transaction.Commit();
                }
            }
            catch (Exception e)
            {
                Logger.TraceException(e.Message, e);
                throw;
            }

            DeploymentContext.RaiseDeploymentEvent(new AfterMaintenanceEvent());
            Logger.Info("Packages uninstalled.");
        }

        private bool ReconfiguringPackages(IEnumerable<ProductPackageInfo> packagesInfo, IPackageGraph resolvedPackagesGraph)
        {
            var packages = new List<IPackageConfiguration>();
            var packageFiles = new Dictionary<PackageId, IEnumerable<IPackageFile>>();

            foreach (var packageInfo in packagesInfo)
            {
                packages.Add(packageInfo.Configuration);
                packageFiles.Add(packageInfo.Configuration.Id, packageInfo.Files);
            }
            
            DeploymentContext.ProductConfigurationProvider.Reconfigure(packages, resolvedPackagesGraph, packageFiles);
            DeploymentContext.ProductConfigurationProvider.Save();
            return true;
        }
    }
}