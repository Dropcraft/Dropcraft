namespace Dropcraft.Common.Configuration
{
    /// <summary>
    /// PackageId uniquely defines package
    /// </summary>
    public class PackageId
    {
        /// <summary>
        /// Id provides identifier of the package, usually it is identical to NuGet package identifier 
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Version defines version of the package, usually it is identical to NuGet package version
        /// </summary>
        public string Version { get; }

        public PackageId(string id, string version)
        {
            Id = id;
            Version = version;
        }
    }

    /// <summary>
    /// PackageInfo provides information of the selected package
    /// </summary>
    public class PackageInfo : PackageId
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
            : this(id, manifestFile, installPath, null, null)
        {
        }

        public PackageInfo(string id, string manifestFile, string installPath, string version, PackageMetadataInfo metadata)
            : base(id, version)
        {
            ManifestFile = manifestFile;
            InstallPath = installPath;
            Metadata = metadata;
        }

        public override string ToString()
        {
            return $"{Id}, {Version}";
        }
    }
}