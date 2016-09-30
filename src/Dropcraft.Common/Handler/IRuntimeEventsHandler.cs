using Dropcraft.Common.Configuration;

namespace Dropcraft.Common.Handler
{
    public interface IRuntimeEventsHandler
    {
        void RuntimeStart(RuntimeStartEvent e);
        void RuntimeStop(RuntimeStopEvent e);
        void BeforePackageLoaded(BeforePackageLoadedEvent e);
        void AfterPackageLoaded(AfterPackageLoadedEvent e);
        void AllRegularPackagesLoaded(AllRegularPackagesLoadedEvent e);
        void AllDeferredPackagesLoaded(AllDeferredPackagesLoadedEvent e);
        void NewExtensibilityPoint(NewExtensibilityPointEvent e);
    }

    public abstract class RuntimeEvent
    {
        public IRuntimeContext Context { get; set; }

        public abstract void HandleEvent(IRuntimeEventsHandler eventHandler);
    }

    public class RuntimeStartEvent : RuntimeEvent {
        public override void HandleEvent(IRuntimeEventsHandler eventHandler)
        {
            eventHandler.RuntimeStart(this);
        }
    }

    public class RuntimeStopEvent : RuntimeEvent {
        public override void HandleEvent(IRuntimeEventsHandler eventHandler)
        {
            eventHandler.RuntimeStop(this);
        }
    }

    public class AllRegularPackagesLoadedEvent : RuntimeEvent {
        public override void HandleEvent(IRuntimeEventsHandler eventHandler)
        {
            eventHandler.AllRegularPackagesLoaded(this);
        }
    }

    public class AllDeferredPackagesLoadedEvent : RuntimeEvent {
        public override void HandleEvent(IRuntimeEventsHandler eventHandler)
        {
            eventHandler.AllDeferredPackagesLoaded(this);
        }
    }

    public class BeforePackageLoadedEvent : RuntimeEvent
    {
        public PackageInfo PackageInfo { get; set; }

        public override void HandleEvent(IRuntimeEventsHandler eventHandler)
        {
            eventHandler.BeforePackageLoaded(this);
        }
    }

    public class AfterPackageLoadedEvent : RuntimeEvent
    {
        public PackageInfo PackageInfo { get; set; }

        public override void HandleEvent(IRuntimeEventsHandler eventHandler)
        {
            eventHandler.AfterPackageLoaded(this);
        }
    }

    public class NewExtensibilityPointEvent : RuntimeEvent
    {
        public IExtensibilityPointHandler ExtensibilityPoint { get; set; }

        public ExtensibilityPointInfo ExtensibilityPointInfo { get; set; }

        public override void HandleEvent(IRuntimeEventsHandler eventHandler)
        {
            eventHandler.NewExtensibilityPoint(this);
        }
    }
}