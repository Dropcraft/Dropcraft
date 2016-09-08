using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Dropcraft.Common;
using Dropcraft.Common.Diagnostics;
using Dropcraft.Common.Configuration;
using Dropcraft.Deployment.NuGet;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Frameworks;
using NuGet.PackageManagement;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using SourceRepositoryProvider = Dropcraft.Deployment.NuGet.SourceRepositoryProvider;

namespace Dropcraft.Deployment
{
    public class DeploymentEngine : IDeploymentEngine
    {
        public DeploymentContext DeploymentContext { get; }

        private readonly bool _updatePackages;
        private readonly DropcraftProject _project;
        private readonly SourceRepositoryProvider _repositoryProvider;
        private readonly NuGetPackageManager _nuGetPackageManager;
        private readonly SourceRepository _localRepository;
        private readonly NuGetLogger _nuGetLogger;
        private readonly NuGetFramework _currentFramework;

        public DeploymentEngine(DeploymentConfiguration configuration)
        {
            _updatePackages = configuration.UpdatePackages;
            DeploymentContext = configuration.DeploymentContext;

            var settings = Settings.LoadDefaultSettings(DeploymentContext.PackagesFolderPath);

            _nuGetLogger = new NuGetLogger();
            _currentFramework = GetCurrentFramework();
            _localRepository = _repositoryProvider.CreateRepository(DeploymentContext.PackagesFolderPath);
            _repositoryProvider = new SourceRepositoryProvider(settings);
            _project = new DropcraftProject(DeploymentContext.PackagesFolderPath);
            _nuGetPackageManager = new NuGetPackageManager(_repositoryProvider, settings, DeploymentContext.PackagesFolderPath)
            {
                PackagesFolderNuGetProject = _project
            };

            foreach (var packageSource in configuration.PackageSources)
            {
                _repositoryProvider.AddPackageRepository(packageSource);
            }

        }

        public async Task InstallPackages(IEnumerable<InstallablePackageInfo> packages)
        {
            var installablePackages = await Task.WhenAll(packages.Select(ResolveInstallablePackage));
        }

        public async Task UpdatePackages(IEnumerable<InstallablePackageInfo> packages)
        {
        }

        public async Task UninstallPackages(IEnumerable<InstallablePackageInfo> packages)
        {
        }

        private async Task<InstallablePackage> ResolveInstallablePackage(InstallablePackageInfo packageInfo)
        {
            Trace.Current.Verbose($"Resolving package {packageInfo.Id}{packageInfo.VersionRange}");

            NuGetVersion resolvedVersion = null;
            if (!_updatePackages && !string.IsNullOrWhiteSpace(packageInfo.VersionRange))
            {
                resolvedVersion = await GetLatestMatchingVersion(packageInfo, _localRepository, _nuGetLogger);
            }

            if (resolvedVersion == null)
            {
            }

            return new InstallablePackage();
        }

        private async Task<NuGetVersion> GetLatestMatchingVersion(InstallablePackageInfo packageInfo, IEnumerable<SourceRepository> sourceRepositories, ILogger logger)
        {
            NuGetVersion[] versionMatches = await Task.WhenAll(sourceRepositories.Select(x => GetLatestMatchingVersion(packageInfo, x, logger)));
            return versionMatches
                .Where(x => x != null)
                .DefaultIfEmpty()
                .Max();
        }

        private async Task<NuGetVersion> GetLatestMatchingVersion(InstallablePackageInfo packageInfo, SourceRepository sourceRepository, ILogger logger)
        {
            try
            {
                var versionRange = VersionRange.Parse(packageInfo.VersionRange);
                DependencyInfoResource dependencyInfoResource = await sourceRepository.GetResourceAsync<DependencyInfoResource>();
                IEnumerable<SourcePackageDependencyInfo> dependencyInfo = await dependencyInfoResource.ResolvePackages(
                    packageInfo.Id, _currentFramework, logger, CancellationToken.None);
                return dependencyInfo
                    .Select(x => x.Version)
                    .Where(x => x != null && (versionRange == null || versionRange.Satisfies(x)))
                    .DefaultIfEmpty()
                    .Max();
            }
            catch (Exception ex)
            {
                Trace.Current.Warning($"Could not get latest version for package {packageInfo.Id}{packageInfo.VersionRange} from source {sourceRepository}: {ex.Message}");
                return null;
            }
        }
        private NuGetFramework GetCurrentFramework()
        {
            string frameworkName = Assembly.GetExecutingAssembly().GetCustomAttributes(true)
                .OfType<System.Runtime.Versioning.TargetFrameworkAttribute>()
                .Select(x => x.FrameworkName)
                .FirstOrDefault();
            return frameworkName == null
                ? NuGetFramework.AnyFramework
                : NuGetFramework.ParseFrameworkName(frameworkName, new DefaultFrameworkNameProvider());
        }

    }
}
