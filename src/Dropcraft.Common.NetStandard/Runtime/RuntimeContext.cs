using System;
using Dropcraft.Common.Package;

namespace Dropcraft.Common.Runtime
{
    /// <summary>
    /// Defines runtime context
    /// </summary>
    public abstract class RuntimeContext : IProductContext
    {
        /// <summary>
        /// Product path
        /// </summary>
        public string ProductPath { get; protected set; }

        /// <summary>
        /// Current entity activator
        /// </summary>
        public IEntityActivator EntityActivator { get; protected set; }

        /// <summary>
        /// Registers new extensibility point
        /// </summary>
        /// <param name="extensibilityPointInfo">Extensibility point definition</param>
        public void RegisterExtensibilityPoint(ExtensibilityPointInfo extensibilityPointInfo)
            => OnRegisterExtensibilityPoint(extensibilityPointInfo);

        /// <summary>
        /// Registers an already instantiated and configured extensibility point handler. For this handler <see cref="IExtensibilityPointHandler.Initialize"/> will not be called.
        /// </summary>
        /// <param name="extensibilityPointId">Extensibility point ID</param>
        /// <param name="extensibilityPointHandler">Extensibility point handler</param>
        public void RegisterExtensibilityPoint(string extensibilityPointId, IExtensibilityPointHandler extensibilityPointHandler)
            => OnRegisterExtensibilityPoint(extensibilityPointId, extensibilityPointHandler);

        /// <summary>
        /// Unregisters extensibility point. Unregistered extensibility point will not be informed about new extensions.
        /// </summary>
        /// <param name="extensibilityPointKey">Extensibility point to unregister</param>
        public void UnregisterExtensibilityPoint(string extensibilityPointKey)
            => OnUnregisterExtensibilityPoint(extensibilityPointKey);

        /// <summary>
        /// Returns registered extensibility point associated with the key.
        /// </summary>
        /// <param name="extensibilityPointKey">Extensibility point to return</param>
        /// <returns>Found extensibility point. Returns null if nothing is found.</returns>
        public IExtensibilityPointHandler GetExtensibilityPoint(string extensibilityPointKey)
            => OnGetExtensibilityPoint(extensibilityPointKey);

        /// <summary>
        /// Registers new extension
        /// </summary>
        /// <param name="extensionInfo">Extension definition</param>
        public void RegisterExtension(ExtensionInfo extensionInfo)
            => OnRegisterExtension(extensionInfo);

        /// <summary>
        /// Registers handler for runtime events 
        /// </summary>
        /// <typeparam name="T">Type of the event to handle</typeparam>
        /// <param name="handler">Event handler</param>
        public void RegisterEventHandler<T>(Action<T> handler) where T: RuntimeEvent
            => OnRegisterEventHandler(handler);

        /// <summary>
        /// Unregisters handler for runtime events 
        /// </summary>
        /// <typeparam name="T">Type of the event</typeparam>
        /// <param name="handler">Event handler</param>
        public void UnregisterEventHandler<T>(Action<T> handler) where T : RuntimeEvent
            => OnUnregisterEventHandler(handler);

        /// <summary>
        /// Raise a new event
        /// </summary>
        /// <param name="runtimeEvent">Event to sent</param>
        public void RaiseRuntimeEvent(RuntimeEvent runtimeEvent)
            => OnRaiseRuntimeEvent(runtimeEvent);

        /// <summary>
        /// Registers services to make them available for packages during start-up. It is not recommended to be used as a generic propose IoC container.
        /// </summary>
        /// <param name="type">Type of the service</param>
        /// <param name="serviceFactory">Service factory which returns service for the GetHostService requests</param>
        public void RegisterHostService(Type type, Func<object> serviceFactory)
            => OnRegisterHostService(type, serviceFactory);

        /// <summary>
        /// Returns registered host service
        /// </summary>
        /// <typeparam name="T">Type of the service</typeparam>
        /// <returns>Returns service object or null if nothing is found</returns>
        public T GetHostService<T>() where T : class => OnGetHostService<T>();


        /// <summary>
        /// Registers new extensibility point
        /// </summary>
        /// <param name="extensibilityPointInfo">The extensibility point information.</param>
        protected abstract void OnRegisterExtensibilityPoint(ExtensibilityPointInfo extensibilityPointInfo);

        /// <summary>
        /// Unregisters extensibility point. Unregistered extensibility point will not be informed about new extensions.
        /// </summary>
        /// <param name="extensibilityPointKey">Extensibility point to unregister</param>
        protected abstract void OnUnregisterExtensibilityPoint(string extensibilityPointKey);

        /// <summary>
        /// Returns registered extensibility point associated with the key.
        /// </summary>
        /// <param name="extensibilityPointKey">Extensibility point to return</param>
        /// <returns>Found extensibility point. Returns null if nothing is found.</returns>
        protected abstract IExtensibilityPointHandler OnGetExtensibilityPoint(string extensibilityPointKey);

        /// <summary>
        /// Registers an already instantiated and configured extensibility point handler. For this handler <see cref="IExtensibilityPointHandler.Initialize"/> will not be called.
        /// <see cref="NewExtensibilityPointRegistrationEvent"/> will not be called as well.
        /// </summary>
        /// <param name="extensibilityPointId">Extensibility point ID</param>
        /// <param name="extensibilityPointHandler">Extensibility point handler</param>
        protected abstract void OnRegisterExtensibilityPoint(string extensibilityPointId,
            IExtensibilityPointHandler extensibilityPointHandler);

        /// <summary>
        /// Registers new extension
        /// </summary>
        /// <param name="extensionInfo">Extension definition</param>
        protected abstract void OnRegisterExtension(ExtensionInfo extensionInfo);

        /// <summary>
        /// Registers handler for runtime events 
        /// </summary>
        /// <typeparam name="T">Type of the event to handle</typeparam>
        /// <param name="handler">Event handler</param>
        protected abstract void OnRegisterEventHandler<T>(Action<T> handler) where T : RuntimeEvent;

        /// <summary>
        /// Unregisters handler for runtime events 
        /// </summary>
        /// <typeparam name="T">Type of the event</typeparam>
        /// <param name="handler">Event handler</param>
        protected abstract void OnUnregisterEventHandler<T>(Action<T> handler) where T : RuntimeEvent;

        /// <summary>
        /// Raise a new event
        /// </summary>
        protected abstract void OnRaiseRuntimeEvent<T>(T runtimeEvent) where T : RuntimeEvent;

        /// <summary>
        /// Returns registered host service
        /// </summary>
        /// <typeparam name="T">Type of the service</typeparam>
        /// <returns>Returns service object or null if nothing is found</returns>
        protected abstract T OnGetHostService<T>() where T : class;

        /// <summary>
        /// Registers services to make them available for packages during start-up. It is not recommended to be used as a generic propose IoC container.
        /// </summary>
        /// <param name="type">Type of the service</param>
        /// <param name="serviceFactory">Service factory which returns service for the GetHostService requests</param>
        protected abstract void OnRegisterHostService(Type type, Func<object> serviceFactory);
    }
}