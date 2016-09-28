using Dropcraft.Common.Handler;

namespace Dropcraft.Common
{
    public abstract class DeploymentContext : ProductContext
    {
        /// <summary>
        /// Path to install/uninstall packages 
        /// </summary>
        public string PackagesFolderPath { get; }

        protected DeploymentContext(string productPath, string packagesFolderPath)
            : base(productPath)
        {
            PackagesFolderPath = packagesFolderPath;
        }

        public void RegisterDeploymentEventHandler(IDeploymentEventsHandler deploymentEventsHandler)
        {
            OnRegisterDeploymentEventHandler(deploymentEventsHandler);
        }

        public void UnregisterRuntimeEventHandler(IDeploymentEventsHandler deploymentEventsHandler)
        {
            OnUnregisterDeploymentEventHandler(deploymentEventsHandler);
        }

        public void RaiseRuntimeEvent(DeploymentEvent deploymentEvent)
        {
            OnRaiseDeploymentEvent(deploymentEvent);
        }

        protected abstract void OnRegisterDeploymentEventHandler(IDeploymentEventsHandler deploymentEventsHandler);
        protected abstract void OnUnregisterDeploymentEventHandler(IDeploymentEventsHandler deploymentEventsHandler);
        protected abstract void OnRaiseDeploymentEvent(DeploymentEvent deploymentEvent);

    }
}