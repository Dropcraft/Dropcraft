using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Dropcraft.Common.Package;
using Dropcraft.Common.Runtime;

namespace Dropcraft.Runtime
{
    public class DefaultRuntimeContext : RuntimeContext
    {
        private readonly object _extensionLock;
        private readonly Dictionary<string, IExtensibilityPointHandler> _extensibilityPoints;
        private readonly List<ExtensionInfo> _extensions;

        private readonly ConcurrentDictionary<string, object> _handlers = new ConcurrentDictionary<string, object>();
        private readonly ReaderWriterLockSlim _eventLock = new ReaderWriterLockSlim();

        private readonly Dictionary<Type, Func<object>> _serviceFactories;

        public DefaultRuntimeContext(string productPath)
        {
            ProductPath = productPath;

            _extensionLock = new object();
            _extensibilityPoints = new Dictionary<string, IExtensibilityPointHandler>();
            _extensions = new List<ExtensionInfo>();
            _serviceFactories = new Dictionary<Type, Func<object>>();
    }

        protected override void OnRegisterExtensibilityPoint(string extensibilityPointKey, IExtensibilityPointHandler extensibilityPoint)
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

        protected override IExtensibilityPointHandler OnGetExtensibilityPoint(string extensibilityPointKey)
        {
            IExtensibilityPointHandler extensibilityPoint;
            lock (_extensionLock)
            {
                _extensibilityPoints.TryGetValue(extensibilityPointKey, out extensibilityPoint);
            }
            return extensibilityPoint;
        }

        protected override void OnRegisterExtension(ExtensionInfo extensionInfo)
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