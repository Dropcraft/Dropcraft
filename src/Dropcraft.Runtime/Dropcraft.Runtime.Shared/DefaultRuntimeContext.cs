using System;
using System.Collections.Generic;
using Dropcraft.Contracts;
using Dropcraft.Contracts.Configuration;

namespace Dropcraft.Runtime
{
    public class DefaultRuntimeContext : RuntimeContext
    {
        private readonly object _eventsLock;
        private readonly List<IHandleRuntimeEvents> _eventHandlers;

        private readonly object _extensionLock;
        private readonly Dictionary<string, IHandleExtensibilityPoint> _extensibilityPoints;
        private readonly List<ExtensionInfo> _extensions;

        private readonly Dictionary<Type, Func<object>> _serviceFactories;

        public DefaultRuntimeContext()
        {
            _eventsLock = new object();
            _eventHandlers = new List<IHandleRuntimeEvents>();

            _extensionLock = new object();
            _extensibilityPoints = new Dictionary<string, IHandleExtensibilityPoint>();
            _extensions = new List<ExtensionInfo>();
            _serviceFactories = new Dictionary<Type, Func<object>>();
    }

        protected override void OnRegisterExtensibilityPoint(string extensibilityPointKey, IHandleExtensibilityPoint extensibilityPoint)
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

        protected override void OnUnregisterExtensibilityPoint(string extensibilityPointKey)
        {
            lock (_extensionLock)
            {
                _extensibilityPoints.Remove(extensibilityPointKey);
            }
        }

        protected override IHandleExtensibilityPoint OnGetExtensibilityPoint(string extensibilityPointKey)
        {
            IHandleExtensibilityPoint extensibilityPoint;
            lock (_extensionLock)
            {
                _extensibilityPoints.TryGetValue(extensibilityPointKey, out extensibilityPoint);
            }
            return extensibilityPoint;
        }

        protected override void OnStashExtension(ExtensionInfo extensionInfo)
        {
            lock (_extensionLock)
            {
                IHandleExtensibilityPoint extensibilityPoint;
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

        protected override void OnRegisterRuntimeEventHandler(IHandleRuntimeEvents runtimeEventsHandler)
        {
            lock (_eventsLock)
            {
                _eventHandlers.Add(runtimeEventsHandler);
            }
        }

        protected override void OnUnregisterRuntimeEventHandler(IHandleRuntimeEvents runtimeEventsHandler)
        {
            lock (_eventsLock)
            {
                _eventHandlers.Remove(runtimeEventsHandler);
            }
        }

        protected override void OnRaiseRuntimeEvent(RuntimeEvent runtimeEvent)
        {
            lock (_eventsLock)
            {
                foreach (var eventHandler in _eventHandlers)
                {
                    runtimeEvent.HandleEvent(eventHandler);
                }
            }
        }

        protected override T OnGetHostService<T>()
        {
            Func<object> func;
            if (_serviceFactories.TryGetValue(typeof (T), out func))
            {
                return func() as T;
            }

            return default(T);
        }

        protected override void OnRegisterHostService(Type type, Func<object> serviceFactory)
        {
            _serviceFactories.Add(type, serviceFactory);
        }

    }
}