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
        public async Task InstallPackages(IEnumerable<PackageId> packages, bool allowDowngrades, bool updatePackages)
        {
            Logger.Info("Installing packages...");

            NuGetEngine.AllowDowngrades = allowDowngrades;
            NuGetEngine.UpdatePackages = updatePackages;

            var productPackages = DeploymentContext.ProductConfigurationProvider
                                    .GetPackageConfigurations(DependencyOrdering.BottomToTop)
                                    .Select(x => x.Id);

            var topLevelProductPackages = DeploymentContext.ProductConfigurationProvider
                                    .GetPackageConfigurations(DependencyOrdering.TopPackagesOnly)
                                    .Select(x => x.Id);

            var workflowContext = new WorkflowContext(packages, productPackages, topLevelProductPackages);
            Logger.Info(
                $"{workflowContext.InputProductPackages.Count} product package(s) discovered and {workflowContext.InputPackages.Count} new package(s) requested");

            var workflow = GetDeploymentWorkflow(DeploymentContext, workflowContext, NuGetEngine);
            await workflow.EnsureAllPackagesAreVersioned();
            Logger.Info("Versions are verified and updated when needed");

            await workflow.ResolvePackages();
            Logger.Info("Packages are resolved");
            Logger.Info(
                $"\t{workflowContext.PackagesForInstallation.Count(x => !workflowContext.PackagesAffectedByUpdate.Contains(x.Id.Id))} package(s) to install");
            Logger.Info($"\t{workflowContext.PackagesAffectedByUpdate.Count} package(s) to update");

            DeploymentContext.RaiseDeploymentEvent(new BeforeMaintenanceEvent());

            workflow.DownloadPackages(PackagesFolderPath);
            Logger.Info($"All resolved packages are unpacked to {PackagesFolderPath}");

            using (var transaction = new FileTransaction())
            {
                workflow.DeletePackages(transaction, workflowContext.ProductPackagesForDeletion,
                    DeploymentContext.ProductConfigurationProvider);
                Logger.Info($"Uninstalled {workflowContext.ProductPackagesForDeletion.Count} package(s)");

                workflow.InstallPackages(transaction, workflowContext.PackagesForInstallation,
                    DeploymentContext.ProductConfigurationProvider,
                    DeploymentContext.PackageConfigurationProvider, DeploymentStartegyProvider);
                Logger.Info($"Installed {workflowContext.PackagesForInstallation.Count} package(s)");

                DeploymentContext.ProductConfigurationProvider.Save();
                transaction.Commit();
            }

            DeploymentContext.RaiseDeploymentEvent(new AfterMaintenanceEvent());
            Logger.Info("Packages installation complete");
        }

        /// <summary>
        /// Allows to define a custom deployment workflow
        /// </summary>
        /// <param name="deploymentContext">Current deployment context</param>
        /// <param name="workflowContext">Workflow context to carry</param>
        /// <param name="nuGetEngine">Current NuGet engine</param>
        /// <returns>Custom deployment workflow</returns>
        protected virtual DeploymentWorkflow GetDeploymentWorkflow(DeploymentContext deploymentContext, WorkflowContext workflowContext, NuGetEngine nuGetEngine)
        {
            return new DeploymentWorkflow(deploymentContext, workflowContext, nuGetEngine);
        }

        /// <summary>
        /// Uninstalls provided packages
        /// </summary>
        /// <param name="packages">Packages to uninstall</param>
        /// <param name="removeDependencies">Remove dependent packages if they are not referenced elsewhere</param>
        /// <returns>Task</returns>
        public async Task UninstallPackages(IEnumerable<PackageId> packages, bool removeDependencies)
        {
            Logger.Info("Uninstalling packages");
            var productPackages = DeploymentContext.ProductConfigurationProvider
                                    .GetPackageConfigurations(DependencyOrdering.TopToBottom)
                                    .Select(x=>x.Id);

            var topLevelProductPackages = DeploymentContext.ProductConfigurationProvider
                                    .GetPackageConfigurations(DependencyOrdering.TopPackagesOnly)
                                    .Select(x => x.Id);

            //TODO: implement removeDependencies

            var workflowContext = new WorkflowContext(new PackageId[] {}, productPackages, topLevelProductPackages);
            var workflow = GetDeploymentWorkflow(DeploymentContext, workflowContext, NuGetEngine);

            using (var transaction = new FileTransaction())
            {
                workflow.DeletePackages(transaction, packages, DeploymentContext.ProductConfigurationProvider);
                transaction.Commit();
                Logger.Info("Packages uninstalled");
            }

            await SuccessTask;
        }

    }

}