using Dropcraft.Common.Configuration;
using Dropcraft.Common.Handlers;
using Dropcraft.Common.Logging;

namespace Dropcraft.Common
{
    /// <summary>
    /// EntityActivator provides platform-independent instantiation logic for the standard types
    /// </summary>
    public abstract class EntityActivator
    {
        private static readonly ILog Logger = LogProvider.For<EntityActivator>();
        private static EntityActivator _currentEntityActivator;

        /// <summary>
        /// Currently selected EntityActivator
        /// </summary>
        public static EntityActivator Current => _currentEntityActivator;

        /// <summary>
        /// InitializeEntityActivator allows to assign EntityActivator to use
        /// </summary>
        /// <param name="entityActivator">New EntityActivator</param>
        public static void InitializeEntityActivator(EntityActivator entityActivator)
        {
            _currentEntityActivator = entityActivator;
        }

        /// <summary>
        /// Instantiates extensibility point using IExtensibilityPointInfo information
        /// </summary>
        /// <param name="info">Information about the extensibility point</param>
        /// <returns>Instantiated extensibility point</returns>
        public IExtensibilityPointHandler GetExtensibilityPointHandler(ExtensibilityPointInfo info)
        {
            Logger.TraceFormat("GetExtensibilityPointHandler for {info}", info);
            return OnGetExtensibilityPointHandler(info);
        }

        protected abstract IExtensibilityPointHandler OnGetExtensibilityPointHandler(ExtensibilityPointInfo info);

        /// <summary>
        /// Instantiates extension using IExtensionInfo information
        /// </summary>
        /// <param name="info">Information about the extension</param>
        /// <returns>Instantiated extension</returns>
        public T GetExtension<T>(ExtensionInfo info) where T : class
        {
            Logger.TraceFormat("GetExtension for {info}", info);
            return OnGetExtension<T>(info);
        }

        protected abstract T OnGetExtension<T>(ExtensionInfo info) where T : class;

        /// <summary>
        /// Instantiates startup event handler using provided PackageStartupHandlerInfo
        /// </summary>
        /// <param name="handlerInfo">Information about the handler to construct</param>
        /// <returns></returns>
        public IPackageStartupHandler GetPackageStartupHandler(PackageStartupHandlerInfo handlerInfo)
        {
            Logger.TraceFormat("GetPackageStartupHandler for {info}", handlerInfo);
            return OnGetPackageStartupHandler(handlerInfo);
        }

        protected abstract IPackageStartupHandler OnGetPackageStartupHandler(PackageStartupHandlerInfo handlerInfo);

        /// <summary>
        /// Instantiates runtime events handler using provided RuntimeEventsHandlerInfo
        /// </summary>
        /// <param name="handlerInfo">Information about the handler to construct</param>
        public IRuntimeEventsHandler GetRuntimeEventsHandler(RuntimeEventsHandlerInfo handlerInfo)
        {
            Logger.TraceFormat("GetRuntimeEventsHandler for {info}", handlerInfo);
            return OnGetRuntimeEventsHandler(handlerInfo);
        }

        protected abstract IRuntimeEventsHandler OnGetRuntimeEventsHandler(RuntimeEventsHandlerInfo handlerInfo);

        /// <summary>
        /// Instantiates deployment events handler using provided DeploymentEventsHandlerInfo
        /// </summary>
        /// <param name="handlerInfo">Information about the handler to construct</param>
        public IDeploymentEventsHandler GetDeploymentEventsHandler(DeploymentEventsHandlerInfo handlerInfo)
        {
            Logger.TraceFormat("GetDeploymentEventsHandler for {info}", handlerInfo);
            return OnGetDeploymentEventsHandler(handlerInfo);
        }

        protected abstract IDeploymentEventsHandler OnGetDeploymentEventsHandler(DeploymentEventsHandlerInfo handlerInfo);

    }
}
