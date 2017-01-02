using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Dropcraft.Common.Package;
using Dropcraft.Common.Runtime;

namespace Dropcraft.Runtime
{
    /// <summary>
    /// Class DefaultRuntimeContext
    /// </summary>
    /// <seealso cref="Dropcraft.Common.Runtime.RuntimeContext" />
    public class DefaultRuntimeContext : RuntimeContext
    {
        private readonly object _extensionLock;
        private readonly Dictionary<string, IExtensibilityPointHandler> _extensibilityPoints;
        private readonly List<ExtensionInfo> _extensions;

        private readonly ConcurrentDictionary<string, object> _handlers = new ConcurrentDictionary<string, object>();
        private readonly ReaderWriterLockSlim _eventLock = new ReaderWriterLockSlim();

        private readonly Dictionary<Type, Func<object>> _serviceFactories;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultRuntimeContext"/> class.
        /// </summary>
        /// <param name="productPath">The product path.</param>
        public DefaultRuntimeContext(string productPath)
        {
            ProductPath = productPath;

            _extensionLock = new object();
            _extensibilityPoints = new Dictionary<string, IExtensibilityPointHandler>();
            _extensions = new List<ExtensionInfo>();
            _serviceFactories = new Dictionary<Type, Func<object>>();
    }

        /// <summary>
        /// Registers new extensibility point
        /// </summary>
        /// <param name="extensibilityPointKey">Extensibility point key. Used to connect extensibility point and extensions</param>
        /// <param name="extensibilityPoint">Extensibility point definition</param>
        protected override void OnRegisterExtensibilityPoint(string extensibilityPointKey, IExtensibilityPointHandler extensibilityPoint)
        {
            var e = new NewExtensibilityPointRegistrationEvent
            {
                ExtensibilityPointHandler = extensibilityPoint,
                ExtensibilityPointKey = extensibilityPointKey
            };

            RaiseRuntimeEvent(e);
            if (!e.IsRegistrationAllowed)
                return;

            lock (_extensionLock)
            {
                _extensibilityPoints.Add(e.ExtensibilityPointKey, e.ExtensibilityPointHandler);

                for (var i = _extensions.Count - 1; i >= 0; i--)
                {
                    if (_extensions[i].ExtensibilityPointId == e.ExtensibilityPointKey)
                    {
                        e.ExtensibilityPointHandler.RegisterExtension(_extensions[i]);
                        _extensions.RemoveAt(i);
                    }
                }
            }
        }

        /// <summary>
        /// Unregisters extensibility point. Unregistered extensibility point will not be informed about new extensions.
        /// </summary>
        /// <param name="extensibilityPointKey">Extensibility point to unregister</param>
        protected override void OnUnregisterExtensibilityPoint(string extensibilityPointKey)
        {
            var e = new ExtensibilityPointUnregistrationEvent {ExtensibilityPointKey = extensibilityPointKey};
            RaiseRuntimeEvent(e);
            if (!e.IsUnregistrationAllowed)
                return;

            lock (_extensionLock)
            {
                _extensibilityPoints.Remove(e.ExtensibilityPointKey);
            }
        }

        /// <summary>
        /// Returns registered extensibility point associated with the key.
        /// </summary>
        /// <param name="extensibilityPointKey">Extensibility point to return</param>
        /// <returns>Found extensibility point. Returns null if nothing is found.</returns>
        protected override IExtensibilityPointHandler OnGetExtensibilityPoint(string extensibilityPointKey)
        {
            IExtensibilityPointHandler extensibilityPoint;
            lock (_extensionLock)
            {
                _extensibilityPoints.TryGetValue(extensibilityPointKey, out extensibilityPoint);
            }
            return extensibilityPoint;
        }

        /// <summary>
        /// Registers new extension
        /// </summary>
        /// <param name="extensionInfo">Extension definition</param>
        protected override void OnRegisterExtension(ExtensionInfo extensionInfo)
        {
            var e = new NewExtensionRegistrationEvent {Extension = extensionInfo};
            RaiseRuntimeEvent(e);

            if (!e.IsRegistrationAllowed)
                return;

            lock (_extensionLock)
            {
                IExtensibilityPointHandler extensibilityPoint;
                if (_extensibilityPoints.TryGetValue(e.Extension.ExtensibilityPointId, out extensibilityPoint))
                {
                    extensibilityPoint.RegisterExtension(e.Extension);
                }
                else
                {
                    _extensions.Add(e.Extension);
                }
            }
        }

        /// <summary>
        /// Registers handler for runtime events
        /// </summary>
        /// <typeparam name="T">Type of the event to handle</typeparam>
        /// <param name="handler">Event handler</param>
        protected override void OnRegisterEventHandler<T>(Action<T> handler)
        {
            var list = (List<Action<T>>)_handlers.GetOrAdd(typeof(T).Name, x => new List<Action<T>>());
            list.Remove(handler);

            try
            {
                _eventLock.EnterWriteLock();
                list.Add(handler);
            }
            finally
            {
                _eventLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Unregisters handler for runtime events
        /// </summary>
        /// <typeparam name="T">Type of the event</typeparam>
        /// <param name="handler">Event handler</param>
        protected override void OnUnregisterEventHandler<T>(Action<T> handler)
        {
            object listObject;
            if (_handlers.TryGetValue(typeof(T).Name, out listObject))
            {
                var list = (List<Action<T>>) listObject;
                try
                {
                    _eventLock.EnterWriteLock();
                    list.Remove(handler);
                }
                finally
                {
                    _eventLock.ExitWriteLock();
                }
            }
        }

        /// <summary>
        /// Raise a new event
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="runtimeEvent">The runtime event.</param>
        protected override void OnRaiseRuntimeEvent<T>(T runtimeEvent)
        {
            if (runtimeEvent.Context == null)
                runtimeEvent.Context = this;

            object listObject;
            if (_handlers.TryGetValue(typeof(T).Name, out listObject))
            {
                var list = (List<Action<T>>)listObject;
                try
                {
                    _eventLock.EnterReadLock();
                    foreach (var handler in list)
                    {
                        handler(runtimeEvent);
                    }
                }
                finally
                {
                    _eventLock.ExitReadLock();
                }
            }
        }

        /// <summary>
        /// Returns registered host service
        /// </summary>
        /// <typeparam name="T">Type of the service</typeparam>
        /// <returns>Returns service object or null if nothing is found</returns>
        protected override T OnGetHostService<T>()
        {
            Func<object> func;
            if (_serviceFactories.TryGetValue(typeof (T), out func))
            {
                return func() as T;
            }

            return default(T);
        }

        /// <summary>
        /// Registers services to make them available for packages during start-up. It is not recommended to be used as a generic propose IoC container.
        /// </summary>
        /// <param name="type">Type of the service</param>
        /// <param name="serviceFactory">Service factory which returns service for the GetHostService requests</param>
        protected override void OnRegisterHostService(Type type, Func<object> serviceFactory)
        {
            _serviceFactories.Add(type, serviceFactory);
        }

    }
}