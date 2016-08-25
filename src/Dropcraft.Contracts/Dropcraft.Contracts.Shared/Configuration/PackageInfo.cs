namespace Dropcraft.Contracts.Configuration
{
    /// <summary>
    /// PackageInfo provides information of the selected package
    /// </summary>
    public class PackageInfo
    {
        /// <summary>
        /// Id provides identifier of the package, usually it is identical to NuGet package identifier 
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Path provides path of the package folder
        /// </summary>
        public string ManifestFile { get; }

        /// <summary>
        /// Path provides path of the package folder
        /// </summary>
        public string InstallPath { get; }

        /// <summary>
        /// Version defines version of the package, usually it is identical to NuGet package version
        /// </summary>
        public PackageVersionInfo Version { get; }

        /// <summary>
        /// Metadata provides package's Metadata information
        /// </summary>
        public PackageMetadataInfo Metadata { get; }

        public PackageInfo(string id, string manifestFile, string installPath) 
            : this(id, manifestFile, installPath, null, null)
        {
        }

        public PackageInfo(string id, string manifestFile, string installPath, PackageVersionInfo version, PackageMetadataInfo metadata)
        {
            Id = id;
            ManifestFile = manifestFile;
            InstallPath = installPath;
            Version = version;
            Metadata = metadata;
        }
    }
}