using System;
using Dropcraft.Common;
using Dropcraft.Common.Configuration;
using Dropcraft.Common.Handlers;

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

        protected override IExtensibilityPointHandler OnGetExtensibilityPointHandler(ExtensibilityPointInfo info)
        {
            return Instantiate<IExtensibilityPointHandler>(info.ClassName);
        }

        protected override T OnGetExtension<T>(ExtensionInfo info)
        {
            return Instantiate<T>(info.ClassName);
        }

        protected override IPackageStartupHandler OnGetPackageStartupHandler(PackageStartupHandlerInfo handlerInfo)
        {
            return Instantiate<IPackageStartupHandler>(handlerInfo.ClassName);
        }

        protected override IRuntimeEventsHandler OnGetRuntimeEventsHandler(RuntimeEventsHandlerInfo handlerInfo)
        {
            return Instantiate<IRuntimeEventsHandler>(handlerInfo.ClassName);
        }

        protected override IDeploymentEventsHandler OnGetDeploymentEventsHandler(DeploymentEventsHandlerInfo handlerInfo)
        {
            return Instantiate<IDeploymentEventsHandler>(handlerInfo.ClassName);
        }
    }
}