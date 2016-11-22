using Dropcraft.Common.Configuration;
using Dropcraft.Common.Handler;

namespace Dropcraft.Common
{
    public abstract class DeploymentContext : IProductContext
    {
        /// <summary>
        /// Target .NET framework
        /// </summary>
        public string TargetFramework { get; protected set; }

        /// <summary>
        /// Path to install the product
        /// </summary>
        public string ProductPath { get; protected set; }

        public IProductConfigurationProvider ProductConfigurationProvider { get; protected set; }

        public IPackageConfigurationProvider PackageConfigurationProvider { get; protected set; }


        /// <summary>
        /// Register a handler for deployment events
        /// </summary>
        /// <param name="deploymentEventsHandler">Events handler</param>
        public void RegisterDeploymentEventHandler(IDeploymentEventsHandler deploymentEventsHandler)
            => OnRegisterDeploymentEventHandler(deploymentEventsHandler);

        /// <summary>
        /// Unregister the events handler
        /// </summary>
        /// <param name="deploymentEventsHandler">Events handler</param>
        public void UnregisterDeploymentEventHandler(IDeploymentEventsHandler deploymentEventsHandler)
            => OnUnregisterDeploymentEventHandler(deploymentEventsHandler);

        /// <summary>
        /// Raise a deployment event
        /// </summary>
        /// <param name="deploymentEvent">Deployment event</param>
        public void RaiseDeploymentEvent(DeploymentEvent deploymentEvent)
            => OnRaiseDeploymentEvent(deploymentEvent);

        protected abstract void OnRegisterDeploymentEventHandler(IDeploymentEventsHandler deploymentEventsHandler);
        protected abstract void OnUnregisterDeploymentEventHandler(IDeploymentEventsHandler deploymentEventsHandler);
        protected abstract void OnRaiseDeploymentEvent(DeploymentEvent deploymentEvent);
    }
}