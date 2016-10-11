using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using NuGet.Configuration;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;

namespace Dropcraft.Deployment.NuGet
{
    public class SourceRepositoryProvider : ISourceRepositoryProvider
    {
        private readonly List<SourceRepository> _repositories = new List<SourceRepository>();
        private readonly ConcurrentDictionary<PackageSource, SourceRepository> _repositoryCache;
        private readonly List<Lazy<INuGetResourceProvider>> _resourceProviders;

        public IReadOnlyList<SourceRepository> Repositories => _repositories;

        public SourceRepositoryProvider(ISettings settings)
        {
            _repositoryCache = new ConcurrentDictionary<PackageSource, SourceRepository>();
            PackageSourceProvider = new PackageSourceProvider(settings);

            _resourceProviders = new List<Lazy<INuGetResourceProvider>>();
            _resourceProviders.AddRange(Repository.Provider.GetCoreV3());
        }

        public void AddPackageRepository(string packageRepository) => _repositories.Add(CreateRepository(packageRepository));

        public SourceRepository CreateRepository(string packageSource) => CreateRepository(new PackageSource(packageSource));

        public IEnumerable<SourceRepository> GetRepositories() => _repositoryCache.Values;

        public SourceRepository CreateRepository(PackageSource source)
        {
            return _repositoryCache.GetOrAdd(source, x => new SourceRepository(source, _resourceProviders));
        }

        public SourceRepository CreateRepository(PackageSource source, FeedType type)
        {
            return _repositoryCache.GetOrAdd(source, x => new SourceRepository(source, _resourceProviders, type));
        }

        public IPackageSourceProvider PackageSourceProvider { get; }
    }
}