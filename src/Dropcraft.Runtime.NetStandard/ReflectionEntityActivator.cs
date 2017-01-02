using System;
using Dropcraft.Common.Logging;
using Dropcraft.Common.Package;

namespace Dropcraft.Runtime
{
    /// <summary>
    /// Class ReflectionEntityActivator.
    /// </summary>
    /// <seealso cref="Dropcraft.Common.Package.IEntityActivator" />
    public class ReflectionEntityActivator : IEntityActivator
    {
        private static readonly ILog Logger = LogProvider.For<ReflectionEntityActivator>();

        /// <summary>
        /// Instantiates object of the specified class
        /// </summary>
        /// <typeparam name="T">Class type</typeparam>
        /// <param name="className">Name of the class.</param>
        /// <returns>Instantiated object of T</returns>
        /// <exception cref="System.TypeLoadException"></exception>
        public static T Instantiate<T>(string className)
        {
            var type = Type.GetType(className);
            if (type == null)
            {
                throw new TypeLoadException($"Type not found: {className}");
            }

            return (T)Activator.CreateInstance(type);
        }

        /// <summary>
        /// Instantiates extensibility point using IExtensibilityPointInfo information
        /// </summary>
        /// <param name="info">Information about the extensibility point</param>
        /// <returns>Instantiated extensibility point</returns>
        public IExtensibilityPointHandler GetExtensibilityPointHandler(ExtensibilityPointInfo info)
        {
            Logger.TraceFormat("GetExtensibilityPointHandler for {info}", info);
            return Instantiate<IExtensibilityPointHandler>(info.ClassName);
        }

        /// <summary>
        /// Instantiates extension using IExtensionInfo information
        /// </summary>
        /// <typeparam name="T">Exception type</typeparam>
        /// <param name="info">Information about the extension</param>
        /// <returns>Instantiated extension</returns>
        public T GetExtension<T>(ExtensionInfo info) where T : class
        {
            Logger.TraceFormat("GetExtension for {info}", info);
            return Instantiate<T>(info.ClassName);
        }

        /// <summary>
        /// Instantiates startup event handler using provided PackageStartupHandlerInfo
        /// </summary>
        /// <param name="handlerInfo">Information about the handler to construct</param>
        /// <returns>Instantiated handler</returns>
        public IPackageStartupHandler GetPackageStartupHandler(PackageStartupHandlerInfo handlerInfo)
        {
            Logger.TraceFormat("GetPackageStartupHandler for {info}", handlerInfo);
            return Instantiate<IPackageStartupHandler>(handlerInfo.ClassName);
        }

        /// <summary>
        /// Instantiates runtime events handler using provided RuntimeEventsHandlerInfo
        /// </summary>
        /// <param name="handlerInfo">Information about the handler to construct</param>
        /// <returns>IRuntimeEventsHandler.</returns>
        public IRuntimeEventsHandler GetRuntimeEventsHandler(RuntimeEventsHandlerInfo handlerInfo)
        {
            Logger.TraceFormat("GetRuntimeEventsHandler for {info}", handlerInfo);
            return Instantiate<IRuntimeEventsHandler>(handlerInfo.ClassName);
        }

        /// <summary>
        /// Instantiates deployment events handler using provided DeploymentEventsHandlerInfo
        /// </summary>
        /// <param name="handlerInfo">Information about the handler to construct</param>
        /// <returns>IDeploymentEventsHandler.</returns>
        public IDeploymentEventsHandler GetDeploymentEventsHandler(DeploymentEventsHandlerInfo handlerInfo)
        {
            Logger.TraceFormat("GetDeploymentEventsHandler for {info}", handlerInfo);
            return Instantiate<IDeploymentEventsHandler>(handlerInfo.ClassName);
        }
    }
}