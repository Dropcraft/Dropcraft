using System;
using Dropcraft.Common.Configuration;
using Dropcraft.Common.Events;
using Dropcraft.Common.Handlers;

namespace Dropcraft.Common
{
    public abstract class RuntimeContext : IProductContext
    {
        public string ProductPath { get; protected set; }

        public void RegisterExtensibilityPoint(string extensibilityPointKey, IExtensibilityPointHandler extensibilityPoint)
            => OnRegisterExtensibilityPoint(extensibilityPointKey, extensibilityPoint);

        public void UnregisterExtensibilityPoint(string extensibilityPointKey)
            => OnUnregisterExtensibilityPoint(extensibilityPointKey);

        public IExtensibilityPointHandler GetExtensibilityPoint(string extensibilityPointKey)
            => OnGetExtensibilityPoint(extensibilityPointKey);

        public void RegisterExtension(ExtensionInfo extensionInfo)
            => OnRegisterExtension(extensionInfo);

        public void RegisterEventHandler<T>(Action<T> handler) where T: RuntimeEvent
            => OnRegisterEventHandler(handler);

        public void UnregisterEventHandler<T>(Action<T> handler) where T : RuntimeEvent
            => OnUnregisterEventHandler(handler);

        public void RaiseRuntimeEvent(RuntimeEvent runtimeEvent)
            => OnRaiseRuntimeEvent(runtimeEvent);

        public T GetHostService<T>() where T : class => OnGetHostService<T>();

        public void RegisterHostService(Type type, Func<object> serviceFactory)
            => OnRegisterHostService(type, serviceFactory);


        protected abstract void OnRegisterExtensibilityPoint(string extensibilityPointKey, IExtensibilityPointHandler extensibilityPoint);

        protected abstract void OnUnregisterExtensibilityPoint(string extensibilityPointKey);

        protected abstract IExtensibilityPointHandler OnGetExtensibilityPoint(string extensibilityPointKey);

        protected abstract void OnRegisterExtension(ExtensionInfo extensionInfo);

        protected abstract void OnRegisterEventHandler<T>(Action<T> handler) where T : RuntimeEvent;
        protected abstract void OnUnregisterEventHandler<T>(Action<T> handler) where T : RuntimeEvent;

        protected abstract void OnRaiseRuntimeEvent<T>(T runtimeEvent) where T : RuntimeEvent;

        protected abstract T OnGetHostService<T>() where T : class;

        protected abstract void OnRegisterHostService(Type type, Func<object> serviceFactory);
    }
}