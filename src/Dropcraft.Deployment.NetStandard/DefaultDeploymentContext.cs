using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Dropcraft.Common;
using Dropcraft.Common.Configuration;

namespace Dropcraft.Deployment
{
    public class DefaultDeploymentContext : DeploymentContext
    {
        private readonly ConcurrentDictionary<string, object> _handlers = new ConcurrentDictionary<string, object>();
        private readonly ReaderWriterLockSlim _eventLock = new ReaderWriterLockSlim();

        public DefaultDeploymentContext(string productPath, string framework,
            IPackageConfigurationProvider packageConfigurationProvider,
            IProductConfigurationProvider productConfigurationProvider)
        {
            ProductPath = productPath;
            TargetFramework = framework;

            PackageConfigurationProvider = packageConfigurationProvider;
            ProductConfigurationProvider = productConfigurationProvider;
        }

        protected override void OnRegisterEventHandler<T>(Action<T> handler)
        {
            var list = (List<Action<T>>)_handlers.GetOrAdd(typeof(T).Name, x => new List<Action<T>>());
            list.Remove(handler);

            try
            {
                _eventLock.EnterWriteLock();
                list.Add(handler);
            }
            finally
            {
                _eventLock.ExitWriteLock();
            }
        }

        protected override void OnUnregisterEventHandler<T>(Action<T> handler)
        {
            object listObject;
            if (_handlers.TryGetValue(typeof(T).Name, out listObject))
            {
                var list = (List<Action<T>>)listObject;
                try
                {
                    _eventLock.EnterWriteLock();
                    list.Remove(handler);
                }
                finally
                {
                    _eventLock.ExitWriteLock();
                }
            }
        }

        protected override void OnRaiseDeploymentEvent<T>(T deploymentEvent)
        {
            if (deploymentEvent.Context == null)
                deploymentEvent.Context = this;

            object listObject;
            if (_handlers.TryGetValue(typeof(T).Name, out listObject))
            {
                var list = (List<Action<T>>)listObject;
                try
                {
                    _eventLock.EnterReadLock();
                    foreach (var handler in list)
                    {
                        handler(deploymentEvent);
                    }
                }
                finally
                {
                    _eventLock.ExitReadLock();
                }
            }
        }
    }
}