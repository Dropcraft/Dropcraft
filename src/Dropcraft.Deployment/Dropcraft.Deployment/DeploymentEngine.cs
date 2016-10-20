using System.Collections.Generic;
using System.Threading.Tasks;
using Dropcraft.Common;
using Dropcraft.Common.Configuration;
using Dropcraft.Common.Logging;
using Dropcraft.Deployment.NuGet;
using Dropcraft.Deployment.Workflow;

namespace Dropcraft.Deployment
{
    public class DeploymentEngine : IDeploymentEngine
    {
        private static readonly ILog Logger = LogProvider.For<DeploymentEngine>();
        protected NuGetEngine NuGetEngine { get; }
        protected IProductConfigurationProvider ProductConfigurationProvider { get; }
        protected IDeploymentStartegyProvider DeploymentStartegyProvider { get; }
        public DeploymentContext DeploymentContext { get; }


        public DeploymentEngine(DeploymentConfiguration configuration)
        {
            DeploymentContext = configuration.DeploymentContext;

            ProductConfigurationProvider =
                configuration.ProductConfigurationSource.GetProductConfigurationProvider(DeploymentContext);
            DeploymentStartegyProvider = configuration.DeploymentStrategySource.GetStartegyProvider(DeploymentContext);

            NuGetEngine = new NuGetEngine(configuration);
        }

        public async Task InstallPackages(IEnumerable<PackageId> packages)
        {
            Logger.Trace("Installing packages");
            var productPackages = ProductConfigurationProvider.GetPackages();
            var context = new InstallationContext(packages, productPackages);
            Logger.Trace($"{context.InputProductPackages.Count} product packages and {context.InputPackages.Count} new packages are requested");

            var workflow = GetInstallationWorkflow(context);
            await workflow.EnsureAllPackagesAreVersioned(context);
            Logger.Trace("Versions are verified and updated when needed");

            await workflow.ResolvePackages(context);
            Logger.Trace($"Packages are resolved: {context.PackagesForInstallation.Count} to be installed and {context.ProductPackagesForDeletion.Count} to be deleted");

            workflow.DownloadPackages(context, DeploymentContext.PackagesFolderPath);
            Logger.Trace($"All resolved packages are unpacked to {DeploymentContext.PackagesFolderPath}");

            // delete files
            // identify files to copy and filter
            // copy new files
            // create product.json
        }

        protected virtual InstallationWorkflow GetInstallationWorkflow(InstallationContext context)
        {
            return new InstallationWorkflow(NuGetEngine);
        }

        public async Task UninstallPackages(IEnumerable<PackageId> packages)
        {
            throw new System.NotImplementedException();
        }

    }

}