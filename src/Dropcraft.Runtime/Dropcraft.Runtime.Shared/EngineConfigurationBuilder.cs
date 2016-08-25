using System;
using System.Collections.Generic;
using Dropcraft.Contracts;
using Dropcraft.Contracts.Configuration;
using Dropcraft.Runtime.Configuration;

namespace Dropcraft.Runtime
{
    public interface IEngineConfigurationBuilder
    {
        IEngineConfigurationBuilder ThenLoadFrom(IEnumerable<PackageInfo> packagesSequence);

        IEngineConfigurationBuilder WithContext(RuntimeContext context);

        IEngineConfigurationBuilder AddPackageConfigurationParser(IPackageConfigurationParser parser);

        /// <summary>
        /// WatchForDropinPackages instructs the engine to monitor provided folder and load packages dropped there in runtime
        /// </summary>
        /// <param name="path">Path to watch for the additional packages</param>
        /// <param name="period">The time interval between probing the path</param>
        /// <returns></returns>
        IEngineConfigurationBuilder WatchForDropInPackages(string path, TimeSpan period);

        IEngineConfigurationBuilder ExportHostService<T>(Func<T> serviceFactory) where T: class;

        EngineConfiguration Build();
    }

    public class EngineConfigurationBuilder : IEngineConfigurationBuilder
    {
        readonly EngineConfiguration _configuration = new EngineConfiguration();

        private EngineConfigurationBuilder()
        {
        }

        public static IEngineConfigurationBuilder LoadPackagesFrom(IEnumerable<PackageInfo> packagesSequence)
        {
            var builder = new EngineConfigurationBuilder();
            builder._configuration.AddPackageSources(packagesSequence);

            return builder;
        }

        public IEngineConfigurationBuilder ThenLoadFrom(IEnumerable<PackageInfo> packagesSequence)
        {
            _configuration.AddPackageSources(packagesSequence);
            return this;
        }

        public IEngineConfigurationBuilder AddPackageConfigurationParser(IPackageConfigurationParser parser)
        {
            _configuration.PackageConfigurationParsers.Add(parser);
            return this;
        }

        public IEngineConfigurationBuilder WithContext(RuntimeContext context)
        {
            _configuration.RuntimeContext = context;
            return this;
        }

        public IEngineConfigurationBuilder WatchForDropInPackages(string path, TimeSpan period)
        {
            _configuration.DropInPath = path;
            _configuration.DropInWatchPeriod = period;
            return this;
        }

        public IEngineConfigurationBuilder ExportHostService<T>(Func<T> serviceFactory) where T : class
        {
            _configuration.ServiceFactories.Add(typeof(T), serviceFactory);
            return this;
        }

        public EngineConfiguration Build()
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