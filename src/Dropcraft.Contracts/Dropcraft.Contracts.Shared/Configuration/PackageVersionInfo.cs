using System;
using System.Linq;

namespace Dropcraft.Contracts.Configuration
{
    /// <summary>
    /// Provider package version information using SimVer
    /// </summary>
    public class PackageVersionInfo
    {
        /// <summary>
        /// Package major version
        /// </summary>
        public int Major { get; }

        /// <summary>
        /// Package minor version
        /// </summary>
        public int Minor { get; }

        /// <summary>
        /// Package patch version
        /// </summary>
        public int Patch { get; }

        /// <summary>
        /// Package extended version, like beta, final, etc.
        /// </summary>
        public string Suffix { get; }

        public PackageVersionInfo(int major, int minor, int patch) : this(major, minor, patch, string.Empty)
        {
        }

        public PackageVersionInfo(int major, int minor, int patch, string suffix)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
            Suffix = suffix;
        }
    }
}
