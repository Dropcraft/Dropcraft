using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dropcraft.Common;
using Dropcraft.Common.Package;
using Dropcraft.Deployment.NuGet;

namespace Dropcraft.Deployment
{
    public class DeploymentEngine : IDeploymentEngine
    {
        private readonly NuGetEngine _nuGetEngine;
        public DeploymentContext DeploymentContext { get; }

        public DeploymentEngine(DeploymentConfiguration configuration)
        {
            DeploymentContext = configuration.DeploymentContext;
            _nuGetEngine = new NuGetEngine(DeploymentContext, configuration.PackageSources, configuration.UpdatePackages);
        }

        public async Task InstallPackages(IEnumerable<VersionedPackageInfo> packages)
        {
            var installablePackages = await Task.WhenAll(packages.Select(_nuGetEngine.ResolveInstallablePackage));

            foreach (var package in installablePackages)
            {
                _nuGetEngine.InstallPackage(package).Wait();
            }
        }

        public async Task UpdatePackages(IEnumerable<VersionedPackageInfo> packages)
        {
        }

        public async Task UninstallPackages(IEnumerable<VersionedPackageInfo> packages)
        {
        }


    }
}
