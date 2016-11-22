using System.Collections.Generic;
using Dropcraft.Common;
using Dropcraft.Common.Configuration;
using Dropcraft.Common.Handler;

namespace Dropcraft.Deployment
{
    public class DefaultDeploymentContext : DeploymentContext
    {
        private readonly object _eventsLock = new object();
        private readonly List<IDeploymentEventsHandler> _eventHandlers = new List<IDeploymentEventsHandler>();

        public DefaultDeploymentContext(string productPath, string framework,
            IPackageConfigurationProvider packageConfigurationProvider,
            IProductConfigurationProvider productConfigurationProvider)
        {
            ProductPath = productPath;
            TargetFramework = framework;

            PackageConfigurationProvider = packageConfigurationProvider;
            ProductConfigurationProvider = productConfigurationProvider;
        }

        protected override void OnRaiseDeploymentEvent(DeploymentEvent deploymentEvent)
        {
            if (deploymentEvent.Context == null)
                deploymentEvent.Context = this;

            lock (_eventsLock)
            {
                foreach (var eventHandler in _eventHandlers)
                {
                    deploymentEvent.HandleEvent(eventHandler);
                }
            }
        }

        protected override void OnRegisterDeploymentEventHandler(IDeploymentEventsHandler deploymentEventsHandler)
        {
            lock (_eventsLock)
            {
                _eventHandlers.Add(deploymentEventsHandler);
            }
        }

        protected override void OnUnregisterDeploymentEventHandler(IDeploymentEventsHandler deploymentEventsHandler)
        {
            lock (_eventsLock)
            {
                _eventHandlers.Remove(deploymentEventsHandler);
            }
        }
    }
}