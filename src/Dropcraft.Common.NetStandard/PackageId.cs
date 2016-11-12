using System;

namespace Dropcraft.Common
{
    /// <summary>
    /// PackageId represents package with ID and version
    /// </summary>
    public class PackageId
    {
        /// <summary>
        /// Id provides identifier of the installablePackage, usually it is identical to NuGet package identifier
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Defines version range of the package
        /// </summary>
        public string Version { get; }

        /// <summary>
        /// Defines if pre-release versions of the package are allowed
        /// </summary>
        public bool AllowPrereleaseVersions { get; }

        public PackageId(string id, string version, bool allowPrereleaseVersions)
        {
            Id = id;
            Version = version;
            AllowPrereleaseVersions = allowPrereleaseVersions;
        }

        public PackageId(string idString)
        {
            var parts = idString.Split('/');
            if (parts.Length != 2)
            {
                throw new ArgumentException("Incorrect package ID format");
            }

            Id = parts[0];
            Version = parts[1];
            AllowPrereleaseVersions = parts[1].Contains("-");
        }

        public bool IsSamePackage(PackageId id)
        {
            return Id == id.Id && Version == id.Version;
        }

        public override string ToString()
        {
            return $"{Id}/{Version}";
        }
    }
}