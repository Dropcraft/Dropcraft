using System.Collections.Generic;
using Dropcraft.Common.Package;

namespace Dropcraft.Runtime.Core
{
    /// <summary>
    /// ProductPackageInfo contains information about installed package 
    /// </summary>
    public class ProductPackageInfo
    {
        /// <summary>
        /// Gets or sets the package configuration.
        /// </summary>
        /// <value>The package configuration.</value>
        public IPackageConfiguration Configuration { get; set; }
        /// <summary>
        /// Contains information about the package's installed files.
        /// </summary>
        /// <value>List of the installed files.</value>
        public List<IPackageFile> Files { get; set; } = new List<IPackageFile>();
    }
}