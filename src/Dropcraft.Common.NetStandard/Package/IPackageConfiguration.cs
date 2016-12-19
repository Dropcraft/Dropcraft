using System.Collections.Generic;

namespace Dropcraft.Common.Package
{
    /// <summary>
    /// PackageConfiguration is an individual package's configuration
    /// </summary>
    public interface IPackageConfiguration
    {
        PackageId Id { get; }

        bool IsPackageEnabled { get; }

        PackageMetadataInfo GetPackageMetadata();

        EntityActivationMode GetPackageActivationMode();

        IEnumerable<PackageStartupHandlerInfo> GetPackageStartupHandlers();

        IEnumerable<RuntimeEventsHandlerInfo> GetRuntimeEventHandlers();

        IEnumerable<ExtensionInfo> GetExtensions();

        IEnumerable<ExtensibilityPointInfo> GetExtensibilityPoints();

        IEnumerable<DeploymentEventsHandlerInfo> GetPackageDeploymentHandlers();

        string AsJson();
    }
}