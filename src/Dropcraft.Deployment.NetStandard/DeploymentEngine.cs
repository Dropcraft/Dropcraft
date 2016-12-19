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
using Dropcraft.Runtime.Configuration;

namespace Dropcraft.Deployment
{
    /// <summary>
    /// Deployment engine responsible for the packages installation/uninstallation
    /// </summary>
    public class DeploymentEngine : IDeploymentEngine
    {
        private static readonly ILog Logger = LogProvider.For<DeploymentEngine>();

        public INuGetEngine NuGetEngine;
        public IPackageDiscoverer PackageDiscoverer;
        public IPackageDeployer PackageDeployer;
        public IDeploymentTransactionSource TransactionSource;

        public IDeploymentStartegyProvider DeploymentStartegyProvider { get; }
        public string PackagesFolderPath { get; }
        public bool DontCachePackages { get; }

        /// <summary>
        /// Assigned deployment context
        /// </summary>
        public DeploymentContext DeploymentContext { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="deploymentContext">Deployment context to use</param>
        /// <param name="deploymentStartegyProvider">Deployment strategy to use</param>
        /// <param name="transactionSource">Transactions factory</param>
        /// <param name="packagesFolderPath">Path to cache packages</param>
        /// <param name="remotePackagesSources">Package sources</param>
        /// <param name="packageDiscoverer">Package version resolver</param>
        /// <param name="packageDeployer">Package installer</param>
        internal DeploymentEngine(DeploymentContext deploymentContext,
            IDeploymentStartegyProvider deploymentStartegyProvider,
            IPackageDiscoverer packageDiscoverer,
            IPackageDeployer packageDeployer,
            IDeploymentTransactionSource transactionSource,
            string packagesFolderPath,
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

            PackageDiscoverer = packageDiscoverer;
            PackageDiscoverer.NuGetEngine = NuGetEngine;

            PackageDeployer = packageDeployer;
            PackageDeployer.NuGetEngine = NuGetEngine;
            PackageDeployer.DeploymentContext = DeploymentContext;
            PackageDeployer.DeploymentStartegyProvider = DeploymentStartegyProvider;
            PackageDeployer.PackagesFolderPath = PackagesFolderPath;

            TransactionSource = transactionSource;
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
            Logger.Info("Installing packages...");
            DeploymentContext.RaiseDeploymentEvent(new BeforeMaintenanceEvent());

            NuGetEngine.AllowDowngrades = allowDowngrades;
            NuGetEngine.UpdatePackages = updatePackages;

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
        /// <param name="removeDependencies">Remove dependent packages if they are not referenced elsewhere</param>
        /// <returns>Task</returns>
        public async Task UninstallPackages(ICollection<PackageId> packages, bool removeDependencies)
        {
            Logger.Info("Uninstalling packages...");
            DeploymentContext.RaiseDeploymentEvent(new BeforeMaintenanceEvent());

            try
            {
                var uninstallationPlan = await PackageDeployer.PlanUninstallation(
                    DeploymentContext.ProductConfigurationProvider.GetPackages(),
                    packages, removeDependencies);

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
            var packageFiles = new Dictionary<PackageId, IEnumerable<string>>();

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