using System.Collections.Generic;
using Dropcraft.Common;
using Dropcraft.Common.Configuration;
using Dropcraft.Deployment.NuGet;
using NuGet.Configuration;

namespace Dropcraft.Deployment
{
    public class DeploymentEngine : IDeploymentEngine
    {
        public DeploymentContext DeploymentContext { get; }

        private readonly DropcraftProject _project;
        private readonly SourceRepositoryProvider repositoryProvider;

        public DeploymentEngine(DeploymentConfiguration configuration)
        {
            DeploymentContext = configuration.DeploymentContext;

            _project = new DropcraftProject(configuration.InstallPath);
            
            repositoryProvider = new SourceRepositoryProvider(Settings.LoadDefaultSettings(configuration.InstallPath));
            foreach (var packageSource in configuration.PackageSources)
            {
                repositoryProvider.AddPackageRepository(packageSource);
            }
        }

        public void InstallPackages(IEnumerable<PackageId> packages)
        {
        }

        public void UpdatePackages(IEnumerable<PackageId> packages)
        {
        }

        public void UninstallPackages(IEnumerable<PackageId> packages)
        {
        }
    }
}
