namespace Dropcraft.Common.Package
{
    /// <summary>
    /// PackageInfo provides information of the selected package
    /// </summary>
    public class PackageInfo : VersionedPackageInfo
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