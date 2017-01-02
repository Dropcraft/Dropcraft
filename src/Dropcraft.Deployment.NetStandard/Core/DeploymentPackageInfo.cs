using Dropcraft.Common;
using NuGet.DependencyResolver;

namespace Dropcraft.Deployment.Core
{
    /// <summary>
    /// Information about the package during the deployment
    /// </summary>
    public class DeploymentPackageInfo
    {
        /// <summary>
        /// Gets or sets the package identifier.
        /// </summary>
        /// <value><see cref="PackageId"/></value>
        public PackageId Id { get; set; }
        /// <summary>
        /// Gets or sets the package NuGet information, which allows to download and install the package.
        /// </summary>
        public RemoteMatch Match { get; set; }

        /// <summary>
        /// Gets or sets the package's target path.
        /// </summary>
        public string PackagePath { get; set; }
    }
}