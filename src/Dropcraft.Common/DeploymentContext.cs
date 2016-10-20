using Dropcraft.Common.Handler;

namespace Dropcraft.Common
{
    public abstract class DeploymentContext : IProductContext
    {
        public string ProductPath { get; protected set; }

        /// <summary>
        /// Path to install/uninstall packages 
        /// </summary>
        public string PackagesFolderPath { get; protected set; }

        public void RegisterDeploymentEventHandler(IDeploymentEventsHandler deploymentEventsHandler)
            => OnRegisterDeploymentEventHandler(deploymentEventsHandler);

        public void UnregisterDeploymentEventHandler(IDeploymentEventsHandler deploymentEventsHandler)
            => OnUnregisterDeploymentEventHandler(deploymentEventsHandler);

        public void RaiseDeploymentEvent(DeploymentEvent deploymentEvent)
            => OnRaiseDeploymentEvent(deploymentEvent);

        protected abstract void OnRegisterDeploymentEventHandler(IDeploymentEventsHandler deploymentEventsHandler);
        protected abstract void OnUnregisterDeploymentEventHandler(IDeploymentEventsHandler deploymentEventsHandler);
        protected abstract void OnRaiseDeploymentEvent(DeploymentEvent deploymentEvent);
    }
}