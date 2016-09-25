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

        internal List<IPackageSequence> PackageSequences { get; } = new List<IPackageSequence>();

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

        /// <summary>
        /// Allows to define primary packages to load
        /// </summary>
        /// <param name="packagesSequence">List of the packages</param>
        /// <returns>Configuration object</returns>
        public RuntimeConfigurationWithSource LoadPackagesFrom(IPackageSequence packagesSequence)
        {
            PackageSequences.Add(packagesSequence);
            return new RuntimeConfigurationWithSource(this);
        }
    }

    /// <summary>
    /// Provides configuration for runtime engine
    /// </summary>
    public class RuntimeConfigurationWithSource
    {
        private readonly RuntimeConfiguration _configuration;

        public RuntimeConfigurationWithSource(RuntimeConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Allows to define additional package sources
        /// </summary>
        /// <param name="packagesSequence">List of the packages to load</param>
        /// <returns>Configuration object</returns>
        public RuntimeConfigurationWithSource AlsoLoadPackagesFrom(IPackageSequence packagesSequence)
        {
            _configuration.PackageSequences.Add(packagesSequence);
            return this;
        }

        /// <summary>
        /// Adds default product configuration source
        /// </summary>
        /// <returns>Configuration object</returns>
        public RuntimeConfigurationWithSource AddDefaultConfigurationParser()
        {
            return AddProductConfigurationSource(new DefaultProductConfigurationSource(_configuration.RuntimeContext));
        }

        /// <summary>
        /// Allows to define custom product configuration source
        /// </summary>
        /// <param name="source">Custom configuration source</param>
        /// <returns>Configuration object</returns>
        public RuntimeConfigurationWithSource AddProductConfigurationSource(ProductConfigurationSource source)
        {
            _configuration.ProductConfigurationSources.Add(source);
            return this;
        }

        /// <summary>
        /// Allows to export host services to use by packages during startup
        /// </summary>
        /// <typeparam name="T">Service type</typeparam>
        /// <param name="serviceFactory">Service type factory to create a concrete service object</param>
        /// <returns>Configuration object</returns>
        public RuntimeConfigurationWithSource ExportHostService<T>(Func<T> serviceFactory) where T : class
        {
            _configuration.ServiceFactories.Add(typeof(T), serviceFactory);
            return this;
        }

        /// <summary>
        /// Creates IRuntimeEngine for the defined configuration
        /// </summary>
        /// <returns>Configuration object</returns>
        public IRuntimeEngine CreateEngine()
        {
            foreach (var serviceFactory in _configuration.ServiceFactories)
            {
                _configuration.RuntimeContext.RegisterHostService(serviceFactory.Key, serviceFactory.Value);
            }

            return new RuntimeEngine(_configuration);
        }
    }
}

