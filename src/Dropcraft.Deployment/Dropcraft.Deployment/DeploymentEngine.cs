using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dropcraft.Common;
using Dropcraft.Common.Configuration;
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
        protected IProductConfigurationProvider ProductConfigurationProvider { get; }
        protected IPackageConfigurationProvider PackageConfigurationProvider { get; }
        protected IDeploymentStartegyProvider DeploymentStartegyProvider { get; }
        public DeploymentContext DeploymentContext { get; }


        public DeploymentEngine(DeploymentConfiguration configuration)
        {
            DeploymentContext = configuration.DeploymentContext;

            ProductConfigurationProvider =
                configuration.ProductConfigurationSource.GetProductConfigurationProvider(DeploymentContext);
            PackageConfigurationProvider = configuration.PackageConfigurationSource.GetPackageConfigurationProvider(DeploymentContext);

            DeploymentStartegyProvider = configuration.DeploymentStrategySource.GetStartegyProvider(DeploymentContext);

            NuGetEngine = new NuGetEngine(configuration);
        }

        /// <summary>
        /// Installs provided packages
        /// </summary>
        /// <param name="packages">Packages to install</param>
        /// <returns>Task</returns>
        public async Task InstallPackages(IEnumerable<PackageId> packages)
        {
            Logger.Trace("Installing packages");
            var productPackages = ProductConfigurationProvider.GetPackageConfigurations().Select(x=>x.Id);
            var context = new WorkflowContext(packages, productPackages);
            Logger.Trace($"{context.InputProductPackages.Count} product packages and {context.InputPackages.Count} new packages are requested");

            var workflow = GetDeploymentWorkflow(context);
            await workflow.EnsureAllPackagesAreVersioned(context);
            Logger.Trace("Versions are verified and updated when needed");

            await workflow.ResolvePackages(context);
            Logger.Trace($"Packages are resolved: {context.PackagesForInstallation.Count} to be installed and {context.ProductPackagesForDeletion.Count} to be deleted");

            workflow.DownloadPackages(context, DeploymentContext.PackagesFolderPath);
            Logger.Trace($"All resolved packages are unpacked to {DeploymentContext.PackagesFolderPath}");

            using (var transaction = new FileTransaction())
            {
                workflow.DeletePackages(transaction, context.ProductPackagesForDeletion, ProductConfigurationProvider);
                Logger.Trace($"Uninstalled {context.ProductPackagesForDeletion.Count} package(s)");

                workflow.InstallPackages(transaction, context.PackagesForInstallation, ProductConfigurationProvider,
                    PackageConfigurationProvider, DeploymentStartegyProvider);
                Logger.Trace($"Installed {context.PackagesForInstallation.Count} package(s)");

                ProductConfigurationProvider.Save();
                transaction.Commit();
            }
        }

        /// <summary>
        /// Allows to define a custom deployment workflow
        /// </summary>
        /// <param name="context">Workflow context to carry</param>
        /// <returns>Custom deployment workflow</returns>
        protected virtual DeploymentWorkflow GetDeploymentWorkflow(WorkflowContext context)
        {
            return new DeploymentWorkflow(NuGetEngine);
        }

        /// <summary>
        /// Uninstalls provided packages
        /// </summary>
        /// <param name="packages">Packages to uninstall</param>
        /// <returns>Task</returns>
        public async Task UninstallPackages(IEnumerable<PackageId> packages)
        {
            Logger.Trace("Uninstalling packages");
            var productPackages = ProductConfigurationProvider.GetPackageConfigurations().Select(x=>x.Id);
            var context = new WorkflowContext(new PackageId[] {}, productPackages);
            var workflow = GetDeploymentWorkflow(context);

            using (var transaction = new FileTransaction())
            {
                workflow.DeletePackages(transaction, packages, ProductConfigurationProvider);
                transaction.Commit();
                Logger.Trace("Packages uninstalled");
            }

            await SuccessTask;
        }

    }

}