using System.Collections.Generic;
using System.Threading.Tasks;
using Dropcraft.Common;
using Dropcraft.Common.Configuration;
using Dropcraft.Common.Handler;
using Dropcraft.Common.Logging;
using Dropcraft.Deployment.NuGet;
using Dropcraft.Deployment.Workflow;

namespace Dropcraft.Deployment
{
    public class DeploymentEngine : IDeploymentEngine
    {
        private static readonly ILog Logger = LogProvider.For<DeploymentEngine>();
        private readonly List<IPackageFileFilteringHandler> _deploymentFilters;
        private readonly NuGetEngine _nuGetEngine;

        protected IProductConfigurationProvider ProductConfigurationProvider { get; private set; }
        public IDeploymentContext DeploymentContext { get; }


        public DeploymentEngine(DeploymentConfiguration configuration)
        {
            DeploymentContext = configuration.DeploymentContext;
            ProductConfigurationProvider =
                configuration.ProductConfigurationSource.GetProductConfigurationProvider(DeploymentContext);

            _deploymentFilters = new List<IPackageFileFilteringHandler>(configuration.DeploymentFilters);
            _nuGetEngine = new NuGetEngine(configuration);
        }

        public async Task InstallPackages(IEnumerable<PackageId> packages)
        {
            var productPackages = ProductConfigurationProvider.GetPackages();

            var workflow = GetInstallationWorkflow(packages, productPackages);
            await workflow.EnsureAllPackagesAreVersioned();
            await workflow.ResolvePackages();

            workflow.InstallPackages(DeploymentContext.PackagesFolderPath);
        }

        protected virtual InstallationWorkflow GetInstallationWorkflow(IEnumerable<PackageId> newPackages, IEnumerable<PackageId> productPackages)
        {
            var context = new InstallationContext(newPackages, productPackages);
            return new InstallationWorkflow(context, _nuGetEngine);
        }

        public async Task UninstallPackages(IEnumerable<PackageId> packages)
        {
            throw new System.NotImplementedException();
        }

    }

}