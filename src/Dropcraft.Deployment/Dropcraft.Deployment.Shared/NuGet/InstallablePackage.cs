using Dropcraft.Common.Configuration;
using Dropcraft.Common.Package;
using NuGet.Versioning;

namespace Dropcraft.Deployment.NuGet
{
    internal class InstallablePackage
    {
        public string Id { get; }

        public NuGetVersion Version { get; }
        public bool AllowPrereleaseVersions { get; }

        public bool IsResolved => Version != null;

        public InstallablePackage(VersionedPackageInfo packageInfo)
            : this(packageInfo, null)
        {
        }

        public InstallablePackage(VersionedPackageInfo packageInfo, NuGetVersion version)
        {
            Id = packageInfo.Id;
            AllowPrereleaseVersions = packageInfo.AllowPrereleaseVersions;
            Version = version;
        }

    }
}
