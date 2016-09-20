using System.Collections.Generic;

namespace Dropcraft.Common.Configuration
{
    public interface IRuntimeParsedPackageConfig
    {
        bool IsPackageEnabled();

        EntityActivationMode GetPackageActivationMode();

        IEnumerable<PackageStartupHandlerInfo> GetPackageStartupHandlers();
        IEnumerable<RuntimeEventsHandlerInfo> GetRuntimeEventHandlers();

        IEnumerable<ExtensionInfo> GetExtensions();
        IEnumerable<ExtensibilityPointInfo> GetExtensibilityPoints();
    }
}