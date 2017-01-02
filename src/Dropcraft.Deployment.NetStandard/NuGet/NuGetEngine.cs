using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Dropcraft.Common;
using Dropcraft.Common.Deployment;
using Dropcraft.Common.Logging;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.DependencyResolver;
using NuGet.Frameworks;
using NuGet.LibraryModel;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using NuGet.RuntimeModel;
using NuGet.Versioning;

namespace Dropcraft.Deployment.NuGet
{
    /// <summary>
    /// Class NuGetEngine.
    /// </summary>
    /// <seealso cref="Dropcraft.Deployment.NuGet.INuGetEngine" />
    public class NuGetEngine : INuGetEngine
    {
        private readonly NuGetLogger _nuGetLogger;
        private readonly SourceRepositoryProvider _repositoryProvider;
        private readonly SourceRepositoryProvider _localRepositoryProvider;
        private readonly NuGetFramework _framework;
        private static readonly ILog Logger = LogProvider.For<NuGetEngine>();
        private readonly SourceCacheContext _cache;

        /// <summary>
        /// Gets or sets a value indicating whether packages shall always be updated from the remote source.
        /// </summary>
        /// <value>When false, remote package sources will only be used when the packages cannot be resolved from the local sources</value>
        public bool UpdatePackages { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether package downgrades are allowed.
        /// </summary>
        /// <value><c>true</c> if [allow downgrades]; otherwise, <c>false</c>.</value>
        public bool AllowDowngrades { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NuGetEngine"/> class.
        /// </summary>
        /// <param name="deploymentContext">The deployment context.</param>
        /// <param name="packagesFolderPath">The packages folder path.</param>
        /// <param name="remotePackagesSources">The remote packages sources.</param>
        /// <param name="localPackagesSources">The local packages sources.</param>
        public NuGetEngine(DeploymentContext deploymentContext, string packagesFolderPath, IEnumerable<string> remotePackagesSources, IEnumerable<string> localPackagesSources)
        {
            var settings = Settings.LoadDefaultSettings(packagesFolderPath);
            _nuGetLogger = new NuGetLogger();
            _cache = new SourceCacheContext();

            _repositoryProvider = new SourceRepositoryProvider(settings);
            foreach (var packageSource in remotePackagesSources)
            {
                _repositoryProvider.AddPackageRepository(packageSource);
            }

            _localRepositoryProvider = new SourceRepositoryProvider(settings);
            foreach (var packagesSource in localPackagesSources)
            {
                _localRepositoryProvider.AddPackageRepository(packagesSource);
            }

            if (!string.IsNullOrWhiteSpace(deploymentContext.TargetFramework))
            {
                _framework = NuGetFramework.Parse(deploymentContext.TargetFramework);
                if (_framework == null)
                {
                    var ex = new ArgumentException($"Framework {deploymentContext.TargetFramework} is unknown");
                    Logger.Trace(ex.Message);
                    throw ex;
                }
            }
            else
            {
                _framework = GetCurrentFramework();
            }
        }

        /// <summary>
        /// Resolves the package version.
        /// </summary>
        /// <param name="packageId">The package identifier.</param>
        /// <returns><see cref="PackageId" /></returns>
        /// <exception cref="System.ArgumentException"></exception>
        public async Task<PackageId> ResolvePackageVersion(PackageId packageId)
        {
            PackageId package = null;
            if (!string.IsNullOrWhiteSpace(packageId.Version))
            {
                var resolvedVer = VersionRange.Parse(packageId.Version);
                if (resolvedVer == null)
                {
                    throw new ArgumentException($"Provided package version is incorrect: {packageId.Version}");
                }

                package = packageId;
            }
            else
            {
                var version = await ResolveNuGetVersion(packageId);
                if (version != null)
                {
                    package = new PackageId(packageId.Id, version, packageId.AllowPrereleaseVersions);
                }
            }

            if (package == null)
            {
                throw new ArgumentException($"Version for package {packageId.Id} cannot be resolved");
            }

            Logger.Trace($"Package {packageId.Id} resolved as {package.Id}/{package.Version}");
            return package;
        }

        private async Task<string> ResolveNuGetVersion(PackageId packageId)
        {
            NuGetVersion resolvedVersion = null;

            if (!UpdatePackages)
            {
                resolvedVersion = await GetLatestMatchingVersion(packageId, _localRepositoryProvider.Repositories, _nuGetLogger);
            }

            if (resolvedVersion == null && _repositoryProvider.Repositories.Count > 0)
            {
                resolvedVersion = await GetLatestMatchingVersion(packageId, _repositoryProvider.Repositories, _nuGetLogger);
            }

            return resolvedVersion == null ? null : resolvedVersion.ToFullString();
        }

        /// <summary>
        /// Resolves the packages and dependencies
        /// </summary>
        /// <param name="packages">The packages.</param>
        /// <returns>NuGet graph of the resolved packages</returns>
        public async Task<GraphNode<RemoteResolveResult>> ResolvePackages(ICollection<PackageId> packages)
        {
            var walkerContext = new RemoteWalkContext(_cache, _nuGetLogger);
            walkerContext.ProjectLibraryProviders.Add(new ProjectLibraryProvider(packages));

            foreach (var sourceRepository in _repositoryProvider.Repositories)
            {
                var provider = new SourceRepositoryDependencyProvider(sourceRepository, _nuGetLogger, _cache, true, true);
                walkerContext.RemoteLibraryProviders.Add(provider);
            }

            foreach (var sourceRepository in _localRepositoryProvider.Repositories)
            {
                var localProvider = new SourceRepositoryDependencyProvider(sourceRepository, _nuGetLogger, _cache, true, true);
                walkerContext.LocalLibraryProviders.Add(localProvider);
            }

            var fakeLib = new LibraryRange("Dropcraft", VersionRange.Parse("1.0.0"), LibraryDependencyTarget.Project);
            var walker = new RemoteDependencyWalker(walkerContext);

            return await walker.WalkAsync(fakeLib, _framework, _framework.GetShortFolderName(), RuntimeGraph.Empty, true);
        }

        /// <summary>
        /// Analyses the packages graph for potential issues (downgrades, missed package, circular dependency, etc.)
        /// </summary>
        /// <param name="resolvedPackages">The resolved packages.</param>
        /// <exception cref="System.InvalidOperationException"></exception>
        public void AnalyzePackages(GraphNode<RemoteResolveResult> resolvedPackages)
        {
            var packagesAnalysis = resolvedPackages.Analyze();
            if (packagesAnalysis.Downgrades.Any())
            {
                foreach (var downgrade in packagesAnalysis.Downgrades)
                {
                    Logger.Warn($"Downgrade from {downgrade.DowngradedFrom.Key} to "
                        + $"{downgrade.DowngradedTo.Key}");
                }

                if (!AllowDowngrades)
                {
                    var name = packagesAnalysis.Downgrades.First().DowngradedFrom.Item.Key.Name;
                    throw new InvalidOperationException($"At least one package requires downgrade and downgrade is not allowed: {name}");
                }
            }

            if (packagesAnalysis.Cycles.Any())
            {
                foreach (var cycle in packagesAnalysis.Cycles)
                {
                    Logger.Trace($"Cycle dependency for {cycle.Item.Key.Name},{cycle.Item.Key.Version.ToNormalizedString()}");
                }

                var name = packagesAnalysis.Cycles.First().Key.Name;
                throw new InvalidOperationException($"At least one package has cycle dependency: {name}");
            }

            if (packagesAnalysis.VersionConflicts.Any())
            {
                foreach (var conflict in packagesAnalysis.VersionConflicts)
                {
                    Logger.Trace($"Conflict for {conflict.Conflicting.Key.Name},{conflict.Conflicting.Key.VersionRange.ToNormalizedString()} resolved as "
                        + $"{conflict.Selected.Item.Key.Name},{conflict.Selected.Item.Key.Version.ToNormalizedString()}");
                }
            }
        }

        private async Task<NuGetVersion> GetLatestMatchingVersion(PackageId packageInfo, IEnumerable<SourceRepository> sourceRepositories, ILogger logger)
        {
            NuGetVersion[] versionMatches = await Task.WhenAll(sourceRepositories.Select(x => GetLatestMatchingVersion(packageInfo, x, logger)));
            return versionMatches
                .Where(x => x != null)
                .DefaultIfEmpty()
                .Max();
        }

        private async Task<NuGetVersion> GetLatestMatchingVersion(PackageId packageInfo, SourceRepository sourceRepository, ILogger logger)
        {
            try
            {
                var versionRange = string.IsNullOrWhiteSpace(packageInfo.Version) ? null : VersionRange.Parse(packageInfo.Version);
                var dependencyInfoResource = await sourceRepository.GetResourceAsync<DependencyInfoResource>();
                var dependencyInfo = await dependencyInfoResource.ResolvePackages(
                    packageInfo.Id, _framework, logger, CancellationToken.None);
                return dependencyInfo
                    .Select(x => x.Version)
                    .Where(x => x != null && (versionRange == null || versionRange.Satisfies(x)) && (packageInfo.AllowPrereleaseVersions || !x.IsPrerelease))
                    .DefaultIfEmpty()
                    .Max();
            }
            catch (Exception ex)
            {
                Logger.Warn($"Could not get latest version for package {packageInfo.Id}{packageInfo.Version} from source {sourceRepository}: {ex.Message}");
                return null;
            }
        }

        private NuGetFramework GetCurrentFramework()
        {
            string frameworkName = GetType().GetTypeInfo().Assembly.GetCustomAttributes()
                .OfType<System.Runtime.Versioning.TargetFrameworkAttribute>()
                .Select(x => x.FrameworkName)
                .FirstOrDefault();
            return frameworkName == null
                ? NuGetFramework.AnyFramework
                : NuGetFramework.ParseFrameworkName(frameworkName, new DefaultFrameworkNameProvider());
        }

        /// <summary>
        /// Installs the package.
        /// </summary>
        /// <param name="match">NuGet package match.</param>
        /// <param name="path">Installation path</param>
        /// <returns>Task</returns>
        /// <seealso cref="INuGetEngine.GetPackageTargetPath" />
        public Task InstallPackage(RemoteMatch match, string path)
        {
            var packageIdentity = new PackageIdentity(match.Library.Name, match.Library.Version);

            var versionFolderPathContext = new VersionFolderPathContext(
                packageIdentity,
                path,
                _nuGetLogger,
                PackageSaveMode.Defaultv3,
                XmlDocFileSaveMode.None);

            return PackageExtractor.InstallFromSourceAsync(
                stream => match.Provider.CopyToAsync(
                    match.Library,
                    stream,
                    _cache,
                    _nuGetLogger,
                    CancellationToken.None),
                versionFolderPathContext,
                CancellationToken.None);
        }

        /// <summary>
        /// Gets the package target path for installation.
        /// </summary>
        /// <param name="packageId">The package identifier.</param>
        /// <param name="version">Package NuGet version</param>
        /// <param name="path">Packages installation path</param>
        /// <returns>Appropriate package installation path, in a form of ...\[packages-path]\[package-id]\[package-version]\</returns>
        public string GetPackageTargetPath(string packageId, NuGetVersion version, string path)
        {
            var pathResolver = new VersionFolderPathResolver(path);
            return pathResolver.GetInstallPath(packageId, version);
        }

        /// <summary>
        /// Gets the most compatible framework.
        /// </summary>
        /// <param name="targetFramework">The target framework.</param>
        /// <param name="candidateFrameworks">The candidate frameworks.</param>
        /// <returns>The most compatible framework</returns>
        public static string GetMostCompatibleFramework(string targetFramework, IEnumerable<string> candidateFrameworks)
        {
            var frameworkReducer = new FrameworkReducer();
            var nearestFramework = frameworkReducer.GetNearest(NuGetFramework.Parse(targetFramework),
                candidateFrameworks.Select(NuGetFramework.Parse));

            return nearestFramework?.GetShortFolderName();
        }

    }
}