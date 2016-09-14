using Dropcraft.Common.Package;
using NuGet.Versioning;

namespace Dropcraft.Deployment.NuGet
{
    internal class InstallablePackage : InstallablePackageInfo
    {
        public NuGetVersion Version { get; }

        public bool IsResolved => Version != null;

        public InstallablePackage(VersionedPackageInfo packageInfo)
            : this(packageInfo, null)
        {
        }

        public InstallablePackage(VersionedPackageInfo packageInfo, NuGetVersion version) 
            : this(packageInfo.Id, packageInfo.VersionRange, packageInfo.AllowPrereleaseVersions, version)
        {
        }

        public InstallablePackage(string id, string versionRange, bool allowPrereleaseVersions, NuGetVersion version)
            : base(id, versionRange, allowPrereleaseVersions)
        {
            Version = version;

            if (Version != null)
                ResolvedVersion = Version.ToString();
        }
    }
}
