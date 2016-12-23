using System.Collections.Generic;
using Dropcraft.Common.Package;

namespace Dropcraft.Common.Deployment
{
    /// <summary>
    /// Base deployment event
    /// </summary>
    public abstract class DeploymentEvent
    {
        /// <summary>
        /// Deployment context
        /// </summary>
        public DeploymentContext Context { get; set; }
    }

    /// <summary>
    /// Base class for package-related events
    /// </summary>
    public abstract class PackageDeploymentEvent : DeploymentEvent
    {
        /// <summary>
        /// Affected package
        /// </summary>
        public PackageId Id { get; set; }

        /// <summary>
        /// Indicates if the package is under update/downgrade from one version to another
        /// </summary>
        public bool IsUpdateInProgress { get; set; }
    }

    /// <summary>
    /// Happens before package is installed
    /// </summary>
    public class BeforePackageInstalledEvent : PackageDeploymentEvent
    {
        /// <summary>
        /// Package's files to install. This list is allowed to be modified by the event handler.
        /// </summary>
        public List<PackageFileDeploymentInfo> FilesToInstall { get; } = new List<PackageFileDeploymentInfo>();
    }

    /// <summary>
    /// Happens after package is installed
    /// </summary>
    public class AfterPackageInstalledEvent : PackageDeploymentEvent
    {
    }

    /// <summary>
    /// Happens before any modifications for product
    /// </summary>
    public class BeforeMaintenanceEvent : DeploymentEvent
    {
    }

    /// <summary>
    /// Happens after all modifications for product
    /// </summary>
    public class AfterMaintenanceEvent : DeploymentEvent
    {
    }

    /// <summary>
    /// Happens before package is uninstalled
    /// </summary>
    public class BeforePackageUninstalledEvent : PackageDeploymentEvent
    {
        /// <summary>
        /// Package's files to uninstall. This list is allowed to be modified by the event handler.
        /// </summary>
        public List<IPackageFile> FilesToDelete { get; } = new List<IPackageFile>();
    }

    /// <summary>
    /// Happens after package is uninstalled
    /// </summary>
    public class AfterPackageUninstalledEvent : PackageDeploymentEvent
    {
    }

    /// <summary>
    /// Happens after package is downloaded from source and unpacked
    /// </summary>
    public class AfterPackageDownloadedEvent : PackageDeploymentEvent
    {
        /// <summary>
        /// Path where package is unpacked
        /// </summary>
        public string PackagePath { get; set; }
    }

}