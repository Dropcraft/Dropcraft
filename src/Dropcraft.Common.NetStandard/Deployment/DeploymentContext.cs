using System;
using Dropcraft.Common.Package;

namespace Dropcraft.Common.Deployment
{
    /// <summary>
    /// Defines deployment context
    /// </summary>
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

        /// <summary>
        /// Current entity activator
        /// </summary>
        public IEntityActivator EntityActivator { get; protected set; }

        /// <summary>
        /// Configuration provider for the target produc
        /// </summary>
        public IProductConfigurationProvider ProductConfigurationProvider { get; protected set; }

        /// <summary>
        /// Configuration provider for packages
        /// </summary>
        public IPackageConfigurationProvider PackageConfigurationProvider { get; protected set; }

        /// <summary>
        /// Register a handler for deployment events
        /// </summary>
        /// <typeparam name="T">Event type</typeparam>
        /// <param name="handler">Event handler</param>
        public void RegisterEventHandler<T>(Action<T> handler) where T: DeploymentEvent
            => OnRegisterEventHandler(handler);

        /// <summary>
        /// Unregister a handler for deployment events
        /// </summary>
        /// <typeparam name="T">Event type</typeparam>
        /// <param name="handler">Event handler</param>
        public void UnregisterEventHandler<T>(Action<T> handler) where T : DeploymentEvent
            => OnUnregisterEventHandler(handler);

        /// <summary>
        /// Raise a deployment event
        /// </summary>
        /// <typeparam name="T">Event type</typeparam>
        /// <param name="deploymentEvent">Deployment event to raise</param>
        public void RaiseDeploymentEvent<T>(T deploymentEvent) where T : DeploymentEvent
            => OnRaiseDeploymentEvent(deploymentEvent);

        /// <summary>
        /// Register a handler for deployment events
        /// </summary>
        /// <typeparam name="T">Event type</typeparam>
        /// <param name="handler">Event handler</param>
        protected abstract void OnRegisterEventHandler<T>(Action<T> handler) where T : DeploymentEvent;

        /// <summary>
        /// Unregister a handler for deployment events
        /// </summary>
        /// <typeparam name="T">Event type</typeparam>
        /// <param name="handler">Event handler</param>
        protected abstract void OnUnregisterEventHandler<T>(Action<T> handler) where T : DeploymentEvent;

        /// <summary>
        /// Raise a deployment event
        /// </summary>
        /// <typeparam name="T">Event type</typeparam>
        /// <param name="deploymentEvent">Deployment event to raise</param>
        protected abstract void OnRaiseDeploymentEvent<T>(T deploymentEvent) where T : DeploymentEvent;
    }
}