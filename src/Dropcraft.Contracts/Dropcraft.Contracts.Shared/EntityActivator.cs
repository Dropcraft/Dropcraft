using Dropcraft.Contracts.Configuration;

namespace Dropcraft.Contracts
{
    /// <summary>
    /// EntityActivator provides platform-dependent instantiation logic for the standard types
    /// </summary>
    public abstract class EntityActivator
    {
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
        public IHandleExtensibilityPoint GetExtensibilityPointHandler(ExtensibilityPointInfo info)
        {
            return OnGetExtensibilityPointHandler(info);
        }

        protected abstract IHandleExtensibilityPoint OnGetExtensibilityPointHandler(ExtensibilityPointInfo info);

        /// <summary>
        /// Instantiates extension using IExtensionInfo information
        /// </summary>
        /// <param name="info">Information about the extension</param>
        /// <returns>Instantiated extension</returns>
        public T GetExtension<T>(ExtensionInfo info) where T : class
        {
            return OnGetExtension<T>(info);
        }

        protected abstract T OnGetExtension<T>(ExtensionInfo info) where T : class;

        /// <summary>
        /// Instantiates startup event handler using provided PackageStartupHandlerInfo
        /// </summary>
        /// <param name="handlerInfo">Information about the handler to construct</param>
        /// <returns></returns>
        public IHandlePackageStartup GetPackageStartupHandler(PackageStartupHandlerInfo handlerInfo)
        {
            return OnGetPackageStartupHandler(handlerInfo);
        }

        protected abstract IHandlePackageStartup OnGetPackageStartupHandler(PackageStartupHandlerInfo handlerInfo);

        /// <summary>
        /// Instantiates runtime events handler using provided RuntimeEventsHandlerInfo
        /// </summary>
        /// <param name="handlerInfo">Information about the handler to construct</param>
        public IHandleRuntimeEvents GetRuntimeEventsHandler(RuntimeEventsHandlerInfo handlerInfo)
        {
            return OnGetRuntimeEventsHandler(handlerInfo);
        }

        protected abstract IHandleRuntimeEvents OnGetRuntimeEventsHandler(RuntimeEventsHandlerInfo handlerInfo);

        /// <summary>
        /// Adds provided path to the list of locations to search for assemblies
        /// </summary>
        /// <param name="path">Path to add</param>
        public void AddPackagePath(string path)
        {
            OnAddPackagePath(path);
        }

        protected abstract void OnAddPackagePath(string path);

        /// <summary>
        /// Removes provided path from the list of locations to search for assemblies
        /// </summary>
        /// <param name="path">Path to remove</param>
        public void RemovePackagePath(string path)
        {
            OnRemovePackagePath(path);
        }

        protected abstract void OnRemovePackagePath(string path);
    }
}
