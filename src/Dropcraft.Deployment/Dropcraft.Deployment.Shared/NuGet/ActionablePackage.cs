using System.Collections.Generic;
using Dropcraft.Common;
using NuGet.Versioning;

namespace Dropcraft.Deployment.NuGet
{
    internal class ActionablePackage : PackageInfo
    {
        public List<PackageFileInfo> ActionableFiles { get; } = new List<PackageFileInfo>();

        public NuGetVersion Version { get; }

        public bool IsResolved => Version != null;

        public ActionablePackage(PackageId packageInfo)
            : this(packageInfo, null)
        {
        }

        public ActionablePackage(PackageId packageInfo, NuGetVersion version) 
            : this(packageInfo.Id, packageInfo.VersionRange, packageInfo.AllowPrereleaseVersions, version)
        {
        }

        public ActionablePackage(string id, string versionRange, bool allowPrereleaseVersions, NuGetVersion version)
            : base(new PackageId(id, versionRange, allowPrereleaseVersions))
        {
            Version = version;

            if (Version != null)
                PackageId.ResolvedVersion = Version.ToString();
        }
    }
}
