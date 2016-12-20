namespace Dropcraft.Common.Package
{
    /// <summary>
    /// IEntityActivator provides platform-independent instantiation logic for the standard package entities
    /// </summary>
    public interface IEntityActivator
    {
        /// <summary>
        /// Instantiates deployment events handler using provided DeploymentEventsHandlerInfo
        /// </summary>
        /// <param name="handlerInfo">Information about the handler to construct</param>
        IDeploymentEventsHandler GetDeploymentEventsHandler(DeploymentEventsHandlerInfo handlerInfo);

        /// <summary>
        /// Instantiates extensibility point using IExtensibilityPointInfo information
        /// </summary>
        /// <param name="info">Information about the extensibility point</param>
        /// <returns>Instantiated extensibility point</returns>
        IExtensibilityPointHandler GetExtensibilityPointHandler(ExtensibilityPointInfo info);

        /// <summary>
        /// Instantiates extension using IExtensionInfo information
        /// </summary>
        /// <param name="info">Information about the extension</param>
        /// <returns>Instantiated extension</returns>
        T GetExtension<T>(ExtensionInfo info) where T : class;

        /// <summary>
        /// Instantiates startup event handler using provided PackageStartupHandlerInfo
        /// </summary>
        /// <param name="handlerInfo">Information about the handler to construct</param>
        /// <returns>Instantiated handler</returns>
        IPackageStartupHandler GetPackageStartupHandler(PackageStartupHandlerInfo handlerInfo);

        /// <summary>
        /// Instantiates runtime events handler using provided RuntimeEventsHandlerInfo
        /// </summary>
        /// <param name="handlerInfo">Information about the handler to construct</param>
        IRuntimeEventsHandler GetRuntimeEventsHandler(RuntimeEventsHandlerInfo handlerInfo);
    }
}