using Dropcraft.Common.Handler;

namespace Dropcraft.Common
{
    public interface IDeploymentContext : IProductContext
    {
        /// <summary>
        /// Path to install/uninstall packages 
        /// </summary>
        string PackagesFolderPath { get; }

        void RegisterDeploymentEventHandler(IDeploymentEventsHandler deploymentEventsHandler);

        void UnregisterDeploymentEventHandler(IDeploymentEventsHandler deploymentEventsHandler);

        void RaiseDeploymentEvent(DeploymentEvent deploymentEvent);
    }
}