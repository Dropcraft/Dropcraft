namespace Dropcraft.Common.Configuration
{
    /// <summary>
    /// InstallablePackageInfo uniquely defines package
    /// </summary>
    public class InstallablePackageInfo
    {
        /// <summary>
        /// Id provides identifier of the installablePackage, usually it is identical to NuGet package identifier
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// VersionRange defines version range of the package
        /// </summary>
        public string VersionRange { get; }

        public bool AllowPrereleaseVersions { get; }

        public InstallablePackageInfo(string id, string versionRange, bool allowPrereleaseVersions)
        {
            Id = id;
            VersionRange = versionRange;
            AllowPrereleaseVersions = allowPrereleaseVersions;
        }
    }

    /// <summary>
    /// PackageInfo provides information of the selected package
    /// </summary>
    public class PackageInfo : InstallablePackageInfo
    {
        /// <summary>
        /// Path provides path of the package folder
        /// </summary>
        public string ManifestFile { get; }

        /// <summary>
        /// Path provides path of the package folder
        /// </summary>
        public string InstallPath { get; }

        /// <summary>
        /// Metadata provides package's Metadata information
        /// </summary>
        public PackageMetadataInfo Metadata { get; }

        public PackageInfo(string id, string manifestFile, string installPath) 
            : this(id, manifestFile, installPath, string.Empty, false, null)
        {
        }

        public PackageInfo(string id, string manifestFile, string installPath, string versionRange, bool allowPrereleaseVersions, PackageMetadataInfo metadata)
            : base(id, versionRange, allowPrereleaseVersions)
        {
            ManifestFile = manifestFile;
            InstallPath = installPath;
            Metadata = metadata;
        }

        public override string ToString()
        {
            return $"{Id}, {VersionRange}";
        }
    }
}