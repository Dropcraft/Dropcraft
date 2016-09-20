using System;
using Dropcraft.Common.Configuration;

namespace Dropcraft.Common
{
    public abstract class RuntimeContext
    {
        public void RegisterExtensibilityPoint(string extensibilityPointKey,
            IHandleExtensibilityPoint extensibilityPoint)
        {
            OnRegisterExtensibilityPoint(extensibilityPointKey, extensibilityPoint);
        }

        public void UnregisterExtensibilityPoint(string extensibilityPointKey)
        {
            OnUnregisterExtensibilityPoint(extensibilityPointKey);
        }

        public IHandleExtensibilityPoint GetExtensibilityPoint(string extensibilityPointKey)
        {
            return OnGetExtensibilityPoint(extensibilityPointKey);
        }

        public void RegisterExtension(ExtensionInfo extensionInfo)
        {
            OnRegisterExtension(extensionInfo);
        }

        public void RegisterRuntimeEventHandler(IHandleRuntimeEvents runtimeEventsHandler)
        {
            OnRegisterRuntimeEventHandler(runtimeEventsHandler);
        }

        public void UnregisterRuntimeEventHandler(IHandleRuntimeEvents runtimeEventsHandler)
        {
            OnUnregisterRuntimeEventHandler(runtimeEventsHandler);
        }

        public void RaiseRuntimeEvent(RuntimeEvent runtimeEvent)
        {
            OnRaiseRuntimeEvent(runtimeEvent);
        }

        public T GetHostService<T>() where T : class
        {
            return OnGetHostService<T>();
        }

        public void RegisterHostService(Type type, Func<object> serviceFactory)
        {
            OnRegisterHostService(type, serviceFactory);
        }

        protected abstract void OnRegisterExtensibilityPoint(string extensibilityPointKey, IHandleExtensibilityPoint extensibilityPoint);
        protected abstract void OnUnregisterExtensibilityPoint(string extensibilityPointKey);
        protected abstract IHandleExtensibilityPoint OnGetExtensibilityPoint(string extensibilityPointKey);
        protected abstract void OnRegisterExtension(ExtensionInfo extensionInfo);
        protected abstract void OnRegisterRuntimeEventHandler(IHandleRuntimeEvents runtimeEventsHandler);
        protected abstract void OnUnregisterRuntimeEventHandler(IHandleRuntimeEvents runtimeEventsHandler);
        protected abstract void OnRaiseRuntimeEvent(RuntimeEvent runtimeEvent);
        protected abstract T OnGetHostService<T>() where T : class;

        protected abstract void OnRegisterHostService(Type type, Func<object> serviceFactory);
    }
}