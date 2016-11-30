using System;
using Dropcraft.Common.Configuration;
using Dropcraft.Common.Events;

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
        /// <param name="handler">Event handler</param>
        public void RegisterEventHandler<T>(Action<T> handler) where T: DeploymentEvent
            => OnRegisterEventHandler(handler);

        /// <summary>
        /// Unregister a handler for deployment events
        /// </summary>
        /// <param name="handler">Event handler</param>
        public void UnregisterEventHandler<T>(Action<T> handler) where T : DeploymentEvent
            => OnUnregisterEventHandler(handler);

        /// <summary>
        /// Raise a deployment event
        /// </summary>
        /// <param name="deploymentEvent">Deployment event</param>
        public void RaiseDeploymentEvent<T>(T deploymentEvent) where T : DeploymentEvent
            => OnRaiseDeploymentEvent(deploymentEvent);

        protected abstract void OnRegisterEventHandler<T>(Action<T> handler) where T : DeploymentEvent;
        protected abstract void OnUnregisterEventHandler<T>(Action<T> handler) where T : DeploymentEvent;

        protected abstract void OnRaiseDeploymentEvent<T>(T deploymentEvent) where T : DeploymentEvent;
    }
}