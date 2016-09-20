using System.Collections.Generic;
using Dropcraft.Common.Package;

namespace Dropcraft.Common.Configuration
{
    public interface IConfigurationSource
    {
        IInstallablePackageConfiguration GetPackageConfiguration(InstallablePackageInfo packageInfo, RuntimeContext runtimeContext);
        IInstaledPackageConfiguration GetPackageConfiguration(PackageInfo packageInfo, DeploymentContext deploymentContext);

        IApplicationConfiguration GetApplicationConfiguration(RuntimeContext runtimeContext);
    }

    public interface IApplicationConfiguration
    {
        IEnumerable<PackageInfo> GetPackages();
    }

    public interface IInstallablePackageConfiguration
    {
        IEnumerable<PackageDeploymentHandlerInfo> GetPackageDeploymentHandlers();
        void Reconfigure(DeploymentContext deploymentContext);
        void AppendToApplicationConfiguration();

    }

    public interface IInstaledPackageConfiguration
    {
        bool IsPackageEnabled();

        EntityActivationMode GetPackageActivationMode();

        IEnumerable<PackageStartupHandlerInfo> GetPackageStartupHandlers();
        IEnumerable<RuntimeEventsHandlerInfo> GetRuntimeEventHandlers();

        IEnumerable<ExtensionInfo> GetExtensions();
        IEnumerable<ExtensibilityPointInfo> GetExtensibilityPoints();
    }
}
