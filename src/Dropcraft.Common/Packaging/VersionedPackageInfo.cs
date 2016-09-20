namespace Dropcraft.Common.Package
{
    /// <summary>
    /// VersionedPackageInfo represents package with ID and version
    /// </summary>
    public class VersionedPackageInfo
    {
        /// <summary>
        /// Id provides identifier of the installablePackage, usually it is identical to NuGet package identifier
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// VersionRange defines version range of the package
        /// </summary>
        public string VersionRange { get; }

        /// <summary>
        /// Actual package version
        /// </summary>
        public string ResolvedVersion { get; protected set; }

        /// <summary>
        /// Defines if prerelease versions of the package are allowed
        /// </summary>
        public bool AllowPrereleaseVersions { get; }

        public VersionedPackageInfo(string id, string versionRange, bool allowPrereleaseVersions)
        {
            Id = id;
            VersionRange = versionRange;
            AllowPrereleaseVersions = allowPrereleaseVersions;
            ResolvedVersion = string.Empty;
        }
    }
}