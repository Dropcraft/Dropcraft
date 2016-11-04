using System.Collections.Generic;

namespace Dropcraft.Common.Configuration
{
    /// <summary>
    /// PackageConfiguration is an individual package's configuration
    /// </summary>
    public interface IPackageConfiguration
    {
        PackageId Id { get; }

        bool IsPackageEnabled { get; }

        PackageMetadataInfo PackageMetadata { get; }

        EntityActivationMode PackageActivationMode { get; }

        IEnumerable<PackageStartupHandlerInfo> GetPackageStartupHandlers();

        IEnumerable<RuntimeEventsHandlerInfo> GetRuntimeEventHandlers();

        IEnumerable<ExtensionInfo> GetExtensions();

        IEnumerable<ExtensibilityPointInfo> GetExtensibilityPoints();

        IEnumerable<DeploymentEventsHandlerInfo> GetPackageDeploymentHandlers();

        IEnumerable<string> GetInstalledFiles(bool deletableFilesOnly);
    }
}