using System.Collections.Generic;

namespace Dropcraft.Contracts.Configuration
{
    public interface IParsedPackageConfiguration
    {
        bool IsPackageEnabled();

        EntityActivationMode GetPackageActivationMode();

        IEnumerable<PackageStartupHandlerInfo> GetPackageStartupHandlers();
        IEnumerable<RuntimeEventsHandlerInfo> GetRuntimeEventHandlers();

        IEnumerable<ExtensionInfo> GetExtensions();
        IEnumerable<ExtensibilityPointInfo> GetExtensibilityPoints();
    }
}