using System;
using System.Collections.Generic;
using NuGet.Configuration;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;

namespace Dropcraft.Deployment.NuGet
{
    class SourceRepositoryProvider : ISourceRepositoryProvider
    {
        public IEnumerable<SourceRepository> GetRepositories()
        {
            throw new NotImplementedException();
        }

        public SourceRepository CreateRepository(PackageSource source)
        {
            throw new NotImplementedException();
        }

        public SourceRepository CreateRepository(PackageSource source, FeedType type)
        {
            throw new NotImplementedException();
        }

        public IPackageSourceProvider PackageSourceProvider { get; }
    }
}
