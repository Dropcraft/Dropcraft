using System;
using System.Collections.Generic;
using Dropcraft.Common;
using Dropcraft.Common.Configuration;
using Dropcraft.Runtime.Configuration;

namespace Dropcraft.Runtime
{
    public interface IRuntimeConfigurationBuilder
    {
        IRuntimeConfigurationBuilder ThenLoadFrom(IEnumerable<PackageInfo> packagesSequence);

        IRuntimeConfigurationBuilder WithContext(RuntimeContext context);

        IRuntimeConfigurationBuilder AddPackageConfigurationParser(IRuntimePackageConfigParser parser);

        IRuntimeConfigurationBuilder ExportHostService<T>(Func<T> serviceFactory) where T: class;

        RuntimeConfiguration Build();
    }

    public class RuntimeConfigurationBuilder : IRuntimeConfigurationBuilder
    {
        readonly RuntimeConfiguration _configuration = new RuntimeConfiguration();

        private RuntimeConfigurationBuilder()
        {
        }

        public static IRuntimeConfigurationBuilder LoadPackagesFrom(IEnumerable<PackageInfo> packagesSequence)
        {
            var builder = new RuntimeConfigurationBuilder();
            builder._configuration.AddPackageSources(packagesSequence);

            return builder;
        }

        public IRuntimeConfigurationBuilder ThenLoadFrom(IEnumerable<PackageInfo> packagesSequence)
        {
            _configuration.AddPackageSources(packagesSequence);
            return this;
        }

        public IRuntimeConfigurationBuilder AddPackageConfigurationParser(IRuntimePackageConfigParser parser)
        {
            _configuration.PackageConfigurationParsers.Add(parser);
            return this;
        }

        public IRuntimeConfigurationBuilder WithContext(RuntimeContext context)
        {
            _configuration.RuntimeContext = context;
            return this;
        }

        public IRuntimeConfigurationBuilder ExportHostService<T>(Func<T> serviceFactory) where T : class
        {
            _configuration.ServiceFactories.Add(typeof(T), serviceFactory);
            return this;
        }

        public RuntimeConfiguration Build()
        {
            if (_configuration.RuntimeContext == null)
                _configuration.RuntimeContext = new DefaultRuntimeContext();

            foreach (var serviceFactory in _configuration.ServiceFactories)
            {
                _configuration.RuntimeContext.RegisterHostService(serviceFactory.Key, serviceFactory.Value);
            }

            if (_configuration.PackageConfigurationParsers.Count == 0)
                _configuration.PackageConfigurationParsers.Add(new PackageManifestParser());

            return _configuration;
        }
    }
}