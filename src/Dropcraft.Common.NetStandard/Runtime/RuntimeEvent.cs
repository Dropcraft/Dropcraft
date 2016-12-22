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

    /// <summary>
    /// Happens at the beginning of the runtime initialization
    /// </summary>
    public class BeforeRuntimeStartedEvent : RuntimeEvent
    {
    }

    /// <summary>
    /// Heppens when the runtime is requested to be stopped
    /// </summary>
    public class AfterRuntimeStoppedEvent : RuntimeEvent
    {
    }

    /// <summary>
    /// Happens after all regular (non-deffered) packages are loaded
    /// </summary>
    public class AllRegularPackagesLoadedEvent : RuntimeEvent
    {
    }

    /// <summary>
    /// Happens after all deffered packages are loaded
    /// </summary>
    public class AllDeferredPackagesLoadedEvent : RuntimeEvent
    {
    }

    /// <summary>
    /// Happens before a package is loaded
    /// </summary>
    public class BeforePackageLoadedEvent : RuntimeEvent
    {
        /// <summary>
        /// Package to load
        /// </summary>
        public PackageId Package { get; set; }
    }

    /// <summary>
    /// Happens after package is loaded
    /// </summary>
    public class AfterPackageLoadedEvent : RuntimeEvent
    {
        /// <summary>
        /// Loaded package
        /// </summary>
        public PackageId Package { get; set; }
    }

    /// <summary>
    /// Happens before new extensibility point is registered
    /// </summary>
    public class NewExtensibilityPointRegistrationEvent : RuntimeEvent
    {
        /// <summary>
        /// Extensibility point key
        /// </summary>
        public string ExtensibilityPointKey { get; set; }

        /// <summary>
        /// Extensibility point handler requested for the registration.
        /// </summary>
        public IExtensibilityPointHandler ExtensibilityPointHandler { get; set; }

        /// <summary>
        /// Allows to block the extensibility point registration
        /// </summary>
        public bool IsRegistrationAllowed { get; set; } = true;
    }

    /// <summary>
    /// Happens before new extensibility point is unregistered
    /// </summary>
    public class ExtensibilityPointUnregistrationEvent : RuntimeEvent
    {
        /// <summary>
        /// Extensibility point key
        /// </summary>
        public string ExtensibilityPointKey { get; set; }

        /// <summary>
        /// Allows to block the extensibility point unregistration
        /// </summary>
        public bool IsUnregistrationAllowed { get; set; } = true;
    }

    /// <summary>
    /// Happens before new extension point is registered
    /// </summary>
    public class NewExtensionRegistrationEvent : RuntimeEvent
    {
        /// <summary>
        /// Extension requested to be registered
        /// </summary>
        public ExtensionInfo Extension { get; set; }

        /// <summary>
        /// Allows to block the extension registration
        /// </summary>
        public bool IsRegistrationAllowed { get; set; } = true;
    }
}