using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Dropcraft.Common
{
    /// <summary>
    /// PackageInfo provides package's description
    /// </summary>
    public class PackageInfo
    {
        /// <summary>
        /// Identifies the package
        /// </summary>
        public PackageId PackageId { get; }

        /// <summary>
        /// Provides package's meta information
        /// </summary>
        public PackageMetadata Metadata { get; protected set; }

        /// <summary>
        /// Package files
        /// </summary>
        public ReadOnlyCollection<string> Files => FilesList.AsReadOnly();

        /// <summary>
        /// Modifiable package file list
        /// </summary>
        protected List<string> FilesList { get; set; } = new List<string>();

        public PackageInfo(PackageId packageId)
            : this(packageId, null)
        {
        }

        public PackageInfo(PackageId packageId, PackageMetadata metadata)
        {
            PackageId = packageId;
            Metadata = metadata;
        }

        public override string ToString()
        {
            return $"{PackageId.Id}, {PackageId.VersionRange}";
        }
    }
}