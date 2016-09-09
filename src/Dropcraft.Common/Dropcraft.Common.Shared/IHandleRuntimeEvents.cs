using Dropcraft.Common.Configuration;
using Dropcraft.Common.Package;

namespace Dropcraft.Common
{
    public interface IHandleRuntimeEvents
    {
        void HandleRuntimeStart(RuntimeStartEvent e);
        void HandleRuntimeStop(RuntimeStopEvent e);
        void HandleBeforePackageLoaded(BeforePackageLoadedEvent e);
        void HandleAfterPackageLoaded(AfterPackageLoadedEvent e);
        void HandleAllRegularPackagesLoaded(AllRegularPackagesLoadedEvent e);
        void HandleAllDeferredPackagesLoaded(AllDeferredPackagesLoadedEvent e);
        void HandleNewExtensibilityPoint(NewExtensibilityPointEvent e);
    }

    public abstract class RuntimeEvent
    {
        public RuntimeContext Context { get; set; }

        public abstract void HandleEvent(IHandleRuntimeEvents eventHandler);
    }

    public class RuntimeStartEvent : RuntimeEvent {
        public override void HandleEvent(IHandleRuntimeEvents eventHandler)
        {
            eventHandler.HandleRuntimeStart(this);
        }
    }

    public class RuntimeStopEvent : RuntimeEvent {
        public override void HandleEvent(IHandleRuntimeEvents eventHandler)
        {
            eventHandler.HandleRuntimeStop(this);
        }
    }

    public class AllRegularPackagesLoadedEvent : RuntimeEvent {
        public override void HandleEvent(IHandleRuntimeEvents eventHandler)
        {
            eventHandler.HandleAllRegularPackagesLoaded(this);
        }
    }

    public class AllDeferredPackagesLoadedEvent : RuntimeEvent {
        public override void HandleEvent(IHandleRuntimeEvents eventHandler)
        {
            eventHandler.HandleAllDeferredPackagesLoaded(this);
        }
    }

    public class BeforePackageLoadedEvent : RuntimeEvent
    {
        public PackageInfo PackageInfo { get; set; }

        public override void HandleEvent(IHandleRuntimeEvents eventHandler)
        {
            eventHandler.HandleBeforePackageLoaded(this);
        }
    }

    public class AfterPackageLoadedEvent : RuntimeEvent
    {
        public PackageInfo PackageInfo { get; set; }

        public override void HandleEvent(IHandleRuntimeEvents eventHandler)
        {
            eventHandler.HandleAfterPackageLoaded(this);
        }
    }

    public class NewExtensibilityPointEvent : RuntimeEvent
    {
        public IHandleExtensibilityPoint ExtensibilityPoint { get; set; }

        public ExtensibilityPointInfo ExtensibilityPointInfo { get; set; }

        public override void HandleEvent(IHandleRuntimeEvents eventHandler)
        {
            eventHandler.HandleNewExtensibilityPoint(this);
        }
    }
}