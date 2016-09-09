using System;
using System.Collections.Generic;
using Dropcraft.Common;
using Dropcraft.Common.Configuration;
using Dropcraft.Common.Package;
using Dropcraft.Runtime.Configuration;

namespace Dropcraft.Runtime
{
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
            PackageConfigurationParsers.Add(new PackageManifestParser());
        }

        public RuntimeConfigurationWithSource LoadPackagesFrom(IEnumerable<PackageInfo> packagesSequence)
        {
            AddPackageSources(packagesSequence);
            return new RuntimeConfigurationWithSource(this);
        }
    }

    public class RuntimeConfigurationWithSource
    {
        private readonly RuntimeConfiguration _configuration;

        public RuntimeConfigurationWithSource(RuntimeConfiguration configuration)
        {
            _configuration = configuration;
        }

        public RuntimeConfigurationWithSource AlsoLoadPackagesFrom(IEnumerable<PackageInfo> packagesSequence)
        {
            _configuration.AddPackageSources(packagesSequence);
            return this;
        }

        public RuntimeConfigurationWithSource AddPackageConfigurationParser(IRuntimePackageConfigParser parser)
        {
            _configuration.PackageConfigurationParsers.Add(parser);
            return this;
        }

        public RuntimeConfigurationWithSource ExportHostService<T>(Func<T> serviceFactory) where T : class
        {
            _configuration.ServiceFactories.Add(typeof(T), serviceFactory);
            return this;
        }

        public IRuntimeEngine CreatEngine()
        {
            foreach (var serviceFactory in _configuration.ServiceFactories)
            {
                _configuration.RuntimeContext.RegisterHostService(serviceFactory.Key, serviceFactory.Value);
            }

            return new RuntimeEngine(_configuration);
        }
    }
}

