using System;
using System.Collections.Generic;
using Dropcraft.Common;
using Dropcraft.Common.Configuration;
using Dropcraft.Common.Handler;

namespace Dropcraft.Runtime
{
    public class RuntimeContext : IRuntimeContext
    {
        private readonly object _eventsLock;
        private readonly List<IRuntimeEventsHandler> _eventHandlers;

        private readonly object _extensionLock;
        private readonly Dictionary<string, IExtensibilityPointHandler> _extensibilityPoints;
        private readonly List<ExtensionInfo> _extensions;

        private readonly Dictionary<Type, Func<object>> _serviceFactories;
        public string ProductPath { get; private set; }

        public RuntimeContext(string productPath)
        {
            ProductPath = productPath;

            _eventsLock = new object();
            _eventHandlers = new List<IRuntimeEventsHandler>();

            _extensionLock = new object();
            _extensibilityPoints = new Dictionary<string, IExtensibilityPointHandler>();
            _extensions = new List<ExtensionInfo>();
            _serviceFactories = new Dictionary<Type, Func<object>>();
    }

        public void RegisterExtensibilityPoint(string extensibilityPointKey, IExtensibilityPointHandler extensibilityPoint)
        {
            lock (_extensionLock)
            {
                _extensibilityPoints.Add(extensibilityPointKey, extensibilityPoint);

                for (var i = _extensions.Count - 1; i >= 0; i--)
                {
                    if (_extensions[i].ExtensibilityPointId == extensibilityPointKey)
                    {
                        extensibilityPoint.RegisterExtension(_extensions[i]);
                        _extensions.RemoveAt(i);
                    }
                }
            }
        }

        public void UnregisterExtensibilityPoint(string extensibilityPointKey)
        {
            lock (_extensionLock)
            {
                _extensibilityPoints.Remove(extensibilityPointKey);
            }
        }

        public IExtensibilityPointHandler GetExtensibilityPoint(string extensibilityPointKey)
        {
            IExtensibilityPointHandler extensibilityPoint;
            lock (_extensionLock)
            {
                _extensibilityPoints.TryGetValue(extensibilityPointKey, out extensibilityPoint);
            }
            return extensibilityPoint;
        }

        public void RegisterExtension(ExtensionInfo extensionInfo)
        {
            lock (_extensionLock)
            {
                IExtensibilityPointHandler extensibilityPoint;
                if (_extensibilityPoints.TryGetValue(extensionInfo.ExtensibilityPointId, out extensibilityPoint))
                {
                    extensibilityPoint.RegisterExtension(extensionInfo);
                }
                else
                {
                    _extensions.Add(extensionInfo);
                }
            }
        }

        public void RegisterRuntimeEventHandler(IRuntimeEventsHandler runtimeEventsHandler)
        {
            lock (_eventsLock)
            {
                _eventHandlers.Add(runtimeEventsHandler);
            }
        }

        public void UnregisterRuntimeEventHandler(IRuntimeEventsHandler runtimeEventsHandler)
        {
            lock (_eventsLock)
            {
                _eventHandlers.Remove(runtimeEventsHandler);
            }
        }

        public void RaiseRuntimeEvent(RuntimeEvent runtimeEvent)
        {
            lock (_eventsLock)
            {
                foreach (var eventHandler in _eventHandlers)
                {
                    runtimeEvent.HandleEvent(eventHandler);
                }
            }
        }

        public T GetHostService<T>() where T: class
        {
            Func<object> func;
            if (_serviceFactories.TryGetValue(typeof (T), out func))
            {
                return func() as T;
            }

            return default(T);
        }

        public void RegisterHostService(Type type, Func<object> serviceFactory)
        {
            _serviceFactories.Add(type, serviceFactory);
        }

    }
}