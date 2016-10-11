using System.Collections.Generic;
using Dropcraft.Common;
using Dropcraft.Common.Handler;

namespace Dropcraft.Deployment
{
    public class DeploymentContext : IDeploymentContext
    {
        private readonly object _eventsLock = new object();
        private readonly List<IDeploymentEventsHandler> _eventHandlers = new List<IDeploymentEventsHandler>();

        public string ProductPath { get; }

        public string PackagesFolderPath { get; }

        public DeploymentContext(string productPath, string packagesFolderPath) 
        {
            ProductPath = productPath;
            PackagesFolderPath = packagesFolderPath;
        }

        public void RaiseDeploymentEvent(DeploymentEvent deploymentEvent)
        {
            lock (_eventsLock)
            {
                foreach (var eventHandler in _eventHandlers)
                {
                    deploymentEvent.HandleEvent(eventHandler);
                }
            }
        }

        public void RegisterDeploymentEventHandler(IDeploymentEventsHandler deploymentEventsHandler)
        {
            lock (_eventsLock)
            {
                _eventHandlers.Add(deploymentEventsHandler);
            }
        }

        public void UnregisterDeploymentEventHandler(IDeploymentEventsHandler deploymentEventsHandler)
        {
            lock (_eventsLock)
            {
                _eventHandlers.Remove(deploymentEventsHandler);
            }
        }
    }
}