using System;
using Dropcraft.Common;
using Dropcraft.Common.Configuration;

namespace Dropcraft.Runtime
{
    internal class ReflectionEntityActivator : EntityActivator
    {
        public static T Instantiate<T>(string className)
        {
            var type = Type.GetType(className);
            if (type == null)
            {
                throw new TypeLoadException($"Type not found: {className}");
            }

            return (T)Activator.CreateInstance(type);
        }

        protected override IHandleExtensibilityPoint OnGetExtensibilityPointHandler(ExtensibilityPointInfo info)
        {
            return Instantiate<IHandleExtensibilityPoint>(info.ClassName);
        }

        protected override T OnGetExtension<T>(ExtensionInfo info)
        {
            return Instantiate<T>(info.ClassName);
        }

        protected override IHandlePackageStartup OnGetPackageStartupHandler(PackageStartupHandlerInfo handlerInfo)
        {
            return Instantiate<IHandlePackageStartup>(handlerInfo.ClassName);
        }

        protected override IHandleRuntimeEvents OnGetRuntimeEventsHandler(RuntimeEventsHandlerInfo handlerInfo)
        {
            return Instantiate<IHandleRuntimeEvents>(handlerInfo.ClassName);
        }
    }
}