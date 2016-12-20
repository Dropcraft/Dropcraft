using Dropcraft.Common.Package;

namespace Dropcraft.Common.Runtime
{
    /// <summary>
    /// Base class for all runtime events
    /// </summary>
    public abstract class RuntimeEvent
    {
        /// <summary>
        /// Current runtime context
        /// </summary>
        public RuntimeContext Context { get; set; }
    }

    public class BeforeRuntimeStartedEvent : RuntimeEvent
    {
    }

    public class AfterRuntimeStoppedEvent : RuntimeEvent
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

    public class NewExtensibilityPointRegistrationEvent : RuntimeEvent
    {
        public string ExtensibilityPointKey { get; set; }
        public IExtensibilityPointHandler ExtensibilityPointHandler { get; set; }
        public bool IsRegistrationAllowed { get; set; } = true;
    }

    public class ExtensibilityPointUnregistrationEvent : RuntimeEvent
    {
        public string ExtensibilityPointKey { get; set; }
        public bool IsUnregistrationAllowed { get; set; } = true;
    }

    public class NewExtensionRegistrationEvent : RuntimeEvent
    {
        public ExtensionInfo Extension { get; set; }
        public bool IsRegistrationAllowed { get; set; } = true;
    }
}