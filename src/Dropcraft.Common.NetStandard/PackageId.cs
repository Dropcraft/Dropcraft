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

        /// <summary>
        /// Constructs PackageId from ID, version and pre-release information
        /// </summary>
        /// <param name="id">Package ID</param>
        /// <param name="version">Package version</param>
        /// <param name="allowPrereleaseVersions">Is pre-released package</param>
        public PackageId(string id, string version, bool allowPrereleaseVersions)
        {
            Id = id;
            Version = version;
            AllowPrereleaseVersions = allowPrereleaseVersions;
        }

        /// <summary>
        /// Constructs PackageId from string
        /// </summary>
        /// <param name="idString">String format is ID/version</param>
        public PackageId(string idString)
        {
            var parts = idString.Split('/');
            if (parts.Length != 2)
            {
                throw new ArgumentException("Incorrect package ID format (<name>/<version>)");
            }

            Id = parts[0];
            Version = parts[1];
            AllowPrereleaseVersions = parts[1].Contains("-");
        }

        /// <summary>
        /// Determines if packages have the same IDs and versions. AllowPrereleaseVersions flag is ignored for the comparison.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool IsSamePackage(PackageId id)
        {
            return Id == id.Id && Version == id.Version;
        }

        /// <summary>
        /// Converts object to string
        /// </summary>
        /// <returns>String formatted as ID/version</returns>
        public override string ToString()
        {
            return $"{Id}/{Version}";
        }
    }
}