using System.Collections.Generic;

namespace Dropcraft.Common.Handler
{
    public interface IDeploymentEventsHandler
    {
        void BeforeMaintenance(BeforeMaintenanceEvent e);
        void AfterMaintenance(AfterMaintenanceEvent e);
        void BeforePackageInstalled(BeforePackageInstalledEvent e);
        void AfterPackageInstalled(AfterPackageInstalledEvent e);
        void BeforePackageUninstalled(BeforePackageUninstalledEvent e);
        void AfterPackageUninstalled(AfterPackageUninstalledEvent e);
        void AfterPackageDownloaded(AfterPackageDownloadedEvent e);
    }

    public abstract class DeploymentEvent
    {
        public DeploymentContext Context { get; set; }

        public abstract void HandleEvent(IDeploymentEventsHandler eventHandler);
    }

    public abstract class PackageDeploymentEvent : DeploymentEvent
    {
        public PackageId Id { get; set; }

        public bool IsUpdateInProgress { get; set; }
    }

    public class BeforePackageInstalledEvent : PackageDeploymentEvent
    {
        public List<PackageFileInfo> FilesToInstall { get; } = new List<PackageFileInfo>();

        public override void HandleEvent(IDeploymentEventsHandler eventHandler)
        {
            eventHandler.BeforePackageInstalled(this);
        }
    }

    public class AfterPackageInstalledEvent : PackageDeploymentEvent
    {
        public override void HandleEvent(IDeploymentEventsHandler eventHandler)
        {
            eventHandler.AfterPackageInstalled(this);
        }
    }

    public class BeforeMaintenanceEvent : DeploymentEvent
    {
        public override void HandleEvent(IDeploymentEventsHandler eventHandler)
        {
            eventHandler.BeforeMaintenance(this);
        }
    }

    public class AfterMaintenanceEvent : DeploymentEvent
    {
        public override void HandleEvent(IDeploymentEventsHandler eventHandler)
        {
            eventHandler.AfterMaintenance(this);
        }
    }

    public class BeforePackageUninstalledEvent : PackageDeploymentEvent
    {
        public List<string> FilesToDelete { get; } = new List<string>();

        public override void HandleEvent(IDeploymentEventsHandler eventHandler)
        {
            eventHandler.BeforePackageUninstalled(this);
        }
    }

    public class AfterPackageUninstalledEvent : PackageDeploymentEvent
    {
        public override void HandleEvent(IDeploymentEventsHandler eventHandler)
        {
            eventHandler.AfterPackageUninstalled(this);
        }
    }

    public class AfterPackageDownloadedEvent : PackageDeploymentEvent
    {
        public string PackagePath { get; set; }

        public override void HandleEvent(IDeploymentEventsHandler eventHandler)
        {
            eventHandler.AfterPackageDownloaded(this);
        }
    }
}