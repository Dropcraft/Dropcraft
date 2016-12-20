using System;
using System.Collections.Generic;
using Dropcraft.Common;
using Dropcraft.Common.Package;
using Dropcraft.Common.Runtime;
using Dropcraft.Runtime.Configuration;
using Dropcraft.Runtime.Core;

namespace Dropcraft.Runtime
{
    /// <summary>
    /// Provides configuration for runtime engine
    /// </summary>
    public class RuntimeConfiguration
    {
        internal IProductConfigurationSource ProductConfigurationSource { get; set; } = new ProductConfigurationSource();

        internal IDictionary<Type, Func<object>> ServiceFactories { get; } = new Dictionary<Type, Func<object>>();

        internal IEntityActivator EntityActivator { get; set; } = new ReflectionEntityActivator();

        /// <summary>
        /// Product configuration
        /// </summary>
        public ProductConfigurationSourceOptions ForProductConfiguration => new ProductConfigurationSourceOptions(this);

        /// <summary>
        /// Runtime extensibility configuration
        /// </summary>
        public ExtensibilityOptions ForExtensibility => new ExtensibilityOptions(this);

        /// <summary>
        /// Allows to export host services to use by packages during startup
        /// </summary>
        /// <typeparam name="T">Service type</typeparam>
        /// <param name="serviceFactory">Service type factory to create a concrete service object</param>
        /// <returns>Configuration object</returns>
        public RuntimeConfiguration ExportHostService<T>(Func<T> serviceFactory) where T : class
        {
            ServiceFactories.Add(typeof(T), serviceFactory);
            return this;
        }

        /// <summary>
        /// Creates IRuntimeEngine for the defined configuration
        /// </summary>
        /// <param name="runtimeContext">Custom runtime context</param>
        /// <returns>Configured runtime engine</returns>
        public IRuntimeEngine CreateEngine(RuntimeContext runtimeContext)
        {
            foreach (var serviceFactory in ServiceFactories)
            {
                runtimeContext.RegisterHostService(serviceFactory.Key, serviceFactory.Value);
            }

            return new RuntimeEngine(runtimeContext,
                ProductConfigurationSource.GetProductConfigurationProvider(runtimeContext.ProductPath), EntityActivator);
        }

        /// <summary>
        /// Creates IRuntimeEngine for the defined configuration
        /// </summary>
        /// <param name="productPath">Target product path</param>
        /// <returns>Configured runtime engine</returns>
        public IRuntimeEngine CreateEngine(string productPath)
        {
            return CreateEngine(new DefaultRuntimeContext(productPath));
        }
    }
}

