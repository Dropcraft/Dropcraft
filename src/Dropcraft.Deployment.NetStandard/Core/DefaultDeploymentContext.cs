using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Dropcraft.Common;
using Dropcraft.Common.Deployment;
using Dropcraft.Common.Package;

namespace Dropcraft.Deployment.Core
{
    /// <summary>
    /// Class DefaultDeploymentContext.
    /// </summary>
    /// <seealso cref="Dropcraft.Common.Deployment.DeploymentContext" />
    public class DefaultDeploymentContext : DeploymentContext
    {
        private readonly ConcurrentDictionary<string, object> _handlers = new ConcurrentDictionary<string, object>();
        private readonly ReaderWriterLockSlim _eventLock = new ReaderWriterLockSlim();

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultDeploymentContext"/> class.
        /// </summary>
        /// <param name="productPath">The product path.</param>
        /// <param name="framework">The target framework.</param>
        /// <param name="packageConfigurationProvider">The package configuration provider.</param>
        /// <param name="productConfigurationProvider">The product configuration provider.</param>
        public DefaultDeploymentContext(string productPath, string framework,
            IPackageConfigurationProvider packageConfigurationProvider,
            IProductConfigurationProvider productConfigurationProvider)
        {
            ProductPath = productPath;
            TargetFramework = framework;

            PackageConfigurationProvider = packageConfigurationProvider;
            ProductConfigurationProvider = productConfigurationProvider;
        }

        /// <summary>
        /// Register a handler for deployment events
        /// </summary>
        /// <typeparam name="T">Event type</typeparam>
        /// <param name="handler">Event handler</param>
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

        /// <summary>
        /// Unregister a handler for deployment events
        /// </summary>
        /// <typeparam name="T">Event type</typeparam>
        /// <param name="handler">Event handler</param>
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

        /// <summary>
        /// Raise a deployment event
        /// </summary>
        /// <typeparam name="T">Event type</typeparam>
        /// <param name="deploymentEvent">Deployment event to raise</param>
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