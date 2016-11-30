using System.Collections.Generic;

namespace Dropcraft.Common.Events
{
    public abstract class DeploymentEvent
    {
        public DeploymentContext Context { get; set; }
    }

    public abstract class PackageDeploymentEvent : DeploymentEvent
    {
        public PackageId Id { get; set; }

        public bool IsUpdateInProgress { get; set; }
    }

    public class BeforePackageInstalledEvent : PackageDeploymentEvent
    {
        public List<PackageFileInfo> FilesToInstall { get; } = new List<PackageFileInfo>();
    }

    public class AfterPackageInstalledEvent : PackageDeploymentEvent
    {
    }

    public class BeforeMaintenanceEvent : DeploymentEvent
    {
    }

    public class AfterMaintenanceEvent : DeploymentEvent
    {
    }

    public class BeforePackageUninstalledEvent : PackageDeploymentEvent
    {
        public List<string> FilesToDelete { get; } = new List<string>();
    }

    public class AfterPackageUninstalledEvent : PackageDeploymentEvent
    {
    }

    public class AfterPackageDownloadedEvent : PackageDeploymentEvent
    {
        public string PackagePath { get; set; }
    }

}