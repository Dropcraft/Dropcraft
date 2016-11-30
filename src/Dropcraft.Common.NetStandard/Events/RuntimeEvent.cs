namespace Dropcraft.Common.Events
{
    public abstract class RuntimeEvent
    {
        public RuntimeContext Context { get; set; }
    }

    public class RuntimeStartedEvent : RuntimeEvent
    {
    }

    public class RuntimeStoppedEvent : RuntimeEvent
    {
    }

    public class AllRegularPackagesLoadedEvent : RuntimeEvent
    {
    }

    public class AllDeferredPackagesLoadedEvent : RuntimeEvent
    {
    }

    public class BeforePackageLoadedEvent : RuntimeEvent
    {
        public PackageId Package { get; set; }
    }

    public class AfterPackageLoadedEvent : RuntimeEvent
    {
        public PackageId Package { get; set; }
    }

    /* //TODO: start using?
    public class NewExtensibilityPointEvent : RuntimeEvent
    {
        public IExtensibilityPointHandler ExtensibilityPoint { get; set; }

        public ExtensibilityPointInfo ExtensibilityPointInfo { get; set; }
    }
    */
}