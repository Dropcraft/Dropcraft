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
        /// Registers new extensibility point
        /// </summary>
        /// <param name="extensibilityPointKey">Extensibility point key. Used to connect extensibility point and extensions</param>
        /// <param name="extensibilityPoint">Extensibility point definition</param>
        public void RegisterExtensibilityPoint(string extensibilityPointKey, IExtensibilityPointHandler extensibilityPoint)
            => OnRegisterExtensibilityPoint(extensibilityPointKey, extensibilityPoint);

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
        /// <param name="extensibilityPointKey">Extensibility point key. Used to connect extensibility point and extensions</param>
        /// <param name="extensibilityPoint">Extensibility point definition</param>
        protected abstract void OnRegisterExtensibilityPoint(string extensibilityPointKey, IExtensibilityPointHandler extensibilityPoint);

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