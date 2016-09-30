using System;
using Dropcraft.Common.Configuration;
using Dropcraft.Common.Handler;

namespace Dropcraft.Common
{
    public interface IRuntimeContext : IProductContext
    {
        void RegisterExtensibilityPoint(string extensibilityPointKey, IExtensibilityPointHandler extensibilityPoint);

        void UnregisterExtensibilityPoint(string extensibilityPointKey);

        IExtensibilityPointHandler GetExtensibilityPoint(string extensibilityPointKey);

        void RegisterExtension(ExtensionInfo extensionInfo);

        void RegisterRuntimeEventHandler(IRuntimeEventsHandler runtimeEventsHandler);

        void UnregisterRuntimeEventHandler(IRuntimeEventsHandler runtimeEventsHandler);

        void RaiseRuntimeEvent(RuntimeEvent runtimeEvent);

        T GetHostService<T>() where T : class;

        void RegisterHostService(Type type, Func<object> serviceFactory);
    }
}