using System;
using System.Collections.Generic;
using Dropcraft.Common;
using Dropcraft.Common.Configuration;
using Dropcraft.Runtime.Configuration;

namespace Dropcraft.Runtime
{
    /// <summary>
    /// Provides configuration for runtime engine
    /// </summary>
    public class RuntimeConfiguration
    {
        /// <summary>
        /// Assigned runtime context
        /// </summary>
        public RuntimeContext RuntimeContext { get; }

        internal List<ProductConfigurationSource> ProductConfigurationSources { get; } = new List<ProductConfigurationSource>();

        internal IDictionary<Type, Func<object>> ServiceFactories { get; } = new Dictionary<Type, Func<object>>();

        /// <summary>
        /// Constructor
        /// </summary>
        public RuntimeConfiguration(string productPath) 
            : this(new DefaultRuntimeContext(productPath))
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="runtimeContext">Configured custom runtime context to use</param>
        public RuntimeConfiguration(RuntimeContext runtimeContext)
        {
            RuntimeContext = runtimeContext;
        }

        public RuntimeConfigurationForPackagesSource LoadPackages => new RuntimeConfigurationForPackagesSource(this);

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
        /// <returns>Configuration object</returns>
        public IRuntimeEngine CreateEngine()
        {
            foreach (var serviceFactory in ServiceFactories)
            {
                RuntimeContext.RegisterHostService(serviceFactory.Key, serviceFactory.Value);
            }

            return new RuntimeEngine(this);
        }

    }

    /// <summary>
    /// Allows to define package configuration sources
    /// </summary>
    public class RuntimeConfigurationForPackagesSource
    {
        private readonly RuntimeConfiguration _runtimeConfiguration;

        public RuntimeConfigurationForPackagesSource(RuntimeConfiguration runtimeConfiguration)
        {
            _runtimeConfiguration = runtimeConfiguration;
        }

        /// <summary>
        /// Instructs to use product configuration from the installation folder. Can be combined with the custom package sources.
        /// </summary>
        /// <returns>Configuration object</returns>
        public RuntimeConfiguration UsingProductConfiguration()
        {
            _runtimeConfiguration.ProductConfigurationSources.Add(new DefaultProductConfigurationSource(_runtimeConfiguration.RuntimeContext));
            return _runtimeConfiguration;
        }

        /// <summary>
        /// Instruct to add a custom package configuration source. Can be used instead and in combination with the default configuration 
        /// to add standalone packages and to control the package loading order.
        /// </summary>
        /// <param name="configurationSource">Custom configuration source</param>
        /// <returns>Configuration object</returns>
        public RuntimeConfiguration UsingCustomConfiguration(ProductConfigurationSource configurationSource)
        {
            _runtimeConfiguration.ProductConfigurationSources.Add(configurationSource);
            return _runtimeConfiguration;
        }
    }
}

