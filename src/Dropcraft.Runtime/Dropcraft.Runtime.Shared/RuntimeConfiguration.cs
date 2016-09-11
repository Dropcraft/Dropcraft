using System;
using System.Collections.Generic;
using Dropcraft.Common;
using Dropcraft.Common.Configuration;
using Dropcraft.Common.Package;
using Dropcraft.Runtime.Configuration;

namespace Dropcraft.Runtime
{
    /// <summary>
    /// Provides configuration for runtime engine
    /// </summary>
    public class RuntimeConfiguration
    {
        public RuntimeContext RuntimeContext { get; set; }

        public List<PackageInfo> PackageSources { get; } = new List<PackageInfo>();

        public List<IRuntimePackageConfigParser> PackageConfigurationParsers { get; } = new List<IRuntimePackageConfigParser>();

        public IDictionary<Type, Func<object>> ServiceFactories { get; } = new Dictionary<Type, Func<object>>();

        internal void AddPackageSources(IEnumerable<PackageInfo> packages)
        {
            PackageSources.AddRange(packages);
        }

        public RuntimeConfiguration() 
            : this(new DefaultRuntimeContext())
        {
        }

        public RuntimeConfiguration(RuntimeContext runtimeContext)
        {
            RuntimeContext = runtimeContext;
        }

        /// <summary>
        /// Allows to define primary package to load
        /// </summary>
        /// <param name="packagesSequence">List of the packages</param>
        /// <returns>Configuration object</returns>
        public RuntimeConfigurationWithSource LoadPackagesFrom(IEnumerable<PackageInfo> packagesSequence)
        {
            AddPackageSources(packagesSequence);
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
        public RuntimeConfigurationWithSource AlsoLoadPackagesFrom(IEnumerable<PackageInfo> packagesSequence)
        {
            _configuration.AddPackageSources(packagesSequence);
            return this;
        }

        /// <summary>
        /// Adds default package configuration parser
        /// </summary>
        /// <returns>Configuration object</returns>
        public RuntimeConfigurationWithSource AddDefaultConfigurationParser()
        {
            _configuration.PackageConfigurationParsers.Add(new PackageManifestParser());
            return this;
        }

        /// <summary>
        /// Allows to define custom package configuration parsers
        /// </summary>
        /// <param name="parser">Custom configuration parser</param>
        /// <returns>Configuration object</returns>
        public RuntimeConfigurationWithSource AddPackageConfigurationParser(IRuntimePackageConfigParser parser)
        {
            _configuration.PackageConfigurationParsers.Add(parser);
            return this;
        }

        /// <summary>
        /// Allows to export host services to use by packages during startup
        /// </summary>
        /// <typeparam name="T">Service type</typeparam>
        /// <param name="serviceFactory">Service type facory to create a concrete service object</param>
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

