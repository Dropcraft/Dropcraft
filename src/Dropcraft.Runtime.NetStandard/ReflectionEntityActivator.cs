using System;
using Dropcraft.Common.Logging;
using Dropcraft.Common.Package;

namespace Dropcraft.Runtime
{
    public class ReflectionEntityActivator : IEntityActivator
    {
        private static readonly ILog Logger = LogProvider.For<ReflectionEntityActivator>();

        public static T Instantiate<T>(string className)
        {
            var type = Type.GetType(className);
            if (type == null)
            {
                throw new TypeLoadException($"Type not found: {className}");
            }

            return (T)Activator.CreateInstance(type);
        }

        public IExtensibilityPointHandler GetExtensibilityPointHandler(ExtensibilityPointInfo info)
        {
            Logger.TraceFormat("GetExtensibilityPointHandler for {info}", info);
            return Instantiate<IExtensibilityPointHandler>(info.ClassName);
        }

        public T GetExtension<T>(ExtensionInfo info) where T : class
        {
            Logger.TraceFormat("GetExtension for {info}", info);
            return Instantiate<T>(info.ClassName);
        }

        public IPackageStartupHandler GetPackageStartupHandler(PackageStartupHandlerInfo handlerInfo)
        {
            Logger.TraceFormat("GetPackageStartupHandler for {info}", handlerInfo);
            return Instantiate<IPackageStartupHandler>(handlerInfo.ClassName);
        }

        public IRuntimeEventsHandler GetRuntimeEventsHandler(RuntimeEventsHandlerInfo handlerInfo)
        {
            Logger.TraceFormat("GetRuntimeEventsHandler for {info}", handlerInfo);
            return Instantiate<IRuntimeEventsHandler>(handlerInfo.ClassName);
        }

        public IDeploymentEventsHandler GetDeploymentEventsHandler(DeploymentEventsHandlerInfo handlerInfo)
        {
            Logger.TraceFormat("GetDeploymentEventsHandler for {info}", handlerInfo);
            return Instantiate<IDeploymentEventsHandler>(handlerInfo.ClassName);
        }
    }
}