using System.Collections.Generic;

namespace Dropcraft.Common.Configuration
{
    /// <summary>
    /// PackageConfiguration is an individual package's configuration
    /// </summary>
    public interface IPackageConfiguration
    {
        bool IsPackageEnabled();

        EntityActivationMode GetPackageActivationMode();

        IEnumerable<PackageStartupHandlerInfo> GetPackageStartupHandlers();

        IEnumerable<RuntimeEventsHandlerInfo> GetRuntimeEventHandlers();

        IEnumerable<ExtensionInfo> GetExtensions();

        IEnumerable<ExtensibilityPointInfo> GetExtensibilityPoints();

        IEnumerable<DeploymentEventsHandlerInfo> GetPackageDeploymentHandlers();
    }
}