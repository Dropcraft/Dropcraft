using System;
using System.Collections.Generic;
using System.IO;
using Dropcraft.Common;
using Dropcraft.Common.Handler;

namespace Dropcraft.Deployment
{
    public class DefaultDeploymentContext : DeploymentContext
    {
        private readonly object _eventsLock = new object();
        private readonly List<IDeploymentEventsHandler> _eventHandlers = new List<IDeploymentEventsHandler>();

        public DefaultDeploymentContext(string productPath)
            : this(productPath, string.Empty, string.Empty)
        {
        }

        public DefaultDeploymentContext(string productPath, string packagesFolderPath, string framework) 
        {
            ProductPath = productPath;
            TargetFramework = framework;

            if (string.IsNullOrWhiteSpace(packagesFolderPath))
            {
                PackagesFolderPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                DontCachePackages = true;
            }
            else
            {
                PackagesFolderPath = packagesFolderPath;
            }
        }

        protected override void OnRaiseDeploymentEvent(DeploymentEvent deploymentEvent)
        {
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