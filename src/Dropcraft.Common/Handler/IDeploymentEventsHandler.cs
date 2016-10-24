namespace Dropcraft.Common.Handler
{
    public interface IDeploymentEventsHandler
    {
        void BeforePackageInstalled(BeforePackageInstalledEvent e);
        void AfterPackageInstalled(AfterPackageDeployedEvent e);
        void BeforePackageUpdated(BeforePackageUpdatedEvent e);
        void AfterPackageUpdated(AfterPackageUpdatedEvent e);
        void BeforePackageUninstalled(BeforePackageUninstalledEvent e);
        void AfterPackageUninstalled(AfterPackageUninstalledEvent e);
    }

    public abstract class DeploymentEvent
    {
        public DeploymentContext Context { get; set; }

        public PackageId PackageInfo { get; set; }

        public abstract void HandleEvent(IDeploymentEventsHandler eventHandler);
    }

    public class BeforePackageInstalledEvent : DeploymentEvent
    {
        public override void HandleEvent(IDeploymentEventsHandler eventHandler)
        {
            eventHandler.BeforePackageInstalled(this);
        }
    }

    public class AfterPackageDeployedEvent : DeploymentEvent
    {
        public override void HandleEvent(IDeploymentEventsHandler eventHandler)
        {
            eventHandler.AfterPackageInstalled(this);
        }
    }

    public class BeforePackageUpdatedEvent : DeploymentEvent
    {
        public override void HandleEvent(IDeploymentEventsHandler eventHandler)
        {
            eventHandler.BeforePackageUpdated(this);
        }
    }

    public class AfterPackageUpdatedEvent : DeploymentEvent
    {
        public override void HandleEvent(IDeploymentEventsHandler eventHandler)
        {
            eventHandler.AfterPackageUpdated(this);
        }
    }

    public class BeforePackageUninstalledEvent : DeploymentEvent
    {
        public override void HandleEvent(IDeploymentEventsHandler eventHandler)
        {
            eventHandler.BeforePackageUninstalled(this);
        }
    }

    public class AfterPackageUninstalledEvent : DeploymentEvent
    {
        public override void HandleEvent(IDeploymentEventsHandler eventHandler)
        {
            eventHandler.AfterPackageUninstalled(this);
        }
    }

}