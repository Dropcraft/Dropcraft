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

        internal IProductConfigurationSource ProductConfigurationSource { get; set; }

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

        public RuntimeConfigurationForPackagesSource ForProductConfiguration => new RuntimeConfigurationForPackagesSource(this);

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
        /// Instructs to use product configuration from the installation folder.
        /// </summary>
        /// <returns>Configuration object</returns>
        public RuntimeConfiguration UseDefaultConfigurationSource()
        {
            _runtimeConfiguration.ProductConfigurationSource = new ProductConfigurationSource();
            return _runtimeConfiguration;
        }

        /// <summary>
        /// Instruct to use a custom package configuration source.
        /// </summary>
        /// <param name="configurationSource">Custom configuration source</param>
        /// <returns>Configuration object</returns>
        public RuntimeConfiguration UseCustomConfigurationSource(IProductConfigurationSource configurationSource)
        {
            _runtimeConfiguration.ProductConfigurationSource = configurationSource;
            return _runtimeConfiguration;
        }
    }
}

