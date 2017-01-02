using System;
using System.IO;
using Dropcraft.Common.Package;

namespace Dropcraft.Runtime.Core
{
    /// <summary>
    /// Defines a package file 
    /// </summary>
    public class PackageFileInfo : IPackageFile
    {
        /// <summary>
        /// Full file path, including file name
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public PackageFileInfo()
        {
        }

        /// <summary>
        /// Constructor which accepts file name
        /// </summary>
        /// <param name="fileName">File name</param>
        public PackageFileInfo(string fileName)
        {
            FileName = fileName;
        }

        /// <summary>
        /// Determines whether the current object represents the same file as the provided package file info.
        /// </summary>
        /// <param name="packageFileInfo">The package file information to compare.</param>
        /// <returns><c>true</c> if files are the same; otherwise, <c>false</c>.</returns>
        public bool IsSameFile(IPackageFile packageFileInfo)
        {
            var path1 = Path.GetFullPath(FileName);
            var path2 = Path.GetFullPath(packageFileInfo.FileName);

            return string.Equals(path1, path2, StringComparison.OrdinalIgnoreCase);
        }
    }
}