using System.Collections.Generic;

namespace Dropcraft.Common.Package
{
    /// <summary>
    /// InstallablePackageInfo represents a resolved package which can be deployed to a target folder
    /// </summary>
    public class InstallablePackageInfo : VersionedPackageInfo
    {
        public List<InstallableFileInfo> InstallableFiles { get; } = new List<InstallableFileInfo>();

        public InstallablePackageInfo(string id, string versionRange, bool allowPrereleaseVersions) 
            : base(id, versionRange, allowPrereleaseVersions)
        {
        }
    }
}